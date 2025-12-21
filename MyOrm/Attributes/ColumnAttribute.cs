using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOrm.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ColumnAttribute : Attribute
    {
        public string Name { get; }
        public DbType Type { get; }
        public int Length { get; set; } = 0;
        public bool IsNullable { get; set; } = true;
        public bool IsUnique { get; set; } = false;
        public string? DefaultSql { get; set; }

        public ColumnAttribute(string name, DbType type)
        {
            Name = name;
            Type = type;
        }
    }
}
