// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using Npgsql;
using RedisVsPostgresBenchmark;
using StackExchange.Redis;

public class Program
{
    private static IDatabase _redisDb;
    private static NpgsqlConnection _postgresConn;

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Initializing database connections...");

        try
        {
            // Initialize Redis connection
            var redis = await ConnectionMultiplexer.ConnectAsync("localhost:6379");
            _redisDb = redis.GetDatabase();

            // Test Redis connection
            await _redisDb.PingAsync();
            Console.WriteLine("Redis connection established successfully");

            // Initialize PostgreSQL connection
            _postgresConn = new NpgsqlConnection("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres");
            await _postgresConn.OpenAsync();

            // Test PostgreSQL connection
            await using (var cmd = new NpgsqlCommand("SELECT 1", _postgresConn))
            {
                await cmd.ExecuteScalarAsync();
            }
            Console.WriteLine("PostgreSQL connection established successfully");

            // Run benchmarks
            Console.WriteLine("Starting benchmarks...");
            var summary = BenchmarkRunner.Run<RedisVsPostgresBenchmarks>();

            Console.WriteLine("Benchmarks completed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            // Clean up resources
            if (_postgresConn != null)
            {
                await _postgresConn.CloseAsync();
                await _postgresConn.DisposeAsync();
            }

            Console.WriteLine("Connections closed");
        }
    }
}