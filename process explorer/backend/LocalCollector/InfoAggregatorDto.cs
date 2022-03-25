﻿/* Morgan Stanley makes this available to you under the Apache License, Version 2.0 (the "License"). You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. See the NOTICE file distributed with this work for additional information regarding copyright ownership. Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License. */

using LocalCollector.Connections;
using LocalCollector.Modules;
using ProcessExplorer.Entities.EnvironmentVariables;
using ProcessExplorer.Entities.Registrations;
using System.Diagnostics;

namespace LocalCollector
{
    public class InfoAggregatorDto
    {
        public int Id { get; set; } = Process.GetCurrentProcess().Id;
        public RegistrationMonitorDto? Registrations { get; set; }
        public EnvironmentMonitorDto? EnvironmentVariables { get; set; }
        public ConnectionMonitorDto? Connections { get; set; }
        public ModuleMonitorDto? Modules { get; set; }
    }
}
