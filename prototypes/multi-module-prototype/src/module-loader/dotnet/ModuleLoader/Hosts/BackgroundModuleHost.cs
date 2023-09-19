﻿/// ********************************************************************************************************
///
/// Morgan Stanley makes this available to you under the Apache License, Version 2.0 (the "License").
/// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
/// See the NOTICE file distributed with this work for additional information regarding copyright ownership.
/// Unless required by applicable law or agreed to in writing, software distributed under the License
/// is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and limitations under the License.
/// 
/// ********************************************************************************************************

using MorganStanley.ComposeUI.Tryouts.Core.Abstractions.Modules;

namespace MorganStanley.ComposeUI.Tryouts.Core.Services.ModulesService.Hosts
{
    internal class BackgroundModuleHost : ModuleHostBase
    {
        private readonly IModuleRunner _runner;

        public BackgroundModuleHost(string name, Guid instanceId, IModuleRunner runner) : base(name, instanceId)
        {
            _runner = runner;
            _processInfo = new ProcessInfo(base.Name, base.InstanceId, UIType.None, null, 0); 
        }

        public override ProcessInfo ProcessInfo => _processInfo;

        private ProcessInfo _processInfo;

        public async override Task Launch()
        {
            var pid = await _runner.Launch();
            _processInfo.pid = pid;
            _lifecycleEvents.OnNext(LifecycleEvent.Started(ProcessInfo));
        }

        public async override Task Teardown()
        {
            await _runner.Stop();
            _lifecycleEvents.OnNext(LifecycleEvent.Stopped(ProcessInfo, true));
        }
    }
}
