import { of } from "rxjs";
import { ProcessInfo } from "../master-view/processes/processes.component";

export class ServiceObject{
    public processes:ProcessInfo[];

    constructor() {
        this.GetProcesses();
    }

    public AddProcesses(processes: ProcessInfo[]){
        this.processes = processes;
        console.log("AddProcesses was called:", processes);
    }

    public AddProcess(process: any){
        console.log(process);
    }

    public UpdateProcess(process:ProcessInfo){
        let index = this.processes.findIndex(item => item.PID == process.PID);
        if(index != -1)
            this.processes[index] = process;
        console.log("Process has been modified: ", process);
    }

    public RemoveProcess(pid:number){
        console.log("PID: " + pid + " has been terminated");
    }

    public AddConnections(conns:any[]){
        console.log(conns);
    }

    public AddConnection(conn:any){
        console.log(conn);
    }

    public UpdateConnection(conn:any){
        console.log(conn);
    }

    public UpdateEnvironmentVariables(environmentVariables:any){
        console.log(environmentVariables);
    }

    public UpdateRegistrations(registrations:any){
        console.log(registrations);
    }

    public UpdateModules(modules:any){
        console.log(modules);
    }
    
    public AddRuntimeInfo(dataObject:any){
        console.log(dataObject);
    }

    public AddRuntimeInfos(dataObject:any[]){
        console.log(dataObject);
    }

    //
    public GetProcesses(){
        let obsof = of(this.processes);
        return obsof;
    }
}