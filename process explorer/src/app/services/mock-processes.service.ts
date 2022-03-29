/* Morgan Stanley makes this available to you under the Apache License, Version 2.0 (the "License"). You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. See the NOTICE file distributed with this work for additional information regarding copyright ownership. Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License. */
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { MockProcesses } from './mock-processes';
import { SuperRPC } from 'super-rpc';
import { ProcessObject } from './test-object';

@Injectable({
  providedIn: 'root'
})
export class MockProcessesService {

  private ws: WebSocket = new WebSocket('ws://localhost:5056/super-rpc');
  private rpc : SuperRPC;
  private process : any;
  private connected: any;

  private async requestRemoteDescriptors() {
    await this.rpc.requestRemoteDescriptors()
  }

  constructor(){
    this.connected = new Promise( (resolve, reject) => 
    {
      try{
        this.ws.addEventListener('open', async() => {
          this.rpc = new SuperRPC( () => (Math.random()*1e17).toString(36));
          this.rpc.connect({
            sendAsync: (message) => this.ws.ssend(JSON.stringify(message)),
            receive: (callback) => { this.ws.addEventListener('message', (msg) => callback(JSON.parse(msg.data)))}
          });
        await this.requestRemoteDescriptors();
        this.process = this.rpc.getProxyObject('process');
        this.rpc.registerHostClass('ProcessObject', ProcessObject, {
          ctor: {},
          instance: {
            functions:[{name: 'ConsoleLogProcesses', returns: 'void'}, {name: 'ConsoleLogCreatedProcess', returns: 'void'},
            {name: 'ConsoleLogTerminatedProcess', returns: 'void'}, {name: 'ConsoleLogModifiedProcess', returns: 'void'}]
          }
        });
        resolve(undefined);
      })}catch(ex){
        reject(ex);}
    });
  }

  public async getProcs() : Promise<any> {
    await this.connected;
    return await this.process.GetProcs();
  }

  // public async getChanges() : Promise<any>{
  //   await this.connected;
  //   return await this.process.ChangedObject;
  // }

  public getData(tableName: string): Observable<any[]> {
    return of(MockProcesses[tableName]);
  }
}

