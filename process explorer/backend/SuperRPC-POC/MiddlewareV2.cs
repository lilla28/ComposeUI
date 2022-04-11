using System.Net;
using System.Net.WebSockets;
using ProcessExplorer.LocalCollector.Communicator;
using ProcessExplorer.Processes.Communicator;
using SuperRPC_POC;
using SuperRPC_POC.ClientBehavior;

namespace SuperRPC;

public class SuperRpcWebSocketMiddlewareV2
{
    private readonly RequestDelegate next;
    private readonly IInfoCollectorServiceObject collector;

    public SuperRpcWebSocketMiddlewareV2(RequestDelegate next, IInfoCollectorServiceObject collector)
    {
        this.next = next;
        this.collector = collector;
    }

    private SuperRPC SetupRPC(RPCReceiveChannel channel, ICommunicator? communicator)
    {
        var rpc = new SuperRPC(() => Guid.NewGuid().ToString("N"));
        SuperRPCWebSocket.RegisterCustomDeserializer(rpc);
        rpc.Connect(channel);

        if (communicator is not null)
        {
            rpc.RegisterHostObject("communicator", communicator, new ObjectDescriptor
            {
                Functions = new FunctionDescriptor[]{ "AddRuntimeInfo", "AddRuntimeInfos", "AddConnectionCollection", "UpdateConnectionInformation", "UpdateEnvironmentVariableInformation", "UpdateRegistrationInformation", "UpdateModuleInformation" }
            });
        }

        return rpc;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == "/super-rpc")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                {
                    ///
                    CollectorHandler collectorHandler = null;
                    var uiHandler = new UIHandler();


                    ///
                    if (collector.InfoAggregator != null)
                    {
                        collectorHandler = new CollectorHandler(collector.InfoAggregator);
                    }


                    var rpcWebSocketHandler = SuperRPCWebSocket.CreateHandler(webSocket);

                    /// 
                    var rpc = SetupRPC(rpcWebSocketHandler.ReceiveChannel, collectorHandler);

                    ///
                    uiHandler.InitSuperRPC(rpc).ContinueWith(_ => collector.InfoAggregator.AddUIConnection(uiHandler));


                    await rpcWebSocketHandler.StartReceivingAsync(collector.InfoAggregator, uiHandler);
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            return;
        }
        else if(context.Request.Path == "/collector-rpc")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                {
                    CollectorHandler collectorHandler = null;

                    if (collector.InfoAggregator != null)
                    {
                        collectorHandler = new CollectorHandler(collector.InfoAggregator);
                    }

                    var rpcWebSocketHandler = SuperRPCWebSocket.CreateHandler(webSocket);
                    var rpc = SetupRPC(rpcWebSocketHandler.ReceiveChannel, collectorHandler);
                    await rpcWebSocketHandler.StartReceivingAsync();
                }
            }
            else
            {
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            }
            return;
        }
        await next(context);
    }


}
