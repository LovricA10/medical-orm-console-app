using MyOrm.Metadata;

namespace MyOrm.Sql
{
    public static class DeleteBuilder
    {
        public static string BuildDeleteById(EntityMap entity) =>
            $"DELETE FROM {entity.TableName} WHERE {entity.KeyColumnName()} = @id;";
    }
}
