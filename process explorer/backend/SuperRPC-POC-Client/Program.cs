using LocalCollector.Connections;
using LocalCollector.Modules;
using LocalCollector.Registrations;
using Microsoft.Extensions.DependencyInjection;
using ProcessExplorer;
using ProcessExplorer.Entities.Connections;
using ProcessExplorer.Entities.EnvironmentVariables;
using ProcessExplorer.Entities.Registrations;
using ProcessExplorer.Processes.RPCCommunicator;
using SuperRPC_POC_Client;
using System.Net.WebSockets;
using System.Text;
using WExmapleProgram.Fakes;


var serviceProvider = new ServiceCollection()
            .AddSingleton<ICommunicator, Communicator>()
            .AddSingleton<IServiceObject, ServiceObject>()
            .BuildServiceProvider();


var foo = serviceProvider.GetService<IServiceObject>();

//var modules = ModuleMonitorDto.FromAssembly();

//var environmentVariables = EnvironmentMonitorDto.FromEnvironment();

//var serviceCollection = new ServiceCollection();
//serviceCollection.AddTransient(typeof(IFakeService), typeof(FakeService));
//var lista = new List<RegistrationDto>();
//foreach (var item in serviceCollection)
//{
//    lista.Add(RegistrationDto.FromProperties(item.ImplementationType.ToString(), item.Lifetime.ToString(), item.ServiceType.ToString()));
//}
//var registrations = RegistrationMonitorDto.FromCollection(lista);
//var connections = new ConnectionMonitor();

//using (ClientWebSocket client = new ClientWebSocket())
//{
//    Uri uri = new Uri("ws://localhost:5056/super-rpc");
//    var cts = new CancellationTokenSource();

//    var conn = new DummyConnectionInfo(client, uri);
//    connections.AddConnection(conn.Data);
//    var communicator = new Communicator();
//    var info = new InfoAggregator(environmentVariables, connections, registrations, modules, communicator);
//    await info.SendMessage("");

//    try
//    {
//        await client.ConnectAsync(uri, cts.Token);

//        //while (client.State == WebSocketState.Open)
//        //{
//        //    Console.WriteLine("Enter message:");
//        //    var message = Console.ReadLine();

//        //    if (message != null)
//        //    {
//        //        ArraySegment<byte> btosend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
//        //        await client.SendAsync(btosend, WebSocketMessageType.Text, true, cts.Token);
//        //        while (true)
//        //        {
//        //            var responseBuffer = new byte[1024];
//        //            var offset = 0;
//        //            var packet = 1024;

//        //            ArraySegment<byte> byteRecieved = new ArraySegment<byte>(responseBuffer, offset, packet);
//        //            WebSocketReceiveResult responde = await client.ReceiveAsync(byteRecieved, cts.Token);
//        //            var responseMessage = Encoding.UTF8.GetString(responseBuffer, offset, responde.Count);
//        //            Console.WriteLine(responseMessage);
//        //            if (responde.EndOfMessage)
//        //                break;
//        //        }
//        //    }
//        //}
//    }
//    catch (WebSocketException ex)
//    {

//    }
//}
