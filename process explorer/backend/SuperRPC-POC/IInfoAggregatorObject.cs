using ProcessExplorer;

namespace SuperRPC_POC
{
    public interface IInfoAggregatorObject
    {
        IProcessInfoAggregator? InfoAggregator { get; set; }
    }
}
