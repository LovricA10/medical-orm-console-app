using System.Reflection;
using MyOrm.Attributes;

namespace MyOrm.Metadata
{
    public static class ModelInspector
    {
        public static EntityMap BuildEntityMap<T>() => BuildEntityMap(typeof(T));

        public static EntityMap BuildEntityMap(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);

            var tableName = GetTableName(type);

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var columns = properties
                .Where(IsMappedProperty)
                .Select(BuildColumnMap)
                .ToList();

            var (keyProperty, keyAutoIncrement) = GetKey(columns, type);

            return new EntityMap
            {
                ClrType = type,
                TableName = tableName,
                KeyProperty = keyProperty,
                KeyAutoIncrement = keyAutoIncrement,
                Columns = columns
            };
        }

        private static string GetTableName(Type type)
        {
            var tableAttr = type.GetCustomAttribute<TableAttribute>()
                ?? throw new InvalidOperationException($"Missing [Table] on {type.Name}");

            return tableAttr.Name;
        }

        private static bool IsMappedProperty(PropertyInfo property) =>
            property.GetCustomAttribute<IgnoreAttribute>() is null
            && property.GetCustomAttribute<ColumnAttribute>() is not null;

        private static ColumnMap BuildColumnMap(PropertyInfo property)
        {
            var colAttr = property.GetCustomAttribute<ColumnAttribute>()!;
            var fkAttr = property.GetCustomAttribute<ForeignKeyAttribute>();

            return new ColumnMap
            {
                Property = property,
                ColumnName = colAttr.Name,
                DbType = colAttr.Type,
                Length = colAttr.Length,
                IsNullable = colAttr.IsNullable,
                IsUnique = colAttr.IsUnique,
                DefaultSql = colAttr.DefaultSql,
                ForeignKeyRefType = fkAttr?.ReferenceType,
                ForeignKeyRefColumn = fkAttr?.ReferenceColumn
            };
        }

        private static (PropertyInfo KeyProperty, bool KeyAutoIncrement) GetKey(
            IReadOnlyList<ColumnMap> columns,
            Type type)
        {
            var (Column, Key) = columns
                .Select(c => (Column: c, Key: c.Property.GetCustomAttribute<KeyAttribute>()))
                .FirstOrDefault(x => x.Key is not null);

            if (Column is null || Key is null)
                throw new InvalidOperationException($"Missing [Key] on {type.Name}");

            return (Column.Property, Key.AutoIncrement);
        }
    }
}
