using LocalCollector.RPCCommunicator;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Nerdbank.Streams;
using Newtonsoft.Json;
using ProcessExplorer.Processes;
using ProcessExplorer.Processes.RPCCommunicator;
using SuperRPC;
using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net;
using System.Net.WebSockets;

namespace SuperRPC_POC
{
    public class CommunicatorHelper : ICommunicator
    {
        private readonly Uri uri = new Uri("ws://localhost:5056/super-rpc");
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private ClientWebSocket client = new ClientWebSocket();
        private SuperRPCWebSocket? rpcWebsocket;
        private readonly SuperRPC.SuperRPC rpc = new SuperRPC.SuperRPC(() => Guid.NewGuid().ToString("N"));
        private IInfoCollectorServiceObject collector;

        public CommunicatorState? State { get; set; }

        public CommunicatorHelper()
        {
            State = CommunicatorState.Opened;
        }

        public async Task SendMessage(object message)
        {
            try
            {
                if(message is ProcessInfoDto)
                {
                    try
                    {
                        ProcessInfoDto? converted = (ProcessInfoDto)message;
                        Console.WriteLine(string.Format("{0} is MODIFIED", converted.PID));
                    }
                    catch
                    {
                        throw new Exception("Somethings went wrong OBJECT");
                    }
                }

                else
                {
                    try
                    {
                        int pid = Convert.ToInt32(message);
                        Console.WriteLine(string.Format("{0} is TERMINATED", pid));
                    }
                    catch
                    {
                        throw new Exception("Somethings went wrong INT");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception was thrown just in case you do not know...");
            }
        }
    }
}
