import { Subject } from "rxjs";
import { ProcessInfo } from "../master-view/processes/processes.component";

export class ServiceObject{
    public processes:ProcessInfo[];
    public subject: Subject<number>;
    private ComposePID: number;

    constructor() {
        
    }

    public AddProcesses(processes: ProcessInfo[]){
        this.processes = processes;
        // let ideiglenesLista = new Array<ProcessInfo>();

        // processes.forEach(element => {
        //     try{
        //         let findPPID = this.processes.findIndex(item => item.PID == element.ParentId)
        //         if(findPPID > -1){
        //             if(this.processes[findPPID].Children == undefined){
        //                 this.processes[findPPID].Children = new Array<ProcessInfo>();
        //             }
        //             this.processes[findPPID].Children.push(element);
        //         }
        //         else{
        //             //todo what happens when a process is under a children's children list and so ...
        //             if(element.ParentId != this.ComposePID){
        //                 //TODO nested list recursive?
        //                 ideiglenesLista.push(element);
        //                 this.processes.push(element);
        //             }
        //             //ha elso szintu akkor bele a listaba
        //             else{
        //                 this.processes.push(element);
        //             }
        //         }
        //     }
        //     catch(err){
        //         try{
        //             this.processes.push(element);
        //         }
        //         catch(err){
        //             this.processes = new Array<ProcessInfo>();
        //             this.processes.push(element);
        //         }
        //     }
        // });
        console.log("AddProcesses was called:", processes);
    }

    private OrganizeChildren(children: ProcessInfo[], process: ProcessInfo) : void{
        if(children.length != 0){
            children.forEach(element => {
                try{
                    let indexOfProcess = children.findIndex(item => item.PID == process.ParentId);
                    if(indexOfProcess > -1){
                        children[indexOfProcess].Children.push(process);
                    }
                    else{
                        this.CheckElementsChildrenList(element, process);
                    }
                }
                catch(err){
                    this.CheckElementsChildrenList(element, process);
                }
            });
        }
    }

    private CheckElementsChildrenList(element: ProcessInfo, process: ProcessInfo){
        if(element.Children != undefined && element.Children.length > 0){
            element.Children.forEach(ele => {
                this.OrganizeChildren(ele.Children, process); 
            });
        }
    }

    public AddProcess(process: any){
        console.log(process);
    }

    public UpdateProcess(process:ProcessInfo){
        var index = this.processes.findIndex(item => item.PID == process.PID);
        if(index != -1){
            this.processes[index] = process;
        }
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

    //NOT REGISTERED METHODS
    private SetSubject(pid: number){
        this.subject.next(pid);
    }

    public GetSubject(){
        return this.subject;
    }

    public GetProcesses(){
        return this.processes;
    }
}