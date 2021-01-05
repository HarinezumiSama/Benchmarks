# HarinezumiSama Benchmarks

Sets of benchmarks for .NET Framework, .NET Core, and .NET implemented using [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet).

The main goal of this project is to measure relative performance of different ways of producing the same result on different .NET implementations/versions.

For instance, the following two methods produce the same result, but using different approaches:
``` C#
[Benchmark(Baseline = true)]
public string ConcatenateValues()
    => "Timeout, ms: " + Timeout
        + ", URL: " + Url
        + ", Request ID: " + RequestId
        + ", Timestamp: " + Timestamp
        + ", Result: " + Result;

[Benchmark]
public string FormatValuesUsingStringFormatMethod()
    //// ReSharper disable once UseStringInterpolation
    => string.Format(
        @"Timeout, ms: {0}, URL: {1}, Request ID: {2}, Timestamp: {3}, Result: {4}",
        Timeout,
        Url,
        RequestId,
        Timestamp,
        Result);
```

#### Current benchmark sets:

Benchmark Set | Benchmarks | Description
:-------------|:-----------|:-----------
StringFormatting | <ul><li>StringFormattingUsingStringValuesBenchmarks</li><li>StringFormattingUsingVariousValuesBenchmarks</li></ul> | Compares various ways of building a string from string literals and variables.

## Build Status
[![Build status](https://ci.appveyor.com/api/projects/status/lssn1v7ssqqnjjkl?svg=true)](https://ci.appveyor.com/project/HarinezumiSama/Benchmarks)

## How to Build and Run

### Prerequisites
* Required Components
  * .NET Framework 4.8 SDK (Targeting pack) and Runtime
  * .NET Core 2.1 SDK and Runtime
  * .NET Core 3.1 SDK and Runtime
  * .NET 5 SDK and Runtime
* Optional Components
  * Windows PowerShell 5+ and/or PowerShell Core 6+

### Build Script Parameters
Parameter | Description
:---------|:-----------
`-Clean` | Delete the output directories before the build (if exist).
`-NoBenchmarking` | Do not run benchmarks.

### Building and Running on Windows

#### In *Windows PowerShell 5+* and *PowerShell Core 6+*
Run the following command:
``` PowerShell
& ./run.ps1 [-Clean] [-NoBenchmarking]
```

#### In *Windows Command Shell* (cmd.exe)
Run the following command:
``` bat
run.cmd [-Clean] [-NoBenchmarking]
```

**NOTE**: `run.cmd` invokes `run.ps1` using Windows PowerShell.

### Building and Running on Linux
**NOTE**: Not tested yet.

#### In *PowerShell Core 6+*
Run the following command:
``` PowerShell
& ./run.ps1 [-Clean] [-NoBenchmarking]
```
