using LocalCollector.RPCCommunicator;
using ProcessExplorer.Processes;
using ProcessExplorer.Processes.RPCCommunicator;

namespace SuperRPC_POC
{
    public class CommunicatorHelper : ICommunicator
    {
        public CommunicatorState? State { get; set; }

        public CommunicatorHelper()
        {
            State = CommunicatorState.Opened;
        }


        public async Task<object> SendMessage(object message)
        {
            try
            {
                if (message is ProcessInfoDto)
                {
                    try
                    {
                        ProcessInfoDto? converted = (ProcessInfoDto)message;
                        //Console.WriteLine(string.Format("{0} is MODIFIED", converted.PID));
                        InfoCollectorServiceObject.SetChanges(converted);
                        return converted;
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
                        //Console.WriteLine(string.Format("{0} is TERMINATED", pid));
                        InfoCollectorServiceObject.SetChanges(pid);
                        return pid;
                    }
                    catch
                    {
                        throw new Exception("Somethings went wrong INT");
                    }
                }
            }
            catch
            {
                throw new Exception("Exception was thrown just in case you do not know...");
            }
        }

    }
}
