using System.Text;
using MyOrm.Metadata;

namespace MyOrm.Sql
{
    public static class CreateTableBuilder
    {
        public static string BuildCreateTable(EntityMap entity)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"CREATE TABLE IF NOT EXISTS {entity.TableName} (");

            for (var i = 0; i < entity.Columns.Count; i++)
            {
                var c = entity.Columns[i];
                sb.Append($"  {c.ColumnName} {SqlTypeMapper.ToPostgresType(c.DbType, c.Length)}");

                if (c.Property == entity.KeyProperty)
                {
                    sb.Append(" PRIMARY KEY");
                    if (entity.KeyAutoIncrement)
                        sb.Append(" GENERATED ALWAYS AS IDENTITY");
                }

                if (!c.IsNullable) sb.Append(" NOT NULL");
                if (c.IsUnique) sb.Append(" UNIQUE");
                if (!string.IsNullOrWhiteSpace(c.DefaultSql))
                    sb.Append($" DEFAULT {c.DefaultSql}");

                sb.AppendLine(i == entity.Columns.Count - 1 ? string.Empty : ",");
            }

            sb.AppendLine(");");
            return sb.ToString();
        }
    }
}
