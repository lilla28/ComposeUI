using LocalCollector;
using LocalCollector.Registrations;
using Newtonsoft.Json;
using ProcessExplorer;
using ProcessExplorer.Entities.Connections;
using ProcessExplorer.Entities.Modules;
using ProcessExplorer.Processes;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace SuperRPC_POC
{
    public class InfoCollectorServiceObject : IInfoCollectorServiceObject
    {
        public IInfoCollector? InfoCollector { get; set; }
        private Communicator communicatorHelper = new Communicator();
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

        public InfoCollectorServiceObject(IInfoCollector collector)
        {
            InfoCollector = collector;
            InfoCollector.SetComposePID(Process.GetCurrentProcess().Id);
            InfoCollector.InitProcessExplorer();
            InfoCollector.SetProcessMonitorCommunicator(communicatorHelper);
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

        public void AddInfo(InfoAggregatorDto info)
        {
            if (info is not null)
                InfoCollector?.AddInformation(info.Id.ToString(), info);
        }

        public ConnectionDto? ConnectionStatusChanged(object conn)
        {
            try
            {
                var s = conn.ToString();
                var connectionDto = JsonConvert.DeserializeObject<ConnectionDto>(s);
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

        public IEnumerable<ProcessInfoDto>? GetProcs()
        {
            IsInitalized = true;
            return InfoCollector?.ProcessMonitor?.Data.Processes;
        }

        public IEnumerable<KeyValuePair<string, InfoAggregatorDto>>? GetInfo()
        {
            return InfoCollector?.Information;
        }

        public IEnumerable<ModuleDto>? GetMods()
        {
            var infos = InfoCollector?.Information?.Select(m => m.Value);
            return infos?.SelectMany(m => m?.Modules?.CurrentModules);
        }

        public IEnumerable<ConnectionDto>? GetCons()
        {
            var infos = InfoCollector?.Information?.Select(m => m.Value);
            return infos?.SelectMany(m => m?.Connections?.Connections);
        }

        public IEnumerable<RegistrationDto>? GetRegs()
        {
            var infos = InfoCollector?.Information?.Select(m => m.Value);
            return infos?.SelectMany(m => m?.Registrations?.Services);
        }

        public IEnumerable<KeyValuePair<string, string>>? GetEnvs()
        {
            var infos = InfoCollector?.Information?.Select(m => m.Value);
            return infos?.SelectMany(m => m?.EnvironmentVariables?.EnvironmentVariables);
        }

    }
}
