namespace HarinezumiSama.Benchmarks.Omnifactotum.StringExtensions;

public class ToUIStringEmptyStringValueBenchmarks() : ToUIStringBenchmarksBase(0);

public class ToUIStringSingleCharValueBenchmarks() : ToUIStringBenchmarksBase(1);

public class ToUIStringExtraShortValueBenchmarks() : ToUIStringBenchmarksBase(50);

public class ToUIStringShortValueBenchmarks() : ToUIStringBenchmarksBase(250);

public class ToUIStringLongValueBenchmarks() : ToUIStringBenchmarksBase(4_000);

public class ToUIStringExtraLongValueBenchmarks() : ToUIStringBenchmarksBase(35_000);

public class ToUIStringHugeValueBenchmarks() : ToUIStringBenchmarksBase(1_000_000);