using BenchmarkDotNet.Running;
using MorganStanley.ComposeUI.Logging.Benchmark;
using MSBenchmark;

//var summmaryMain = BenchmarkRunner.Run<Benchmarks>();
var summary = BenchmarkRunner.Run<Benchmarkie>();
//var summary2 = BenchmarkRunner.Run<Benchmarkie2>();
//var summary3 = BenchmarkRunner.Run<EntLibBenchmark>();
//var summaryOriginal = BenchmarkRunner.Run<OriginalBenchMark>();