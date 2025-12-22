using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOrm.Migrations
{
    public sealed class MigrationSql
    {
        public required string Name { get; init; }
        public required IReadOnlyList<string> Up { get; init; }
        public required IReadOnlyList<string> Down { get; init; }
    }
}
