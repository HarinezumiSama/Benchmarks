namespace HarinezumiSama.Benchmarks.Omnifactotum.StringExtensions;

public class ToSecuredUIStringEmptyStringValueBenchmarks() : ToSecuredUIStringBenchmarksBase(0);

public class ToSecuredUIStringSingleCharValueBenchmarks() : ToSecuredUIStringBenchmarksBase(1);

public class ToSecuredUIStringExtraShortValueBenchmarks() : ToSecuredUIStringBenchmarksBase(50);

public class ToSecuredUIStringShortValueBenchmarks() : ToSecuredUIStringBenchmarksBase(250);

public class ToSecuredUIStringLongValueBenchmarks() : ToSecuredUIStringBenchmarksBase(4_000);

public class ToSecuredUIStringExtraLongValueBenchmarks() : ToSecuredUIStringBenchmarksBase(35_000);

public class ToSecuredUIStringHugeValueBenchmarks() : ToSecuredUIStringBenchmarksBase(1_000_000);