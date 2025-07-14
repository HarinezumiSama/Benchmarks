namespace HarinezumiSama.Benchmarks.Omnifactotum.StringExtensions;

public class ToUIStringEmptyStringValueBenchmarks() : ToUIStringBenchmarks(0);

public class ToUIStringSingleCharValueBenchmarks() : ToUIStringBenchmarks(1);

// public class ToUIStringFewCharsValueBenchmarks() : ToUIStringBenchmarks(16);
//
// public class ToUIStringExtraShortValueBenchmarks() : ToUIStringBenchmarks(50);
//
// public class ToUIStringShortValueBenchmarks() : ToUIStringBenchmarks(250);
//
// public class ToUIStringLongValueBenchmarks() : ToUIStringBenchmarks(4_000);
//
// public class ToUIStringExtraLongValueBenchmarks() : ToUIStringBenchmarks(35_000);
//
// public class ToUIStringHugeValueBenchmarks() : ToUIStringBenchmarks(1_000_000);