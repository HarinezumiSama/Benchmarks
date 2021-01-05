using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

// Columns

[assembly: BaselineColumn]
[assembly: MeanColumn]
[assembly: MedianColumn]
[assembly: StdDevColumn]

// Exporters

[assembly: PlainExporter]
[assembly: JsonExporter(indentJson: true)]
[assembly: HtmlExporter]
[assembly: MarkdownExporter]

// Miscellaneous

[assembly: Orderer(SummaryOrderPolicy.FastestToSlowest)]