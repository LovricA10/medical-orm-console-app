using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyOrm.Metadata
{
    public sealed class EntityMap
    {
        public required Type ClrType { get; init; }
        public required string TableName { get; init; }
        public required PropertyInfo KeyProperty { get; init; }
        public required bool KeyAutoIncrement { get; init; }
        public required IReadOnlyList<ColumnMap> Columns { get; init; }
    }
}
