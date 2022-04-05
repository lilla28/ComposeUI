/* Morgan Stanley makes this available to you under the Apache License, Version 2.0 (the "License"). You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. See the NOTICE file distributed with this work for additional information regarding copyright ownership. Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License. */
import { Component, OnInit } from '@angular/core';
import { MockProcessesService } from '../../services/mock-processes.service';
import * as Highcharts from 'highcharts';
import { distinctUntilChanged } from 'rxjs/operators';
import { interval } from 'rxjs';



@Component({
  selector: 'app-processes',
  templateUrl: './processes.component.html',
  styleUrls: ['./processes.component.scss']
})
export class ProcessesComponent implements OnInit {
  public mockProcessesData: any = null;
  public processes: any;

  public sub$ : any;
  public changed: any;

  Highcharts: typeof Highcharts = Highcharts;
  chartOptions: Highcharts.Options = {
    series: [{
      data: [1, 2, 3],
      type: 'line'
    }]
  };

  constructor(private mockProcessesService: MockProcessesService) { 
    
  }

  async ngOnInit() {
    // depending on implementation, data subscriptions might need to be unsubbed later
    this.mockProcessesService.getData('Processes').subscribe(data => this.mockProcessesData = data);
    // this.processes = await this.mockProcessesService.getProcs();
    // console.log('Processes:', this.processes);
    
    // this.sub$ = interval(100)
    //   .subscribe(async() => 
    //       {
    //         const value = await this.mockProcessesService.getChanges()
    //         if (this.changed != value) {
    //           this.changed = value; 
    //           console.log(this.changed);
    //         }
    //       })

    // this.sub$ = await this.mockProcessesService.getChanges();
    // this.sub$.pipe(distinctUntilChanged())
    //   .subscribe(console.log)
  }

}

