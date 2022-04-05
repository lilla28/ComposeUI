using LocalCollector.Connections;
using Microsoft.Extensions.DependencyInjection;
using System.Net.WebSockets;
using ProcessExplorer.LocalCollector;
using ProcessExplorer.LocalCollector.Communicator;
using ProcessExplorer.LocalCollector.Connections;
using ProcessExplorer.LocalCollector.EnvironmentVariables;
using ProcessExplorer.LocalCollector.Modules;
using ProcessExplorer.LocalCollector.Registrations;
using WExmapleProgram.Fakes;

namespace SuperRPC_POC_Client
{
    public class ServiceObject : IServiceObject
    {
        private static IProcessInfoCollector processInfo;
        private ICommunicator? communicator;
        public ServiceObject(Communicator communicatorHelper = null)
        {
            //MODULES
            var modules = ModuleMonitorInfo.FromAssembly();

            //ENVIRONMENT VARIABLES
            var environmentVariables = EnvironmentMonitorInfo.FromEnvironment();

            //REGISTRATIONS
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient(typeof(IFakeService), typeof(FakeService));
            var lista = new List<RegistrationInfo>();
            foreach (var item in serviceCollection)
            {
                if (item is not null && item.ServiceType is not null && item.ImplementationType is not null)
                    lista.Add(RegistrationInfo.FromProperties(item.ImplementationType.ToString(), item.Lifetime.ToString(), item.ServiceType.ToString()));
            }
            var registrations = RegistrationMonitorInfo.FromCollection(lista);

            //CONNECTIONS
            var connections = new ConnectionMonitor();
            Uri uri = new Uri("ws://localhost:5056/super-rpc");
            var conn = new DummyConnectionInfo(new ClientWebSocket(), uri);
            connections.AddConnection(conn.Data);

            //COMMUNICATION
            this.communicator = communicatorHelper.GetCommunicatorObject().Result;

            //COLLECTOR INSTANTIATION
            processInfo = new ProcessInfoCollector(environmentVariables, connections, registrations, modules, communicator);


            processInfo.SendRuntimeInfo();
            Thread.Sleep(10000);
            conn.Data.Status = ConnectionStatus.Stopped.ToStringCached();
            connections.StatusChanged(conn.Data);
            Console.Read();
        }

        public IProcessInfoCollector? SetInfo()
        {
            return processInfo;
        }

        public IEnumerable<ModuleInfo>? GetMods()
        {
            return processInfo.Data.Modules.CurrentModules;
        }

        public IEnumerable<ConnectionInfo>? GetCons()
        {
            return processInfo.Data.Connections.Connections;
        }

        public IEnumerable<RegistrationInfo>? GetRegs()
        {
            return processInfo.Data.Registrations.Services;
        }

        public IEnumerable<KeyValuePair<string, string>>? GetEnvs()
        {
            return processInfo.Data.EnvironmentVariables.EnvironmentVariables;
        }
    }
}
