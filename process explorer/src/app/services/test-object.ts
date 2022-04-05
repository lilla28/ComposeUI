export class ServiceObject{
    constructor() {
        
    }

    public AddProcesses(processes: any[]){
        console.log(processes);
    }

    public AddProcess(process: any){
        console.log(process);
    }

    public UpdateProcess(process:any){
        console.log(process);
    }

    public RemoveProcess(pid:number){
        console.log("PID: " + pid + " has been terminated");
    }

    

    public AddRegistrations(registrations : any[]){
        console.log(registrations);
    }

    public AddModules(modules:any[]){
        console.log(modules);
    }

    public AddConnections(conns:any[]){
        console.log(conns);
    }

    public AddConnection(conn:any){
        console.log(conn);
    }

    public AddEnvironmentVariables(environmentVariables:any){
        console.log(environmentVariables);
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


}