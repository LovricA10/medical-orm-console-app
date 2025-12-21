using MyOrm.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOrm.Core
{
    internal sealed class TrackedEntity
    {
        public required object Entity { get; init; }
        public required EntityMap Map { get; init; }
        public required IDictionary<string, object?> OriginalValues { get; init; }
    }
}
