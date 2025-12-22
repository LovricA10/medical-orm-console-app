using MyOrm.Metadata;

namespace MyOrm.Sql
{
    internal static class SqlMetadataExtensions
    {
        public static string KeyColumnName(this EntityMap entity) =>
            entity.Columns.First(c => c.Property == entity.KeyProperty).ColumnName;

        public static string ColumnList(this EntityMap entity) =>
            string.Join(", ", entity.Columns.Select(c => c.ColumnName));
    }
}
