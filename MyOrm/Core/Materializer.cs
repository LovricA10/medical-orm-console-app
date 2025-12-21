using MyOrm.Metadata;
using Npgsql;

namespace MyOrm.Core
{
    internal static class Materializer
    {
        public static T Materialize<T>(EntityMap map, NpgsqlDataReader reader) where T : new()
        {
            ArgumentNullException.ThrowIfNull(map);
            ArgumentNullException.ThrowIfNull(reader);

            var obj = new T();

            foreach (var c in map.Columns)
            {
                var raw = reader[c.ColumnName];
                var value = raw == DBNull.Value ? null : raw;

                var targetType = Nullable.GetUnderlyingType(c.Property.PropertyType) ?? c.Property.PropertyType;

                if (value is not null)
                    value = Coerce(value, targetType);

                c.Property.SetValue(obj, value);
            }

            return obj;
        }

        private static object? Coerce(object value, Type targetType)
        {
            if (targetType.IsAssignableFrom(value.GetType()))
                return value;

            if (targetType.IsEnum)
            {
                var underlying = Enum.GetUnderlyingType(targetType);
                var converted = Convert.ChangeType(value, underlying);
                return converted is null ? null : Enum.ToObject(targetType, converted);
            }

            return Convert.ChangeType(value, targetType);
        }
    }
}
