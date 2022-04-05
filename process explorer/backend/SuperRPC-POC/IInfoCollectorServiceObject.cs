using System.Collections.Concurrent;
using ProcessExplorer;
using ProcessExplorer.LocalCollector;
using ProcessExplorer.LocalCollector.Modules;
using ProcessExplorer.LocalCollector.Registrations;
using ProcessExplorer.Processes;
using ConnectionInfo = ProcessExplorer.LocalCollector.Connections.ConnectionInfo;

namespace SuperRPC_POC
{
    public interface IInfoCollectorServiceObject
    {
        IProcessInfoAggregator? InfoAggregator { get; set; }
        bool IsInitalized { get; set; }
        ConcurrentBag<object> ProcessChanges { get; set; }

        void AddInfo(ProcessInfoCollectorData processInfo);
        ConnectionInfo? ConnectionStatusChanged(object connection);
        IEnumerable<ConnectionInfo>? GetCons();
        IEnumerable<KeyValuePair<string, string>>? GetEnvs();
        IEnumerable<KeyValuePair<string, ProcessInfoCollectorData>>? GetInfo();
        IEnumerable<ModuleInfo>? GetMods();
        IEnumerable<ProcessInfoData>? GetProcs();
        IEnumerable<RegistrationInfo>? GetRegs();
    }
}
