using System;
using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using LocalCollector;
using LocalCollector.Connections;
using LocalCollector.Modules;
using LocalCollector.Registrations;
using Microsoft.AspNetCore.Http;
using ProcessExplorer;
using ProcessExplorer.Entities.Connections;
using ProcessExplorer.Entities.EnvironmentVariables;
using ProcessExplorer.Entities.Modules;
using ProcessExplorer.Entities.Registrations;
using ProcessExplorer.Processes;
using SuperRPC_POC;

namespace SuperRPC;

public class SuperRpcWebSocketMiddlewareV2
{
    private readonly RequestDelegate next;
    private readonly SuperRPC rpc;
    private readonly RPCReceiveChannel receiveChannel;

    MyService service = new MyService();
    private readonly IInfoCollectorServiceObject process;

    public interface IService
    {
        Task<int> Add(int a, int b);
    }

    public class CustomDTO
    {
        public string Name { get; set; }
    }


    public SuperRpcWebSocketMiddlewareV2(RequestDelegate next, IInfoCollectorServiceObject collector, SuperRPC rpc)
    {
        this.next = next;
        this.rpc = rpc;
        process = collector;

        SuperRPCWebSocket.RegisterCustomDeserializer(rpc);

        receiveChannel = new RPCReceiveChannel();
        rpc.Connect(receiveChannel);

        // register host objects here
        rpc.RegisterHostObject("process", process, new ObjectDescriptor
        {
            Functions = new FunctionDescriptor[] { new FunctionDescriptor { Name = "AddInfo", Returns = FunctionReturnBehavior.Void}, 
                new FunctionDescriptor { Name = "ConnectionStatusChanged", Returns = FunctionReturnBehavior.Void }, 
                "GetProcs", "GetInfo", "GetMods", "GetCons", "GetRegs", "GetEnvs" },
            ProxiedProperties = new PropertyDescriptor[] { "InfoCollector", "communicatorHelper", "ChangedObject" }
        }
        );

        rpc.RegisterHostObject("service", service, new ObjectDescriptor
        {
            Functions = new FunctionDescriptor[] { "Add", "Increment" },
            ProxiedProperties = new PropertyDescriptor[] { "Counter" }
        });

        rpc.RegisterHostFunction("testDTO", (CustomDTO x) => Debug.WriteLine($"Custom DTO name: {x.Name}"));

        rpc.RegisterHostFunction("InfoAggregatorDto", (InfoAggregatorDto x) => x);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == "/super-rpc")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                {
                    await SuperRPCWebSocket.HandleWebsocketConnectionAsync(webSocket, receiveChannel);
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            return;
        }
        await next(context);
    }

}
