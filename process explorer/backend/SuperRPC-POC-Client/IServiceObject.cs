
using ProcessExplorer.LocalCollector;
using ProcessExplorer.LocalCollector.Connections;
using ProcessExplorer.LocalCollector.Modules;
using ProcessExplorer.LocalCollector.Registrations;

namespace SuperRPC_POC_Client
{
    public interface IServiceObject
    {
        IEnumerable<ConnectionInfo>? GetCons();
        IEnumerable<KeyValuePair<string, string>>? GetEnvs();
        IEnumerable<ModuleInfo>? GetMods();
        IEnumerable<RegistrationInfo>? GetRegs();
        IProcessInfoCollector? SetInfo();
    }
}
