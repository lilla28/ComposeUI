
using SuperRPC;
using System.Net.WebSockets;
using ProcessExplorer.LocalCollector.Communicator;

namespace SuperRPC_POC_Client
{
    public class Communicator
    {
        private readonly Uri uri = new Uri("ws://localhost:5056/super-rpc");
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private readonly ClientWebSocket client = new ClientWebSocket();


        private ICommunicator communicator;


        private SuperRPCWebSocket? rpcWebsocket;
        private readonly SuperRPC.SuperRPC rpc = new SuperRPC.SuperRPC(() => Guid.NewGuid().ToString("N"));
        private readonly Task Connected;

        public Communicator()
        {

            try
            {
                client.ConnectAsync(uri, cts.Token).Wait();
                rpcWebsocket = SuperRPCWebSocket.CreateHandler(client);
                rpc.Connect(rpcWebsocket.ReceiveChannel);
                SuperRPCWebSocket.RegisterCustomDeserializer(rpc);

                ConnectAsync();
                Connected = RequestRemoteDescriptors()
                    .ContinueWith(t => communicator = rpc.GetProxyObject<ICommunicator>("communicator"));                    
            }
            catch(Exception ex)
            {
                throw new Exception("Error " + ex.Message);
            }
        }

        private async Task ConnectAsync()
        {
            if (rpcWebsocket != null)
                await rpcWebsocket.StartReceivingAsync();
        }

        private async Task RequestRemoteDescriptors()
        {
            await rpc.RequestRemoteDescriptors();
        }

        public async Task GetCommunicatorForwardersCommunicator()
        {
            await Connected;
        }

        internal async Task<ICommunicator> GetCommunicatorObject()
        {
            await GetCommunicatorForwardersCommunicator();
            return this.communicator;
        }

        internal ICommunicator GetCom()
            => this.communicator;

    }
}
