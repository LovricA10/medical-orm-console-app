using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOrm.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class KeyAttribute : Attribute
    {
        public bool AutoIncrement { get; set; } = true;
    }
}
