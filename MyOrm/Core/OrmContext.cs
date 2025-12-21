using MyOrm.Metadata;
using MyOrm.Sql;
using Npgsql;

namespace MyOrm.Core
{
    public sealed class OrmContext : IDisposable
    {
        private readonly NpgsqlConnection _connection;
        private readonly ChangeTracker _tracker = new();
        private readonly Dictionary<Type, EntityMap> _mapCache = [];

        private bool _disposed;

        public OrmContext(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string is required.", nameof(connectionString));

            _connection = new NpgsqlConnection(connectionString);
            _connection.Open();
        }

        public void Execute(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException("SQL is required.", nameof(sql));

            using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.ExecuteNonQuery();
        }

        public void ClearTracking() => _tracker.ClearTracking();

        public void Insert<T>(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var map = GetMap<T>();
            var sql = InsertBuilder.BuildInsert(map);

            using var cmd = new NpgsqlCommand(sql, _connection);

            var columns = map.Columns
                .Where(c => c.Property != map.KeyProperty || !map.KeyAutoIncrement)
                .ToList();

            AddParameters(cmd, columns, entity);
            cmd.ExecuteNonQuery();
        }

        public List<T> SelectAll<T>() where T : new()
        {
            var map = GetMap<T>();
            var sql = SelectBuilder.BuildSelectAll(map);

            using var cmd = new NpgsqlCommand(sql, _connection);
            using var reader = cmd.ExecuteReader();

            var result = new List<T>();

            while (reader.Read())
            {
                var obj = Materializer.Materialize<T>(map, reader);
                _tracker.Attach(obj!, map);
                result.Add(obj);
            }

            return result;
        }

        public T? GetById<T>(object id) where T : new()
        {
            ArgumentNullException.ThrowIfNull(id);

            var map = GetMap<T>();
            var sql = SelectBuilder.BuildSelectById(map);

            using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return default;

            var obj = Materializer.Materialize<T>(map, reader);
            _tracker.Attach(obj!, map);
            return obj;
        }

        public void Update<T>(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var map = GetMap<T>();
            var sql = UpdateBuilder.BuildUpdate(map);

            var idValue = map.KeyProperty.GetValue(entity)
                ?? throw new InvalidOperationException("Entity key is null.");

            using var cmd = new NpgsqlCommand(sql, _connection);

            var columns = map.Columns
                .Where(c => c.Property != map.KeyProperty)
                .ToList();

            AddParameters(cmd, columns, entity);
            cmd.Parameters.AddWithValue("@id", idValue);
            cmd.ExecuteNonQuery();
        }

        public void DeleteById<T>(object id)
        {
            ArgumentNullException.ThrowIfNull(id);

            var map = GetMap<T>();
            var sql = DeleteBuilder.BuildDeleteById(map);

            using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public List<T> SelectWhere<T>(string whereSql, params object[] args) where T : new()
        {
            if (string.IsNullOrWhiteSpace(whereSql))
                throw new ArgumentException("WHERE SQL is required.", nameof(whereSql));

            var map = GetMap<T>();
            var sql = SelectBuilder.BuildSelectWhere(map, whereSql);

            using var cmd = new NpgsqlCommand(sql, _connection);

            for (var i = 0; i < args.Length; i++)
                cmd.Parameters.AddWithValue($"@p{i}", args[i] ?? DBNull.Value);

            using var reader = cmd.ExecuteReader();

            var result = new List<T>();
            while (reader.Read())
            {
                var obj = Materializer.Materialize<T>(map, reader);
                _tracker.Attach(obj!, map);
                result.Add(obj);
            }

            return result;
        }

        public int SaveChanges() => _tracker.SaveChanges(_connection);

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _connection.Dispose();
        }

        private EntityMap GetMap<T>() => GetMap(typeof(T));

        private EntityMap GetMap(Type type)
        {
            if (_mapCache.TryGetValue(type, out var map))
                return map;

            map = ModelInspector.BuildEntityMap(type);
            _mapCache[type] = map;
            return map;
        }

        private static void AddParameters<T>(NpgsqlCommand cmd, IReadOnlyList<ColumnMap> columns, T entity)
        {
            for (var i = 0; i < columns.Count; i++)
            {
                var value = columns[i].Property.GetValue(entity);
                cmd.Parameters.AddWithValue($"@p{i}", value ?? DBNull.Value);
            }
        }
    }
}
