using Npgsql;

namespace MyOrm.Migrations
{
    public sealed class MigrationApplier
    {
        private const string MigrationsTable = "__migrations";

        private readonly NpgsqlConnection _conn;

        public MigrationApplier(NpgsqlConnection conn)
        {
            _conn = conn ?? throw new ArgumentNullException(nameof(conn));
            EnsureMigrationsTable();
        }

        public void ApplyUp(MigrationSql migration)
        {
            ArgumentNullException.ThrowIfNull(migration);
            if (IsApplied(migration.Name)) return;

            using var tx = _conn.BeginTransaction();

            ExecuteBatch(migration.Up, tx);

            using var cmd = new NpgsqlCommand(
                $"INSERT INTO {MigrationsTable}(name, up_sql, down_sql) VALUES (@n, @u, @d);",
                _conn, tx);

            cmd.Parameters.AddWithValue("@n", migration.Name);
            cmd.Parameters.AddWithValue("@u", string.Join("\n", migration.Up));
            cmd.Parameters.AddWithValue("@d", string.Join("\n", migration.Down));
            cmd.ExecuteNonQuery();

            tx.Commit();
        }

        public void ApplyDown(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Migration name is required.", nameof(name));

            var (Id, DownSql) = GetMigration(name);
            if (Id is null) return;

            using var tx = _conn.BeginTransaction();

            var statements = SplitStatements(DownSql);
            ExecuteBatch(statements, tx);

            using var cmd = new NpgsqlCommand($"DELETE FROM {MigrationsTable} WHERE id=@id;", _conn, tx);
            cmd.Parameters.AddWithValue("@id", Id.Value);
            cmd.ExecuteNonQuery();

            tx.Commit();
        }

        private void EnsureMigrationsTable()
        {
            const string sql = @"
              CREATE TABLE IF NOT EXISTS __migrations(
              id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
              name VARCHAR(200) NOT NULL UNIQUE,
              applied_at TIMESTAMP NOT NULL DEFAULT NOW(),
              up_sql TEXT NOT NULL,
              down_sql TEXT NOT NULL
        );";
            using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.ExecuteNonQuery();
        }

        private static IEnumerable<string> SplitStatements(string sql) =>
            sql.Split('\n', StringSplitOptions.RemoveEmptyEntries)
               .Select(s => s.Trim())
               .Where(s => s.Length > 0);

        private void ExecuteBatch(IEnumerable<string> statements, NpgsqlTransaction tx)
        {
            foreach (var sql in statements)
            {
                using var cmd = new NpgsqlCommand(sql, _conn, tx);
                cmd.ExecuteNonQuery();
            }
        }

        private bool IsApplied(string name)
        {
            using var cmd = new NpgsqlCommand(
                $"SELECT EXISTS (SELECT 1 FROM {MigrationsTable} WHERE name=@n);",
                _conn);

            cmd.Parameters.AddWithValue("@n", name);
            return (bool)cmd.ExecuteScalar()!;
        }

        private (int? Id, string DownSql) GetMigration(string name)
        {
            using var cmd = new NpgsqlCommand(
                $"SELECT id, down_sql FROM {MigrationsTable} WHERE name=@n;",
                _conn);

            cmd.Parameters.AddWithValue("@n", name);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return (null, string.Empty);

            return (r.GetInt32(0), r.GetString(1));
        }
    }
}
