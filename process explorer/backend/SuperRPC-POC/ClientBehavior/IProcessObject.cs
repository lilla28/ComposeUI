namespace SuperRPC_POC.ClientBehavior
{
    public interface IProcessObject
    {
        public void ConsoleLogProcessObject(object processes);
        public void ConsoleLogCreatedProcess(object createdProcess);
        public void ConsoleLogTerminatedProcess(object terminatedProcess);
        public void ConsoleLogModifiedProcess(object modifiedProcess);
    }
}
