using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOrm.Attributes
{

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ForeignKeyAttribute : Attribute
    {
        public Type ReferenceType { get; }
        public string ReferenceColumn { get; }

        public ForeignKeyAttribute(Type referenceType, string referenceColumn = "id")
        {
            ReferenceType = referenceType;
            ReferenceColumn = referenceColumn;
        }
    }
}
