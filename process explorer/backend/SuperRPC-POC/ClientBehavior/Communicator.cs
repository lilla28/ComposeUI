using LocalCollector.RPCCommunicator;
using ProcessExplorer.Processes;
using ProcessExplorer.Processes.RPCCommunicator;

namespace SuperRPC_POC
{
  public class Communicator : ICommunicator
  {
    public CommunicatorState? State { get; set; }

    public Communicator()
    {
      State = CommunicatorState.Opened;
    }

    public async Task Add(object message)
    {
      if (message is ProcessInfoDto)
      {
        try
        {
          ProcessInfoDto? converted = (ProcessInfoDto)message;
          Console.WriteLine(string.Format("{0} is CREATED", converted.PID));
          InfoCollectorServiceObject.SetChanges(converted);
        }
        catch
        {
          throw new Exception("Somethings went wrong OBJECT");
        }
      }
    }

    public async Task Update(object message)
    {
      if (message is ProcessInfoDto)
      {
        try
        {
          ProcessInfoDto? converted = (ProcessInfoDto)message;
          Console.WriteLine(string.Format("{0} is MODIFIED", converted.PID));
          InfoCollectorServiceObject.SetChanges(converted);
        }
        catch
        {
          throw new Exception("Somethings went wrong OBJECT");
        }
      }
    }

    public async Task Remove(object message)
    {
      try
      {
        int pid = Convert.ToInt32(message);
        Console.WriteLine(string.Format("{0} is TERMINATED", pid));
        InfoCollectorServiceObject.SetChanges(pid);
      }
      catch
      {
        throw new Exception("Somethings went wrong INT");
      }
    }
  }
}
