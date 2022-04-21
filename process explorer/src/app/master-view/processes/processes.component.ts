/* Morgan Stanley makes this available to you under the Apache License, Version 2.0 (the "License"). You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. See the NOTICE file distributed with this work for additional information regarding copyright ownership. Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License. */
import { Component, NgZone, OnInit, ViewChild } from '@angular/core';
import { MockProcessesService } from '../../services/mock-processes.service';
import * as Highcharts from 'highcharts';
import { interval, Subscription } from 'rxjs';

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

@Component({
  selector: 'app-processes',
  templateUrl: './processes.component.html',
  styleUrls: ['./processes.component.scss']
})
export class ProcessesComponent implements OnInit {
  @ViewChild("treeGrid") TreeGrid : any;

  public mockProcessesData: Array<ProcessInfo>;
  public processes: any;

  private subscription : Subscription;

  Highcharts: typeof Highcharts = Highcharts;
  chartOptions: Highcharts.Options = {
    series: [{
      data: [1, 2, 3],
      type: 'line'
    }]
  };

  constructor(private ngZone: NgZone, private mockProcessesService: MockProcessesService) { 
    
  }

  ngOnInit() {
    // depending on implementation, data subscriptions might need to be unsubbed later
    // this.mockProcessesService.getData('Processes').subscribe(data => this.mockProcessesData = data);
      this.checkForChanges();
  }

  public checkForChanges(){
    this.subscription = interval(100).subscribe( () => this.ngZone.run(() => {
      this.mockProcessesData = this.mockProcessesService.getProcs();
      this.TreeGrid.markForCheck();
    }));
  }
}

