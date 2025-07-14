namespace HarinezumiSama.Benchmarks.Omnifactotum.StringExtensions;

public class ToSecuredUIStringEmptyStringValueBenchmarks() : ToSecuredUIStringBenchmarks(0);

public class ToSecuredUIStringSingleCharValueBenchmarks() : ToSecuredUIStringBenchmarks(1);

// public class ToSecuredUIStringFewCharsValueBenchmarks() : ToSecuredUIStringBenchmarks(16);
//
// public class ToSecuredUIStringExtraShortValueBenchmarks() : ToSecuredUIStringBenchmarks(50);
//
// public class ToSecuredUIStringShortValueBenchmarks() : ToSecuredUIStringBenchmarks(250);
//
// public class ToSecuredUIStringLongValueBenchmarks() : ToSecuredUIStringBenchmarks(4_000);
//
// public class ToSecuredUIStringExtraLongValueBenchmarks() : ToSecuredUIStringBenchmarks(35_000);
//
// public class ToSecuredUIStringHugeValueBenchmarks() : ToSecuredUIStringBenchmarks(1_000_000);