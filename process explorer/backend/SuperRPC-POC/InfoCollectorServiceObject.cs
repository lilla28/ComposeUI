using LocalCollector;
using LocalCollector.Registrations;
using Newtonsoft.Json;
using ProcessExplorer;
using ProcessExplorer.Entities.Connections;
using ProcessExplorer.Entities.Modules;
using ProcessExplorer.Processes;
using System.Diagnostics;
using System.Text.Json;

namespace SuperRPC_POC
{
    public class InfoCollectorServiceObject : IInfoCollectorServiceObject
    {
        public IInfoCollector? InfoCollector { get; set; }
        public InfoCollectorServiceObject(IInfoCollector collector)
        {
            InfoCollector = collector;
            InfoCollector.SetComposePID(Process.GetCurrentProcess().Id);
            InfoCollector.InitProcessExplorer();

            CreateNewProcess();
        }

        private static void CreateNewProcess()
        {
            Thread.Sleep(10000);
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
            if (info is not null && info.Id is not null)
                InfoCollector?.AddInformation(info?.Id?.ToString(), info);
        }

        public void ConnectionStatusChanged(object conn)
        {
            try
            {
                var s = conn.ToString();
                var connectionDto = JsonConvert.DeserializeObject<ConnectionDto>(s);
                if (connectionDto is not null)
                {
                    Console.WriteLine(connectionDto.Name + "'s connection is changed to " + connectionDto.Status);
                    
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error...." + ex.Message);
            }
        }

        public IEnumerable<ProcessInfoDto>? GetProcs()
        {
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
