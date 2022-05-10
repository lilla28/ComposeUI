﻿using LocalCollector.Connections;
using Microsoft.Extensions.DependencyInjection;
using System.Net.WebSockets;
using ProcessExplorer.LocalCollector;
using ProcessExplorer.LocalCollector.Communicator;
using ProcessExplorer.LocalCollector.Connections;
using ProcessExplorer.LocalCollector.Modules;
using ProcessExplorer.LocalCollector.Registrations;
using WExmapleProgram.Fakes;
using System.Reflection;
using System.Diagnostics;

namespace SuperRPC_POC_Client
{
    public class ServiceObject : IServiceObject
    {
        private static IProcessInfoCollector processInfo;
        private ICommunicator? communicator;
        public ServiceObject(Communicator communicatorHelper = null)
        {
            //REGISTRATIONS
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient(typeof(IFakeService), typeof(FakeService));
            var lista = new List<RegistrationInfo>();
            foreach (var item in serviceCollection)
            {
                if (item is not null && item.ServiceType is not null && item.ImplementationType is not null)
                    lista.Add(new RegistrationInfo() {ImplementationType = item.ImplementationType.ToString(), ServiceType = item.ServiceType.ToString(), LifeTime = item.Lifetime.ToString() });
            }
            var registrations = RegistrationMonitorInfo.FromCollection(lista);

            //CONNECTIONS
            var connections = new ConnectionMonitor();
            Uri uri = new Uri("ws://localhost:5056/collector-rpc");
            var conn = new DummyConnectionInfo(new ClientWebSocket(), uri);
            connections.AddConnection(conn.Data);

            //COMMUNICATION
            this.communicator = communicatorHelper.GetCommunicatorObject().Result;

            //COLLECTOR INSTANTIATION
            processInfo = new ProcessInfoCollector(connections, registrations, communicator);
            processInfo.SetAssemblyID(Assembly.GetExecutingAssembly().GetName().Name);
            processInfo.SetClientPID(Process.GetCurrentProcess().Id);

            //SEND INFO
            Thread.Sleep(10000);
            processInfo.SendRuntimeInfo();
            Thread.Sleep(15000);
            connections.UpdateConnection((Guid)conn.Data.Id, ConnectionStatus.Stopped);
            
        }
    }
}