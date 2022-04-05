using Newtonsoft.Json;
using ProcessExplorer;
using ProcessExplorer.Processes;
using System.Collections.Concurrent;
using System.Diagnostics;
using ProcessExplorer.LocalCollector;
using ProcessExplorer.LocalCollector.Modules;
using ProcessExplorer.LocalCollector.Registrations;
using ConnectionInfo = ProcessExplorer.LocalCollector.Connections.ConnectionInfo;

namespace SuperRPC_POC
{
    public class InfoCollectorServiceObject : IInfoCollectorServiceObject
    {
        // private Communicator communicatorHelper = new Communicator();
        public IProcessInfoAggregator? InfoAggregator { get; set; }
        public bool IsInitalized { get; set; } = false;


        public static object? ChangedObject { get; set; }
        public ConcurrentBag<object> ProcessChanges { get; set; } = new ConcurrentBag<object>();
        private static readonly object locker = new object();

        public static void SetChanges(object newObject)
        {
            lock (locker)
            {
                ChangedObject = null;
                ChangedObject = newObject;
            }
        }

        public InfoCollectorServiceObject(IProcessInfoAggregator aggregator)
        {
            InfoAggregator = aggregator;
            InfoAggregator.SetComposePID(Process.GetCurrentProcess().Id);
            InfoAggregator.InitProcessExplorer();
            // InfoAggregator.SetProcessMonitorCommunicator(communicatorHelper);
            //CreateNewProcess();
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

        public void AddInfo(ProcessInfoCollectorData processInfo)
        {
            if (processInfo is not null)
                InfoAggregator?.AddInformation(processInfo.Id.ToString(), processInfo);
        }

        public ConnectionInfo? ConnectionStatusChanged(object conn)
        {
            try
            {
                var s = conn.ToString();
                var connectionDto = JsonConvert.DeserializeObject<ConnectionInfo>(s);
                if (connectionDto is not null)
                {
                    Console.WriteLine(connectionDto.Name + "'s connection is changed to " + connectionDto.Status);
                    SetChanges(conn);
                    return connectionDto;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error...." + ex.Message);
                return null;
            }
            return null;
        }

        public IEnumerable<ProcessInfoData>? GetProcs()
        {
            IsInitalized = true;
            return InfoAggregator?.ProcessMonitor?.Data.Processes;
        }

        public IEnumerable<KeyValuePair<string, ProcessInfoCollectorData>>? GetInfo()
        {
            return InfoAggregator?.Information;
        }

        public IEnumerable<ModuleInfo>? GetMods()
        {
            var infos = InfoAggregator?.Information?.Select(m => m.Value);
            return infos?.SelectMany(m => m?.Modules?.CurrentModules);
        }

        public IEnumerable<ConnectionInfo>? GetCons()
        {
            var infos = InfoAggregator?.Information?.Select(m => m.Value);
            return infos?.SelectMany(m => m?.Connections?.Connections);
        }

        public IEnumerable<RegistrationInfo>? GetRegs()
        {
            var infos = InfoAggregator?.Information?.Select(m => m.Value);
            return infos?.SelectMany(m => m?.Registrations?.Services);
        }

        public IEnumerable<KeyValuePair<string, string>>? GetEnvs()
        {
            var infos = InfoAggregator?.Information?.Select(m => m.Value);
            return infos?.SelectMany(m => m?.EnvironmentVariables?.EnvironmentVariables);
        }

    }
}
