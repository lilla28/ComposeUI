using System.Collections.Concurrent;
using LocalCollector;
using LocalCollector.Registrations;
using ProcessExplorer;
using ProcessExplorer.Entities.Connections;
using ProcessExplorer.Entities.Modules;
using ProcessExplorer.Processes;

namespace SuperRPC_POC
{
    public interface IInfoCollectorServiceObject
    {
        IInfoCollector? InfoCollector { get; set; }
        bool IsInitalized { get; set; }
        ConcurrentBag<object> ProcessChanges { get; set; }

        void AddInfo(InfoAggregatorDto info);
        ConnectionDto? ConnectionStatusChanged(object connection);
        IEnumerable<ConnectionDto>? GetCons();
        IEnumerable<KeyValuePair<string, string>>? GetEnvs();
        IEnumerable<KeyValuePair<string, InfoAggregatorDto>>? GetInfo();
        IEnumerable<ModuleDto>? GetMods();
        IEnumerable<ProcessInfoDto>? GetProcs();
        IEnumerable<RegistrationDto>? GetRegs();
    }
}
