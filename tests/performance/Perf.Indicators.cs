using BenchmarkDotNet.Attributes;
using Internal.Tests;
using Skender.Stock.Indicators;

namespace Tests.Performance;

[MarkdownExporterAttribute.GitHub]
[MemoryDiagnoser]
public class IndicatorPerformance
{
    private static IEnumerable<Quote> h;
    private static IEnumerable<Quote> ho;
    private static List<Quote> hList;

    private static IEnumerable<Quote> GenerateQuotes()
    {
        var dt = DateTime.Now.AddYears(-1);

        for (int i = 0; i < 500000; i++)
        {
            yield return new Quote
            {
                Date = dt + TimeSpan.FromMinutes(i + 1),
                Close = i,
                High = i,
                Low = i,
                Open = i,
                Volume = i
            };
        }
    }

    private static IEnumerable<Quote> FromFile() => File.ReadLines("_common/data/btcusd15x69k.csv")
        .Select(Importer.QuoteFromCsv);

    [Benchmark]
    public object AllLines() => FromFile().ToList();

    [Benchmark]
    public object LastLines() => FromFile().Last();
    
    [Benchmark]
    public object GeneratedQuotes_GetRsiOrderedEfficientLast() => GenerateQuotes().OrderBy(q => q.Date).GetRsiEfficient().Last();

    [Benchmark]
    public object GeneratedQuotes_GetRsiEfficientLast() => GenerateQuotes().GetRsiEfficient().Last();

    [Benchmark]
    public object GeneratedQuotes_GetRsiOrderedEfficientToList() => GenerateQuotes().OrderBy(q => q.Date).GetRsiEfficient().ToList();

    [Benchmark]
    public object GeneratedQuotes_GetRsiEfficientToList() => GenerateQuotes().GetRsiEfficient().ToList();

    [Benchmark]
    public object GeneratedQuotes_GetRsiLast() => GenerateQuotes().GetRsi().Last();

    [Benchmark]
    public object GeneratedQuotes_GetRsiLast_Old() => GenerateQuotes()
        .GetRsiOld()
        .Last();

    [Benchmark]
    public object GeneratedQuotes_GetRsiList_Old() => GenerateQuotes()
        .GetRsiOld()
        .ToList();
    
    [Benchmark]
    public object FromFile_GetRsiOrderedEfficientLast() => FromFile().OrderBy(q => q.Date).GetRsiEfficient().Last();

    [Benchmark]
    public object FromFile_GetRsiEfficientLast() => FromFile().GetRsiEfficient().Last();

    [Benchmark]
    public object FromFile_GetRsiOrderedEfficientToList() => FromFile().OrderBy(q => q.Date).GetRsiEfficient().ToList();

    [Benchmark]
    public object FromFile_GetRsiEfficientToList() => FromFile().GetRsiEfficient().ToList();

    [Benchmark]
    public object FromFile_GetRsiFromFile_Old() => FromFile()
        .GetRsiOld();

    [Benchmark]
    public object FromFile_GetRsiLast() => FromFile()
        .GetRsi()
        .Last();

    [Benchmark]
    public object FromFile_GetRsiToList() => FromFile().ToList().GetRsi().ToList();

    [Benchmark]
    public object FromFile_ConvertLinesToList() => FromFile().ToList();
}
