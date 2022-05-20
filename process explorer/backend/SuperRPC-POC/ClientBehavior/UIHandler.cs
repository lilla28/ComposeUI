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
            await serviceProxy.RemoveProcess(pid);
    }

    public Task AddRuntimeInfo(ProcessInfoCollectorData dataObject)
    {
        if (serviceProxy is not null)
            return serviceProxy.AddRuntimeInfo(dataObject);
        return null;
    }

    public async Task AddConnections(IEnumerable<ConnectionInfo> connections)
    {
        if(serviceProxy is not null)
            await serviceProxy.AddConnections(connections);
    }

    public async Task AddConnection(ConnectionInfo connection)
    {
        if(serviceProxy is not null)
            await serviceProxy.AddConnection(connection);
    }

    public async Task UpdateConnection(ConnectionInfo connection)
    {
        if (serviceProxy is not null)
            await serviceProxy.UpdateConnection(connection);
    }

    public async Task UpdateEnvironmentVariables(IEnumerable<KeyValuePair<string, string>> environmentVariables)
    {
        if (serviceProxy is not null)
            await serviceProxy.UpdateEnvironmentVariables(environmentVariables);
    }

    public async Task UpdateRegistrations(IEnumerable<RegistrationInfo> registrations)
    {
        if (serviceProxy is not null)
            await serviceProxy.UpdateRegistrations(registrations);
    }

    public async Task UpdateModules(IEnumerable<ModuleInfo> modules)
    {
        if (serviceProxy is not null)
            await serviceProxy.UpdateModules(modules);
    }

    public async Task AddRuntimeInfos(IEnumerable<ProcessInfoCollectorData> runtimeInfos)
    {
        if(serviceProxy is not null)
            await serviceProxy.AddRuntimeInfos(runtimeInfos);
    }
}
