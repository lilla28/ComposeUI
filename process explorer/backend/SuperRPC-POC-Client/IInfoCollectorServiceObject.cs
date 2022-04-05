using ProcessExplorer;
using ProcessExplorer.LocalCollector;
using ProcessExplorer.LocalCollector.Connections;
using ProcessExplorer.LocalCollector.Modules;
using ProcessExplorer.LocalCollector.Registrations;
using ProcessExplorer.Processes;

namespace SuperRPC_POC
{
    public interface IInfoCollectorServiceObject
    {
        IProcessInfoAggregator? InfoCollector { get; set; }

        object? AddInfo(ProcessInfoCollectorData processInfo);
        object? ConnectionStatusChanged(object connection);
        IEnumerable<ConnectionInfo>? GetCons();
        IEnumerable<KeyValuePair<string, string>>? GetEnvs();
        IEnumerable<KeyValuePair<string, ProcessInfoCollectorData>>? GetInfo();
        IEnumerable<ModuleInfo>? GetMods();
        IEnumerable<ProcessInfoData>? GetProcs();
        IEnumerable<RegistrationInfo>? GetRegs();
    }
}
