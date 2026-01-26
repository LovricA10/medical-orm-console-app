using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyOrm.Migrations
{
    public static class AutoMigrations
    {
        public static MigrationSql ApplyDeterministicName(MigrationSql migration, string prefix = "auto")
        {
            ArgumentNullException.ThrowIfNull(migration);
            if (migration.Up.Count == 0)
                return migration;

            var signature = string.Join("\n", migration.Up);

        var hash = Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(signature)))
            .Substring(0, 12)
            .ToLowerInvariant();

            return new MigrationSql
            {
                Name = $"{prefix}_{hash}",
                Up = migration.Up,
                Down = migration.Down
            };
        }
    }
}
