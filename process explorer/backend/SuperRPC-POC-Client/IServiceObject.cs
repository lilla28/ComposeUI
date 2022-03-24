using LocalCollector.Registrations;
using ProcessExplorer;
using ProcessExplorer.Entities.Connections;
using ProcessExplorer.Entities.Modules;

namespace SuperRPC_POC_Client
{
    public interface IServiceObject
    {
        IEnumerable<ConnectionDto>? GetCons();
        IEnumerable<KeyValuePair<string, string>>? GetEnvs();
        IEnumerable<ModuleDto>? GetMods();
        IEnumerable<RegistrationDto>? GetRegs();
        IInfoAggregator? SetInfo();
    }
}