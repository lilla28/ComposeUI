export class ProcessInfo{
    StartTime : string;
    ProcessorUsageTime: string;
    PhysicalMemoryUsageBit: number;
    ProcessName: string;
    PID: number;
    PriorityLevel: number;
    ProcessPriorityClass: string;
    Threads: Array<any>;
    VirtualMemorySize: number;
    ParentId: number;
    PrivateMemoryUsage: number;
    ProcessStatus: string;
    MemoryUsage: number;
    ProcessorUsage: number;
    Children: ProcessInfo[];
  }