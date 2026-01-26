using System;
using System.Collections.Generic;
using System.Linq;
using MyOrm.Metadata;
using MyOrm.Sql;
using Npgsql;

namespace MyOrm.Migrations
{
    public sealed class MigrationGenerator
    {
        private readonly NpgsqlConnection _conn;

        public MigrationGenerator(NpgsqlConnection conn) => _conn = conn ?? throw new ArgumentNullException(nameof(conn));

        public MigrationSql Generate(string name, IEnumerable<Type> entityTypes)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Migration name is required.", nameof(name));

            ArgumentNullException.ThrowIfNull(entityTypes);

            var up = new List<string>();
            var down = new List<string>();

            foreach (var map in entityTypes.Select(ModelInspector.BuildEntityMap))
                AppendEntityMigration(map, up, down);

            return new MigrationSql
            {
                Name = name,
                Up = up,
                Down = down
            };
        }

        private void AppendEntityMigration(EntityMap map, List<string> up, List<string> down)
        {
            if (!TableExists(map.TableName))
            {
                up.Add(CreateTableBuilder.BuildCreateTable(map));
                down.Insert(0, $"DROP TABLE IF EXISTS {map.TableName};");
                return;
            }

            var dbCols = GetDbColumns(map.TableName);

            foreach (var col in map.Columns.Where(c => !dbCols.Contains(c.ColumnName)))
            {
                up.Add(BuildAddColumnSql(map.TableName, col));
                down.Insert(0, $"ALTER TABLE {map.TableName} DROP COLUMN IF EXISTS {col.ColumnName};");
            }
        }

        private static string BuildAddColumnSql(string tableName, ColumnMap col)
        {
            var pgType = SqlTypeMapper.ToPostgresType(col.DbType, col.Length);

            var sql = $"ALTER TABLE {tableName} ADD COLUMN {col.ColumnName} {pgType}";
            if (!col.IsNullable) sql += " NOT NULL";
            if (!string.IsNullOrWhiteSpace(col.DefaultSql)) sql += $" DEFAULT {col.DefaultSql}";
            if (col.IsUnique) sql += " UNIQUE";

            return sql + ";";
        }

        private bool TableExists(string table)
        {
            using var cmd = new NpgsqlCommand(
                "SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='public' AND table_name=@t);",
                _conn);

            cmd.Parameters.AddWithValue("@t", table);
            return (bool)cmd.ExecuteScalar()!;
        }

        private ISet<string> GetDbColumns(string table)
        {
            using var cmd = new NpgsqlCommand(
                "SELECT column_name FROM information_schema.columns WHERE table_schema='public' AND table_name=@t;",
                _conn);

            cmd.Parameters.AddWithValue("@t", table);

            using var r = cmd.ExecuteReader();
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            while (r.Read())
                set.Add(r.GetString(0));

            return set;
        }
    }
}
