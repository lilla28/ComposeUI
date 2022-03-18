

using LocalCollector;
using LocalCollector.Connections;
using LocalCollector.Modules;
using LocalCollector.Registrations;
using Microsoft.Extensions.DependencyInjection;
using ProcessExplorer;
using ProcessExplorer.Entities.Connections;
using ProcessExplorer.Entities.EnvironmentVariables;
using ProcessExplorer.Entities.Modules;
using ProcessExplorer.Entities.Registrations;
using ProcessExplorer.Processes.RPCCommunicator;
using System.Net.WebSockets;
using WExmapleProgram.Fakes;

namespace SuperRPC_POC_Client
{
    public class ServiceObject : IServiceObject
    {
        private static IInfoAggregator info;
        private ICommunicator? communicator;
        public ServiceObject(ICommunicator communicator = null)
        {
            //MODULES
            var modules = ModuleMonitorDto.FromAssembly();

            //ENVIRONMENT VARIABLES
            var environmentVariables = EnvironmentMonitorDto.FromEnvironment();

            //REGISTRATIONS
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient(typeof(IFakeService), typeof(FakeService));
            var lista = new List<RegistrationDto>();
            foreach (var item in serviceCollection)
            {
                if (item is not null && item.ServiceType is not null && item.ImplementationType is not null)
                    lista.Add(RegistrationDto.FromProperties(item.ImplementationType.ToString(), item.Lifetime.ToString(), item.ServiceType.ToString()));
            }
            var registrations = RegistrationMonitorDto.FromCollection(lista);

            //CONNECTIONS
            var connections = new ConnectionMonitor();
            Uri uri = new Uri("ws://localhost:5056/super-rpc");
            var conn = new DummyConnectionInfo(new ClientWebSocket(), uri);
            connections.AddConnection(conn.Data);

            //COMMUNICATION
            communicator = communicator;

            info = new InfoAggregator(environmentVariables, connections, registrations, modules, communicator);
            info.SendMessage();
            Thread.Sleep(10000);
            conn.Data.Status = ConnectionStatus.Stopped.ToStringCached();
            connections.StatusChanged(conn.Data);
            Console.Read();
        }

        public IInfoAggregator? SetInfo()
        {
            return info;
        }

        public IEnumerable<ModuleDto>? GetMods()
        {
            return info.Data.Modules.CurrentModules;
        }

        public IEnumerable<ConnectionDto>? GetCons()
        {
            return info.Data.Connections.Connections;
        }

        public IEnumerable<RegistrationDto>? GetRegs()
        {
            return info.Data.Registrations.Services;
        }

        public IEnumerable<KeyValuePair<string, string>>? GetEnvs()
        {
            return info.Data.EnvironmentVariables.EnvironmentVariables;
        }
    }
}
