```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3624)
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET SDK 9.0.102
  [Host]     : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2


```
| Method               | Mean       | Error     | StdDev    | Median     | Allocated |
|--------------------- |-----------:|----------:|----------:|-----------:|----------:|
| Redis_Set            |   463.5 μs |  16.84 μs |  49.12 μs |   456.9 μs |     193 B |
| Redis_Get            |   413.0 μs |  15.58 μs |  44.71 μs |   411.4 μs |     201 B |
| Postgres_Set         | 2,629.9 μs | 161.19 μs | 443.98 μs | 2,447.0 μs |    1280 B |
| Postgres_Get         |   436.5 μs |  19.29 μs |  56.87 μs |   441.0 μs |    1011 B |
| PostgresUnlogged_Set |   450.6 μs |  20.44 μs |  60.28 μs |   449.0 μs |    1290 B |
| PostgresUnlogged_Get |   448.9 μs |  15.83 μs |  45.91 μs |   455.7 μs |    1033 B |
