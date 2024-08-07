﻿// Morgan Stanley makes this available to you under the Apache License,
// Version 2.0 (the "License"). You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0.
// 
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership. Unless required by applicable law or agreed
// to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
// or implied. See the License for the specific language governing permissions
// and limitations under the License.

using Microsoft.Extensions.Logging;
using MorganStanley.ComposeUI.ProcessExplorer.Abstractions;
using MorganStanley.ComposeUI.ProcessExplorer.Abstractions.Infrastructure;
using MorganStanley.ComposeUI.ProcessExplorer.Abstractions.Processes;
using MorganStanley.ComposeUI.ProcessExplorer.Abstractions.Subsystems;
using MorganStanley.ComposeUI.ProcessExplorer.Core.Processes;

namespace MorganStanley.ComposeUI.ProcessExplorer.Core.Factories;

public static class ProcessAggregatorFactory
{
    public static IProcessInfoAggregator CreateProcessInfoAggregator(
        IProcessInfoMonitor processInfoMonitor, 
        IUiHandler handler,
        ISubsystemController subsystemController, 
        ILogger<IProcessInfoAggregator>? logger = null)
    {
        return new ProcessInfoAggregator(processInfoMonitor, handler, subsystemController, logger);
    }
}
