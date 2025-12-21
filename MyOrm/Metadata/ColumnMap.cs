using MyOrm.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyOrm.Metadata
{
    public sealed class ColumnMap
    {
        public required PropertyInfo Property { get; init; }
        public required string ColumnName { get; init; }
        public required DbType DbType { get; init; }
        public int Length { get; init; }
        public bool IsNullable { get; init; }
        public bool IsUnique { get; init; }
        public string? DefaultSql { get; init; }
        public Type? ForeignKeyRefType { get; init; }
        public string? ForeignKeyRefColumn { get; init; }
    }
}
