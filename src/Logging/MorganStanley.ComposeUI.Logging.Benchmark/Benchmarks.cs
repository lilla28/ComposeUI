using BenchmarkDotNet.Attributes;
using MSBenchmark;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorganStanley.ComposeUI.Logging.Benchmark
{
    [MemoryDiagnoser]
    public class Benchmarks
    {
        private Benchmarkie benchmarkie = new Benchmarkie();
        private Benchmarkie2 benchmarkie2 = new Benchmarkie2();
        private EntLibBenchmark entLibBenchmark = new EntLibBenchmark();

        [Benchmark]
        public void SeriLogWithoutJson()
        {
            benchmarkie.LogWithoutJSONMainBenchmark();
        }

        [Benchmark]
        public void SeriLogWithJSON()
        {
            benchmarkie2.LogWithJSONBenchmark();
        }

        //[Benchmark]
        //public void EntLibWithoutJSON()
        //{
        //    entLibBenchmark.LogWithEntLibProvider();
        //}
    }
}
