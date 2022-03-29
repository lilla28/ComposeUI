using LocalCollector;
using LocalCollector.RPCCommunicator;
using ProcessExplorer.Entities.Connections;
using ProcessExplorer.Processes.RPCCommunicator;
using SuperRPC;
using SuperRPC_POC;
using System.Net.WebSockets;

namespace SuperRPC_POC_Client
{
    public class Communicator : ICommunicator
    {
        private readonly Uri uri = new Uri("ws://localhost:5056/super-rpc");
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private ClientWebSocket client = new ClientWebSocket();

        private IInfoCollectorServiceObject process;

        private SuperRPCWebSocket? rpcWebsocket;
        private readonly SuperRPC.SuperRPC rpc = new SuperRPC.SuperRPC(() => Guid.NewGuid().ToString("N"));
        public Communicator()
        {

            try
            {
                client.ConnectAsync(uri, cts.Token).Wait();
                rpcWebsocket = SuperRPCWebSocket.CreateHandler(client);
                rpc.Connect(rpcWebsocket.ReceiveChannel);
                State = CommunicatorState.Opened;
                SuperRPCWebSocket.RegisterCustomDeserializer(rpc);

                ConnectAsync();
                Connected = RequestRemoteDescriptors().ContinueWith(t =>
                   process = rpc.GetProxyObject<IInfoCollectorServiceObject>("process"));
            }
            catch
            {
                State = CommunicatorState.Closed;
            }
        }

        private async Task ConnectAsync()
        {
            if (rpcWebsocket != null)
                await rpcWebsocket.StartReceivingAsync();
        }

        public CommunicatorState? State { get; set; }

        private async Task RequestRemoteDescriptors()
        {
            await rpc.RequestRemoteDescriptors();
        }

        private Task Connected;

        public async Task Add(object message)
        {
            await Connected;
            if (message is InfoAggregatorDto)
            {
                InfoAggregatorDto? converted = (InfoAggregatorDto)message;
                if (process is not null)
                    process.AddInfo(converted);
            }
        }

        public async Task Update(object message)
        {
            await Connected;
            if (message is ConnectionDto)
            {
                ConnectionDto conn = (ConnectionDto)message;
                if (process is not null && conn is not null)
                    process.ConnectionStatusChanged(conn);
            }
        }

        public async Task Remove(object message)
        {
            await Connected;
            Console.WriteLine(message.ToString());
        }
    }
}
