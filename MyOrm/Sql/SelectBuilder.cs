using MyOrm.Metadata;

namespace MyOrm.Sql
{
    public static class SelectBuilder
    {
        public static string BuildSelectAll(EntityMap entity) =>
            $"SELECT {entity.ColumnList()} FROM {entity.TableName};";

        public static string BuildSelectById(EntityMap entity) =>
            $"SELECT {entity.ColumnList()} FROM {entity.TableName} WHERE {entity.KeyColumnName()} = @id LIMIT 1;";

        public static string BuildSelectWhere(EntityMap entity, string whereSql) =>
            $"SELECT {entity.ColumnList()} FROM {entity.TableName} WHERE {whereSql};";
    }
}
