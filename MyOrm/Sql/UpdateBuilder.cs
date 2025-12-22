using MyOrm.Metadata;

namespace MyOrm.Sql
{
    public static class UpdateBuilder
    {
        public static string BuildUpdate(EntityMap entity)
        {
            var updatable = entity.Columns
                .Where(c => c.Property != entity.KeyProperty);

            return Build(entity, updatable);
        }

        public static string BuildUpdateChanged(EntityMap entity, IReadOnlyList<ColumnMap> changedColumns) =>
            Build(entity, changedColumns);

        private static string Build(EntityMap entity, IEnumerable<ColumnMap> columns)
        {
            var setParts = string.Join(", ", columns.Select((c, i) => $"{c.ColumnName} = @p{i}"));
            return $"UPDATE {entity.TableName} SET {setParts} WHERE {entity.KeyColumnName()} = @id;";
        }
    }
}
