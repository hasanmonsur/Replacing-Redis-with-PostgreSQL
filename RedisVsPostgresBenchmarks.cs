using BenchmarkDotNet.Attributes;
using Npgsql;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisVsPostgresBenchmark
{
    [MemoryDiagnoser]
    public class RedisVsPostgresBenchmarks
    {
        private IDatabase _redisDb;
        private NpgsqlConnection _postgresConn;
        private const string Key = "test_key";
        private const string Value = "test_value";

        [GlobalSetup]
        public void Setup()
        {
            // Redis setup
            var redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
            _redisDb = redis.GetDatabase();

            // PostgreSQL setup
            _postgresConn = new NpgsqlConnection("Host=127.0.0.1:5432;Database=postgres;Username=postgres;Password=postgres");
            _postgresConn.Open();

            // Create a simple key-value table
            using var cmd = new NpgsqlCommand(
                "CREATE TABLE IF NOT EXISTS kv_store (key TEXT PRIMARY KEY, value TEXT)",
                _postgresConn);
            cmd.ExecuteNonQuery();

            // Create an unlogged table for faster performance (similar to Redis)
            using var cmdUnlogged = new NpgsqlCommand(
                "CREATE UNLOGGED TABLE IF NOT EXISTS kv_store_unlogged (key TEXT PRIMARY KEY, value TEXT)",
                _postgresConn);
            cmdUnlogged.ExecuteNonQuery();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _postgresConn?.Dispose();
        }

        [Benchmark]
        public void Redis_Set()
        {
            _redisDb.StringSet(Key, Value);
        }

        [Benchmark]
        public void Redis_Get()
        {
            _redisDb.StringGet(Key);
        }

        [Benchmark]
        public void Postgres_Set()
        {
            using var cmd = new NpgsqlCommand(
                "INSERT INTO kv_store (key, value) VALUES (@key, @value) " +
                "ON CONFLICT (key) DO UPDATE SET value = @value",
                _postgresConn);
            cmd.Parameters.AddWithValue("key", Key);
            cmd.Parameters.AddWithValue("value", Value);
            cmd.ExecuteNonQuery();
        }

        [Benchmark]
        public void Postgres_Get()
        {
            using var cmd = new NpgsqlCommand(
                "SELECT value FROM kv_store WHERE key = @key",
                _postgresConn);
            cmd.Parameters.AddWithValue("key", Key);
            cmd.ExecuteScalar();
        }

        [Benchmark]
        public void PostgresUnlogged_Set()
        {
            using var cmd = new NpgsqlCommand(
                "INSERT INTO kv_store_unlogged (key, value) VALUES (@key, @value) " +
                "ON CONFLICT (key) DO UPDATE SET value = @value",
                _postgresConn);
            cmd.Parameters.AddWithValue("key", Key);
            cmd.Parameters.AddWithValue("value", Value);
            cmd.ExecuteNonQuery();
        }

        [Benchmark]
        public void PostgresUnlogged_Get()
        {
            using var cmd = new NpgsqlCommand(
                "SELECT value FROM kv_store_unlogged WHERE key = @key",
                _postgresConn);
            cmd.Parameters.AddWithValue("key", Key);
            cmd.ExecuteScalar();
        }
    }
}
