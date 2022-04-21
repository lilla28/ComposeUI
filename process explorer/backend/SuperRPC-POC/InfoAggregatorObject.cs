
using ProcessExplorer;
using System.Diagnostics;
using ProcessExplorer.Processes.User;

namespace SuperRPC_POC
{
    public class InfoAggregatorObject : IInfoAggregatorObject
    {
        public IProcessInfoAggregator? InfoAggregator { get; set; }

        public InfoAggregatorObject(IProcessInfoAggregator aggregator)
        {
            InfoAggregator = aggregator;
            InfoAggregator.SetComposePID(Process.GetCurrentProcess().Id);
            InfoAggregator.InitProcessExplorer();

            var machine = MachineInfo.FromMachine();
            Console.WriteLine(machine.AvailableRAM);
            CreateNewProcess();
        }

        private static void CreateNewProcess()
        {
            Thread.Sleep(1000);
            Process pr = new Process();
            ProcessStartInfo prs = new ProcessStartInfo();
            prs.FileName = @"notepad.exe";
            pr.StartInfo = prs;

            ThreadStart ths = new ThreadStart(() => pr.Start());
            Thread th = new Thread(ths);
            th.Start();
        }
    }
}
