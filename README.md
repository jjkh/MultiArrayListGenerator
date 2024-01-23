# Jjkh.MultiArrayListGenerator

WIP C# source generator to convert a `[record] class/struct` to a [Structure of arrays](https://en.wikipedia.org/wiki/AoS_and_SoA#Structure_of_arrays), similar to [Zig's MultiArrayList](https://github.com/ziglang/zig/blob/master/lib/std/multi_array_list.zig).

Currently broken! Don't use!

## Benchmarks

| # of iterations |  .NET Framework (AoS) |  .NET Framework (SoA) |  Speedup |  .NET 8 (AoS) |  .NET 8 (SoA) |  Speedup |
| --------------- | --------------------: | --------------------: | -------: | ------------: | ------------: | -------: |
| 10,000          |               0.0053s |              0.00087s |    6.11x |        0.011s |        0.019s |    0.56x |
| 100,000         |               0.0085s |               0.0032s |    2.64x |        0.012s |        0.030s |    0.40x |
| 1,000,000       |                0.061s |                0.026s |    2.35x |        0.038s |        0.042s |    0.90x |
| 10,000,000      |                 0.57s |                 0.23s |    2.54x |         0.38s |         0.24s |    1.61x |
| 100,000,000     |                 4.53s |                 2.18s |    2.08x |         2.03s |         1.35s |    1.50x |
| 1,000,000,000   |              5:27.66s |              0:43.14s |    7.60x |      5:08.53s |      0:34.16s |    9.03x |

