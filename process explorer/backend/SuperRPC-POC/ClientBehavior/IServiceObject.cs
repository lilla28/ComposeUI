using ProcessExplorer.LocalCollector;
using ProcessExplorer.LocalCollector.Modules;
using ProcessExplorer.LocalCollector.Registrations;
using ProcessExplorer.Processes;
using ConnectionInfo = ProcessExplorer.LocalCollector.Connections.ConnectionInfo;

namespace SuperRPC_POC.ClientBehavior
{
    public interface IServiceObject
    {
        Task AddRuntimeInfo(ProcessInfoCollectorData dataObject);
        Task AddRegistrations(IEnumerable<RegistrationInfo> registrations);
        Task AddModules(IEnumerable<ModuleInfo> modules);
        Task AddConnections(IEnumerable<ConnectionInfo> connections);
        Task AddConnection(ConnectionInfo connection);
        Task AddEnvironmentVariables(IEnumerable<KeyValuePair<string,string>> environmentVariables);
        Task UpdateConnection(ConnectionInfo connection);
        Task UpdateEnvironmentVariables(IEnumerable<KeyValuePair<string,string>> environmentVariables);
        Task UpdateRegistrations(IEnumerable<RegistrationInfo> registrations);
        Task UpdateModules(IEnumerable<ModuleInfo> modules);

        Task AddProcesses(IEnumerable<ProcessInfoData>? processes);
        Task AddProccess(ProcessInfoData process);
        Task UpdateProcess(ProcessInfoData process);
        Task RemoveProcess(int pid);


    }
}
