using MyOrm.Metadata;

namespace MyOrm.Sql
{
    public static class InsertBuilder
    {
        public static string BuildInsert(EntityMap entity)
        {
            var columns = entity.Columns
                .Where(c => c.Property != entity.KeyProperty || !entity.KeyAutoIncrement)
                .ToList();

            var columnNames = string.Join(", ", columns.Select(c => c.ColumnName));
            var paramNames = string.Join(", ", columns.Select((_, i) => $"@p{i}"));

            return $"INSERT INTO {entity.TableName} ({columnNames}) VALUES ({paramNames});";
        }
    }
}
