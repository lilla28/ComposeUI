import { Subject } from "rxjs";
import { ConnectionInfo } from "../DTOs/ConnectionInfo";
import { ModuleInfo } from "../DTOs/ModuleInfo";
import { ProcessInfo } from "../DTOs/ProcessInfo";
import { ProcessInfoCollectorData } from "../DTOs/ProcessInfoCollectorData";
import { RegistrationInfo } from "../DTOs/RegistrationInfo";

export class ServiceObject{
    public processes : ProcessInfo[];
    public runtimeInfoOfProcesses : Map<string,ProcessInfoCollectorData> = new Map<string, ProcessInfoCollectorData>();

    public subjectAddProcesses : Subject<ProcessInfo[]> = new Subject<ProcessInfo[]>();
    public subjectAddProcess : Subject<ProcessInfo> = new Subject<ProcessInfo>();
    public subjectUpdateProcess : Subject<ProcessInfo> = new Subject<ProcessInfo>();
    public subjectRemoveProcess : Subject<ProcessInfo> = new Subject<ProcessInfo>();
    public subjectAddConnections : Subject<ConnectionInfo[]> = new Subject<ConnectionInfo[]>();
    public subjectAddConenction : Subject<ConnectionInfo> = new Subject<ConnectionInfo>();
    public subjectUpdateConnection : Subject<ConnectionInfo> = new Subject<ConnectionInfo>();
    public subjectUpdateEnvironmentVariables : Subject<Map<string,string>> = new Subject<Map<string,string>>();
    public subjectUpdateModules :  Subject<ModuleInfo[]> = new Subject<ModuleInfo[]>();
    public subjectUpdateRegistrations : Subject<RegistrationInfo[]> = new Subject<RegistrationInfo[]>();
    public subjectAddRuntimeInfos : Subject<ProcessInfoCollectorData[]> = new Subject<ProcessInfoCollectorData[]>();
    public subjectAddRuntimeInfo : Subject<ProcessInfoCollectorData> = new Subject<ProcessInfoCollectorData>();

    constructor() {
        
    }

    public AddProcesses(processes : ProcessInfo[]){
        this.processes = processes;
        this.subjectAddProcesses.next(this.processes);
        console.log("AddProcesses was called:", processes);
    }

    public AddProcess(process : ProcessInfo){
        if(this.getIndexOfProcess(process.PID) == -1){
            this.processes.push(process);
            this.subjectAddProcess.next(process);
        }
        console.log("Process has been created: ", process);
    }

    public UpdateProcess(process : ProcessInfo){
        var index = this.getIndexOfProcess(process.PID);
        if(index != -1){
            this.processes[index] = process;
            this.subjectAddProcesses.next(this.processes);
            this.subjectUpdateProcess.next(process);
        }
        console.log("Process has been modified: ", process);
    }

    public RemoveProcess(pid : number){
        console.log("PID: " + pid + " has been terminated");
        var index = this.getIndexOfProcess(pid);
        if(index >= 0){
            this.subjectRemoveProcess.next(this.processes[index]);
            this.processes.splice(index, 1);
        }
    }

    public AddConnections(id : string, conns : ConnectionInfo[]){
        var element = this.runtimeInfoOfProcesses.get(id);
        this.changeElementInArray(conns, element, element?.Connections, () => {
            this.subjectAddConnections.next(conns); 
            console.log("Connections are updated:", conns); 
        });
    }

    public AddConnection(id : string, conn : ConnectionInfo){
        console.log(conn);
    }

    public UpdateConnection(id : string, conn : ConnectionInfo){
        var element = this.runtimeInfoOfProcesses.get(id);
        if(element != undefined){
            console.log("Connection is updated:", conn);
            element.Connections.map(item => {
                if(item.Id === conn.Id){
                    item = conn;
                }
            })
            this.subjectUpdateConnection.next(conn);
        }
    }

    public UpdateEnvironmentVariables(id : string, environmentVariables : Map<string,string>){
        var element = this.runtimeInfoOfProcesses.get(id);
        this.changeElementInArray(environmentVariables, element, element?.EnvironmentVariables, () => {
            this.subjectUpdateEnvironmentVariables.next(environmentVariables); 
            console.log("Environment variables are updated:", environmentVariables); 
        });
    }

    public UpdateRegistrations(id : string, registrations : RegistrationInfo[]){
        var element = this.runtimeInfoOfProcesses.get(id);
        this.changeElementInArray(registrations, element, element?.Registrations, () => {
            this.subjectUpdateRegistrations.next(registrations); 
            console.log("Registrations are updated:", registrations); 
        });
    }

    public UpdateModules(id : string, modules : ModuleInfo[]){
        var element = this.runtimeInfoOfProcesses.get(id);
        this.changeElementInArray(modules, element, element?.Modules, () => {
            this.subjectUpdateModules.next(modules); 
            console.log("Modules are updated:", modules); 
        });
    }
    
    public AddRuntimeInfo(id : string, runtimeInfoArray : ProcessInfoCollectorData){
        console.log("New runtime info arrived:", runtimeInfoArray);
        this.runtimeInfoOfProcesses.set(id, runtimeInfoArray);
        this.subjectAddRuntimeInfo.next(runtimeInfoArray);
    }

    public AddRuntimeInfos(runtimeInfoArray : Map<string, ProcessInfoCollectorData>){
        console.log("Runtimeinfo initalized. ", runtimeInfoArray);
        for(let i = 0; i < runtimeInfoArray.size; i++){
            this.runtimeInfoOfProcesses.set(
                Array.from(runtimeInfoArray)[i][0], Array.from(runtimeInfoArray)[i][1]
                );
        }
        this.subjectAddRuntimeInfos.next(Array.from(runtimeInfoArray.values()));
    }

    private getIndexOfProcess(pid : number) : number{
        return this.processes.findIndex(item => item.PID == pid);
    }

    private changeElementInArray(elementToChange : any, 
        possibleRuntimeInfo : any, 
        target: any, 
        callback: Function) : void{
        if (possibleRuntimeInfo != undefined 
            && typeof(possibleRuntimeInfo) === typeof(ProcessInfoCollectorData)){
            if(target != undefined){
                target = elementToChange;
                callback();
            }
        }
    }

    public GetProcesses(): ProcessInfo[]{
        return this.processes;
    }
}