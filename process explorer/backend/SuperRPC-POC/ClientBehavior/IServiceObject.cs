using ProcessExplorer.LocalCollector;
using ProcessExplorer.LocalCollector.Modules;
using ProcessExplorer.LocalCollector.Registrations;
using ProcessExplorer.Processes;
using ConnectionInfo = ProcessExplorer.LocalCollector.Connections.ConnectionInfo;

namespace SuperRPC_POC.ClientBehavior
{
    public interface IServiceObject
    {
        Task AddRuntimeInfo(string assemblyId, ProcessInfoCollectorData dataObject);
        Task AddRuntimeInfos(IEnumerable<KeyValuePair<string, ProcessInfoCollectorData>> runtimeInfos);
        Task AddConnections(string assemblyId, IEnumerable<ConnectionInfo> connections);
        Task AddConnection(string assemblyId, ConnectionInfo connection);
        Task UpdateConnection(string assemblyId, ConnectionInfo connection);
        Task UpdateEnvironmentVariables(string assemblyId, IEnumerable<KeyValuePair<string,string>> environmentVariables);
        Task UpdateRegistrations(string assemblyId, IEnumerable<RegistrationInfo> registrations);
        Task UpdateModules(string assemblyId, IEnumerable<ModuleInfo> modules);
        Task AddProcesses(IEnumerable<ProcessInfoData>? processes);
        Task AddProcess(ProcessInfoData process);
        Task UpdateProcess(ProcessInfoData process);
        Task RemoveProcessByID(int pid);
    }
}
