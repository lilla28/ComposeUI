using System.Net;
using System.Net.WebSockets;
using SuperRPC_POC;
using SuperRPC_POC.ClientBehavior;

namespace SuperRPC;

public class SuperRpcWebSocketMiddlewareV2
{
    private readonly RequestDelegate next;
    private readonly SuperRPC rpc;
    private readonly RPCReceiveChannel receiveChannel;

    MyService service = new MyService();
    private readonly IInfoCollectorServiceObject process;

    public IProcessObject ProcessObject;

    public interface IService
    {
        Task<int> Add(int a, int b);
    }

    public class CustomDTO
    {
        public string Name { get; set; }
    }


    public SuperRpcWebSocketMiddlewareV2(RequestDelegate next, IInfoCollectorServiceObject collector) //, IInfoCollectorServiceObject collector, SuperRPC rpc
    {
        this.next = next;
        //this.rpc = rpc;
        process = collector;        
    }

    private void SetupRPC(RPCReceiveChannel channel)
    {
        var rpc = new SuperRPC(() => Guid.NewGuid().ToString("N"));
        SuperRPCWebSocket.RegisterCustomDeserializer(rpc);
        rpc.Connect(channel);

        // register host objects here
        rpc.RegisterHostObject("process", process, new ObjectDescriptor
        {
            Functions = new FunctionDescriptor[] { new FunctionDescriptor { Name = "AddInfo", Returns = FunctionReturnBehavior.Void},
                new FunctionDescriptor { Name = "ConnectionStatusChanged", Returns = FunctionReturnBehavior.Void },
                "GetProcs", "GetInfo", "GetMods", "GetCons", "GetRegs", "GetEnvs" },
            ProxiedProperties = new PropertyDescriptor[] { "InfoCollector", "communicatorHelper", "ChangedObject" }
        }
       );
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == "/super-rpc")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                {
                    var rpcWebSocketHandler = SuperRPCWebSocket.CreateHandler(webSocket);
                    SetupRPC(rpcWebSocketHandler.ReceiveChannel);
                    await rpcWebSocketHandler.StartReceivingAsync();
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
