using ProcessExplorer.LocalCollector;
using ProcessExplorer.LocalCollector.Modules;
using ProcessExplorer.LocalCollector.Registrations;
using ProcessExplorer.Processes;
using ProcessExplorer.Processes.Communicator;
using ConnectionInfo = ProcessExplorer.LocalCollector.Connections.ConnectionInfo;

namespace SuperRPC_POC.ClientBehavior;

public class UIHandler : IUIHandler
{
    private SuperRPC.SuperRPC rpc;
    private IServiceObject serviceProxy;
    private readonly ILogger<UIHandler>? logger;

    public UIHandler(ILogger<UIHandler> logger = null)
    {
        this.logger = logger;
    }

    public async Task InitSuperRPC(SuperRPC.SuperRPC superRpc)
    {
        this.rpc = superRpc;
        await rpc.RequestRemoteDescriptors();
        serviceProxy = rpc.GetProxyObject<IServiceObject>("ServiceObject");
        State = CommunicatorState.Opened;
    }

    public CommunicatorState State { get; set; }

    public Task AddProcesses(IEnumerable<ProcessInfoData>? processes)
    {
        try
        {
            if(serviceProxy is not null)
                return serviceProxy.AddProcesses(processes);
        }
        catch (Exception exception)
        {
            logger?.LogError(exception.Message);
        }
        return Task.CompletedTask;
    }

    public Task AddProcess(ProcessInfoData process)
    {
        return serviceProxy.AddProccess(process);
    }

    public Task UpdateProcess(ProcessInfoData process)
    {
        if(serviceProxy is not null)
            return serviceProxy.UpdateProcess(process);
        return Task.CompletedTask;
    }

    public Task RemoveProcess(int pid)
    {
        return serviceProxy.RemoveProcess(pid);
    }

    public Task AddRuntimeInfo(ProcessInfoCollectorData dataObject)
    {
        return serviceProxy.AddRuntimeInfo(dataObject);
    }

    public Task AddRegistrations(IEnumerable<RegistrationInfo> registrations)
    {
        return serviceProxy.AddRegistrations(registrations);
    }

    public Task AddModules(IEnumerable<ModuleInfo> modules)
    {
        return serviceProxy.AddModules(modules);
    }

    public Task AddConnections(IEnumerable<ConnectionInfo> connections)
    {
        return serviceProxy.AddConnections(connections);
    }

    public Task AddConnection(ConnectionInfo connection)
    {
        return serviceProxy.AddConnection(connection);
    }

    public Task AddEnvironmentVariables(IEnumerable<KeyValuePair<string, string>> environmentVariables)
    {
        return serviceProxy.AddEnvironmentVariables(environmentVariables);
    }

    public Task UpdateConnection(ConnectionInfo connection)
    {
        return serviceProxy.UpdateConnection(connection);
    }

    public Task UpdateEnvironmentVariables(IEnumerable<KeyValuePair<string, string>> environmentVariables)
    {
        return serviceProxy.UpdateEnvironmentVariables(environmentVariables);
    }

    public Task UpdateRegistrations(IEnumerable<RegistrationInfo> registrations)
    {
        return serviceProxy.UpdateRegistrations(registrations);
    }

    public Task UpdateModules(IEnumerable<ModuleInfo> modules)
    {
        return serviceProxy.UpdateModules(modules);
    }
}
