using ProcessExplorer.LocalCollector;
using ProcessExplorer.LocalCollector.Modules;
using ProcessExplorer.LocalCollector.Registrations;
using ProcessExplorer.Processes;
using ProcessExplorer.Processes.Communicator;
using ConnectionInfo = ProcessExplorer.LocalCollector.Connections.ConnectionInfo;

namespace SuperRPC_POC.ClientBehavior;

public class UIHandler : IUIHandler
{
    private Super.RPC.SuperRPC rpc;
    private IServiceObject serviceProxy;
    private readonly ILogger<UIHandler>? logger;

    public UIHandler(ILogger<UIHandler> logger = null)
    {
        this.logger = logger;
    }

    public async Task InitSuperRPC(Super.RPC.SuperRPC superRpc)
    {
        this.rpc = superRpc;
        await rpc.RequestRemoteDescriptors();
        serviceProxy = rpc.GetProxyObject<IServiceObject>("ServiceObject");
    }

    public async Task AddProcesses(IEnumerable<ProcessInfoData>? processes)
    {
        try
        {
            if(serviceProxy is not null)
                await serviceProxy.AddProcesses(processes);
        }
        catch (Exception exception)
        {
            logger?.LogError(exception.Message);
        }
    }

    public async Task AddProcess(ProcessInfoData process)
    {
        if (serviceProxy is not null)
            await serviceProxy.AddProcess(process);
    }

    public async Task UpdateProcess(ProcessInfoData process)
    {
        if(serviceProxy is not null)
            await serviceProxy.UpdateProcess(process);
    }

    public async Task RemoveProcess(int pid)
    {
        if (serviceProxy is not null)
            await serviceProxy.RemoveProcessByID(pid);
    }

    public Task AddRuntimeInfo(string assemblyId, ProcessInfoCollectorData dataObject)
    {
        if (serviceProxy is not null)
            return serviceProxy.AddRuntimeInfo(assemblyId, dataObject);
        return null;
    }

    public async Task AddConnections(string assemblyId, IEnumerable<ConnectionInfo> connections)
    {
        if(serviceProxy is not null)
            await serviceProxy.AddConnections(assemblyId, connections);
    }

    public async Task AddConnection(string assemblyId, ConnectionInfo connection)
    {
        if(serviceProxy is not null)
            await serviceProxy.AddConnection(assemblyId, connection);
    }

    public async Task UpdateConnection(string assemblyId, ConnectionInfo connection)
    {
        if (serviceProxy is not null)
            await serviceProxy.UpdateConnection(assemblyId, connection);
    }

    public async Task UpdateEnvironmentVariables(string assemblyId, IEnumerable<KeyValuePair<string, string>> environmentVariables)
    {
        if (serviceProxy is not null)
            await serviceProxy.UpdateEnvironmentVariables(assemblyId, environmentVariables);
    }

    public async Task UpdateRegistrations(string assemblyId, IEnumerable<RegistrationInfo> registrations)
    {
        if (serviceProxy is not null)
            await serviceProxy.UpdateRegistrations(assemblyId, registrations);
    }

    public async Task UpdateModules(string assemblyId, IEnumerable<ModuleInfo> modules)
    {
        if (serviceProxy is not null)
            await serviceProxy.UpdateModules(assemblyId, modules);
    }

    public async Task AddRuntimeInfos(IEnumerable<KeyValuePair<string, ProcessInfoCollectorData>> runtimeInfos)
    {
        if(serviceProxy is not null)
            await serviceProxy.AddRuntimeInfos(runtimeInfos);
    }

    public async Task RemoveProcessByID(int pid)
    {
        if(serviceProxy != null)
        {
            await serviceProxy.RemoveProcessByID(pid);
        }      
    }
}
