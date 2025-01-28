using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;

namespace HarinezumiSama.Benchmarks.Common;

public static class BenchmarkHelper
{
    public static Type[] GetAllBenchmarkTypes<T>()
        where T : class
    {
        var candidateTypes = EnumerateCandidateBenchmarkTypes<T>().ToArray();

        var result = candidateTypes
            .SelectMany(type => TypeFilter.GetTypesWithRunnableBenchmarks([type], [], NullLogger.Instance).runnable)
            .ToArray();

        return result;
    }

    private static IEnumerable<Type> EnumerateCandidateBenchmarkTypes<T>()
    {
        var initialType = typeof(T);

        if (initialType is { IsClass: false, IsInterface: false })
        {
            throw new ArgumentException($"The type '{initialType.FullName}' is neither a class nor an interface.", nameof(T));
        }

        if (initialType is { IsAbstract: true, IsSealed: true })
        {
            throw new ArgumentException($"The type '{initialType.FullName}' is static.", nameof(T));
        }

        if (initialType is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false })
        {
            yield return initialType;
        }

        if (initialType.IsGenericTypeDefinition)
        {
            throw new NotImplementedException(); // ... yet
        }

        var types = initialType.Assembly
            .GetTypes()
            .Where(static type => type is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false })
            .ToArray();

        foreach (var type in types)
        {
            if (initialType.IsAssignableFrom(type))
            {
                yield return type;
            }
        }
    }
}