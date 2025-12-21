using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOrm.Attributes
{
    public enum DbType
    {
        Int,
        Decimal,
        Float,
        Varchar,
        Char,
        Text,
        TimestampWithTimezone,
        TimestampWithoutTimezone
    }
}
