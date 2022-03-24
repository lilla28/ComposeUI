using System.Threading.Tasks;

namespace SuperRPC
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var webSocketService = new WebSocketService();
            await webSocketService.Start();
        }
    }
}