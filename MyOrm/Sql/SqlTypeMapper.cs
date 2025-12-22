using MyOrm.Attributes;

namespace MyOrm.Sql
{
    public static class SqlTypeMapper
    {
        public static string ToPostgresType(DbType type, int length) =>
            type switch
            {
                DbType.Int => "INTEGER",
                DbType.Decimal => "DECIMAL",
                DbType.Float => "REAL",
                DbType.Varchar => length > 0 ? $"VARCHAR({length})" : "VARCHAR",
                DbType.Char => length > 0 ? $"CHAR({length})" : "CHAR",
                DbType.Text => "TEXT",
                DbType.TimestampWithTimezone => "TIMESTAMPTZ",
                DbType.TimestampWithoutTimezone => "TIMESTAMP",
                _ => throw new NotSupportedException($"Unsupported DbType: {type}")
            };
    }
}
