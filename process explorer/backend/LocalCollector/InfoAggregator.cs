/* Morgan Stanley makes this available to you under the Apache License, Version 2.0 (the "License"). You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. See the NOTICE file distributed with this work for additional information regarding copyright ownership. Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License. */

using LocalCollector;
using LocalCollector.Connections;
using LocalCollector.Logging;
using LocalCollector.Modules;
using LocalCollector.Registrations;
using ProcessExplorer.Entities.Connections;
using ProcessExplorer.Entities.EnvironmentVariables;
using ProcessExplorer.Entities.Registrations;
using ProcessExplorer.Processes.RPCCommunicator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ProcessExplorer
{
    public class InfoAggregator : IInfoAggregator
    {
        #region Properties
        public InfoAggregatorDto Data { get; set; } = new InfoAggregatorDto();
        internal ConnectionMonitor? ConnectionMonitor { get; set; }
        private readonly ICommunicator? channel;
        private readonly ILogger<InfoAggregator>? logger;
        private readonly object locker = new object();
        #endregion

        #region Constructors
        InfoAggregator(ICommunicator? channel = null, ILogger<InfoAggregator>? logger = null)
        {
            this.channel = channel;
            this.logger = logger;
        }

        public InfoAggregator(EnvironmentMonitorDto envs, ConnectionMonitor cons, ICommunicator? channel = null, ILogger<InfoAggregator>? logger = null)
            : this(channel, logger)
        {
            Data.Id = Process.GetCurrentProcess().Id;
            Data.EnvironmentVariables = envs;
            ConnectionMonitor = cons;
            Data.Connections = ConnectionMonitor.Data;

            SetConnectionChangedEvent();
        }

        public InfoAggregator(EnvironmentMonitorDto envs, ConnectionMonitor cons,
            RegistrationMonitorDto registrations, ModuleMonitorDto modules, ICommunicator? channel = null, ILogger<InfoAggregator>? logger = null)
            : this(envs, cons, channel, logger)
        {
            Data.Registrations = registrations;
            Data.Modules = modules;
        }

        public InfoAggregator(ConnectionMonitor cons, ICollection<RegistrationDto> regs, ICommunicator? channel = null, ILogger<InfoAggregator>? logger = null)
            : this(EnvironmentMonitorDto.FromEnvironment(), cons, RegistrationMonitorDto.FromCollection(regs), ModuleMonitorDto.FromAssembly(), channel, logger)
        {

        }

        public InfoAggregator(ConnectionMonitor cons, IServiceCollection regs, ICommunicator? channel = null, ILogger<InfoAggregator>? logger = null)
            : this(EnvironmentMonitorDto.FromEnvironment(), cons, RegistrationMonitorDto.FromCollection(regs), ModuleMonitorDto.FromAssembly(), channel, logger)
        {

        }
        #endregion

        protected void SetConnectionChangedEvent()
        {
            ConnectionMonitor?.SetSendConnectionStatusChanged(SendMessageAboutConnectionChangedEvent);
        }

        public async Task SendMessageAboutConnectionChangedEvent(ConnectionDto? conn)
        {
            if (channel is not null && conn is not null)
            {
                lock (locker)
                {
                    try
                    {
                        var info = Data.Connections?.Connections.Where(p => p.Id == conn.Id).FirstOrDefault();
                        if (info is not null)
                        {
                            var index = Data.Connections?.Connections.IndexOf(info);
                            if (index >= 0 && Data.Connections?.Connections.Count <= index)
                            {
                                Data.Connections?.Connections.RemoveAt(Convert.ToInt32(index));
                                Data.Connections?.Connections.Add(conn);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        logger?.ConnectionStatusChanged(exception);
                    }
                }
                //await channel.SendMessage(conn);
                await channel.Update(conn);
            }
        }

        public async Task SendObject()
        {
            if (channel is not null)
            {
                await channel.Add(this.Data);
            }
        }

        public async void AddConnectionMonitor(ConnectionMonitorDto connections)
        {
            bool flag = false;
            if (connections.Connections is not null)
            {
                flag = true;
                lock (locker)
                {
                    foreach (var conn in connections.Connections)
                    {
                        if (conn is not null)
                        {
                            Data.Connections?.Connections.Add(conn);
                        }
                    }
                }
                if (flag)
                    await channel?.Add(connections.Connections);
            }
        }
        public void AddConnectionMonitor(ConnectionMonitor connections)
            => AddConnectionMonitor(connections.Data);


        public async void AddEnvironmentVariables(EnvironmentMonitorDto environmentVariables)
        {
            if (Data.EnvironmentVariables is null)
            {
                Data.EnvironmentVariables = environmentVariables;
            }
            else
                lock (locker)
                {
                    foreach (var env in environmentVariables.EnvironmentVariables)
                    {
                        Data.EnvironmentVariables.EnvironmentVariables.AddOrUpdate(
                            env.Key, env.Value, (_, _) => env.Value);
                    }
                }
            await channel?.Update(environmentVariables);
        }

        public async void AddRegistrations(RegistrationMonitorDto registrations)
        {
            if (registrations.Services is not null)
                lock (locker)
                {
                    if (Data.Registrations is null)
                        Data.Registrations = registrations;
                    else
                    {
                        foreach (var reg in registrations.Services)
                        {
                            if (reg is not null)
                                Data.Registrations.Services.Add(reg);
                        }
                    }
                }
            await channel?.Update(registrations);
        }

        public async void AddModules(ModuleMonitorDto modules)
        {
            if (modules.CurrentModules is not null)
                lock (locker)
                {
                    if (Data.Modules is null)
                        Data.Modules = modules;
                    else
                    {
                        foreach (var module in modules.CurrentModules)
                        {
                            if (module is not null)
                                Data.Modules.CurrentModules.Add(module);
                        }
                    }
                }
            await channel?.Update(modules);
        }

        public void AddInformation(ConnectionMonitor connections, EnvironmentMonitorDto envrionmentVariables,
            RegistrationMonitorDto registrations, ModuleMonitorDto modules)
        {
            AddConnectionMonitor(connections);
            AddEnvironmentVariables(envrionmentVariables);
            AddRegistrations(registrations);
            AddModules(modules);
        }
    }
}
