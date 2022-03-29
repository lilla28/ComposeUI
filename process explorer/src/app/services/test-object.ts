export class ProcessObject{
    constructor() {
        
    }

    public ConsoleLogProcesses(processes:any){
        console.log(processes);
    }

    public ConsoleLogCreatedProcess(createdProcess:any){
        console.log(createdProcess);
    }

    public ConsoleLogTerminatedProcess(terminatedProcess:any){
        console.log(terminatedProcess);
    }

    public ConsoleLogModifiedProcess(modifiedProcess:any){
        console.log(modifiedProcess);
    }
}