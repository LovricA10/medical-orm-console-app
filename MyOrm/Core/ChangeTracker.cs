using MyOrm.Metadata;
using MyOrm.Sql;
using Npgsql;

namespace MyOrm.Core
{
    internal sealed class ChangeTracker
    {
        private readonly List<TrackedEntity> _tracked = [];

        public void ClearTracking() => _tracked.Clear();

        public void Attach(object entity, EntityMap map)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(map);

            if (_tracked.Any(t => ReferenceEquals(t.Entity, entity)))
                return;

            var original = Snapshot(entity, map);

            _tracked.Add(new TrackedEntity
            {
                Entity = entity,
                Map = map,
                OriginalValues = original
            });
        }

        public int SaveChanges(NpgsqlConnection connection)
        {
            ArgumentNullException.ThrowIfNull(connection);

            var updates = 0;

            foreach (var te in _tracked.ToList())
            {
                var map = te.Map;

                var idValue = map.KeyProperty.GetValue(te.Entity);
                if (idValue is null)
                    continue;

                var changed = GetChangedColumns(te, map);
                if (changed.Count == 0)
                    continue;

                updates += ExecuteUpdate(connection, te, map, idValue, changed);
                RefreshSnapshot(te, changed);
            }

            return updates;
        }

        private static IDictionary<string, object?> Snapshot(object entity, EntityMap map)
        {
            var snapshot = new Dictionary<string, object?>(StringComparer.Ordinal);
            foreach (var c in map.Columns)
                snapshot[c.ColumnName] = c.Property.GetValue(entity);

            return snapshot;
        }

        private static IReadOnlyList<ColumnMap> GetChangedColumns(TrackedEntity tracked, EntityMap map)
        {
            var changed = new List<ColumnMap>();

            foreach (var c in map.Columns)
            {
                if (c.Property == map.KeyProperty)
                    continue;

                var current = c.Property.GetValue(tracked.Entity);
                tracked.OriginalValues.TryGetValue(c.ColumnName, out var original);

                if (!Equals(current, original))
                    changed.Add(c);
            }

            return changed;
        }

        private static int ExecuteUpdate(
            NpgsqlConnection connection,
            TrackedEntity tracked,
            EntityMap map,
            object idValue,
            IReadOnlyList<ColumnMap> changed)
        {
            var sql = UpdateBuilder.BuildUpdateChanged(map, changed);

            using var cmd = new NpgsqlCommand(sql, connection);

            for (var i = 0; i < changed.Count; i++)
            {
                var value = changed[i].Property.GetValue(tracked.Entity);
                cmd.Parameters.AddWithValue($"@p{i}", value ?? DBNull.Value);
            }

            cmd.Parameters.AddWithValue("@id", idValue);
            return cmd.ExecuteNonQuery();
        }

        private static void RefreshSnapshot(TrackedEntity tracked, IEnumerable<ColumnMap> changed)
        {
            foreach (var c in changed)
                tracked.OriginalValues[c.ColumnName] = c.Property.GetValue(tracked.Entity);
        }
    }
}
