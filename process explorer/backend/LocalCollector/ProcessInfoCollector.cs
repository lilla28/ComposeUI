/* Morgan Stanley makes this available to you under the Apache License, Version 2.0 (the "License"). You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. See the NOTICE file distributed with this work for additional information regarding copyright ownership. Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License. */

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using ProcessExplorer.LocalCollector.Communicator;
using ProcessExplorer.LocalCollector.Connections;
using ProcessExplorer.LocalCollector.EnvironmentVariables;
using ProcessExplorer.LocalCollector.Logging;
using ProcessExplorer.LocalCollector.Modules;
using ProcessExplorer.LocalCollector.Registrations;

namespace ProcessExplorer.LocalCollector
{
    public class ProcessInfoCollector : IProcessInfoCollector
    {
        #region Properties
        public ProcessInfoCollectorData Data { get;} = new ProcessInfoCollectorData();
        private ConnectionMonitor? ConnectionMonitor { get; }
        private ICommunicator? channel;
        private readonly ILogger<ProcessInfoCollector>? logger;
        private readonly string assemblyId = Assembly.GetExecutingAssembly().GetName().Name;
        private readonly object locker = new object();
        #endregion

        #region Constructors
        ProcessInfoCollector(ICommunicator? channel = null, ILogger<ProcessInfoCollector>? logger = null)
        {
            this.channel = channel;
            this.logger = logger;
        }

        public ProcessInfoCollector(EnvironmentMonitorInfo envs, ConnectionMonitor cons, ICommunicator? channel = null, ILogger<ProcessInfoCollector>? logger = null)
            : this(channel, logger)
        {
            Data.Id = Process.GetCurrentProcess().Id;
            Data.EnvironmentVariables = envs;
            ConnectionMonitor = cons;
            Data.Connections = ConnectionMonitor.Data;

            SetConnectionChangedEvent();
        }

        public ProcessInfoCollector(EnvironmentMonitorInfo envs, ConnectionMonitor cons,
            RegistrationMonitorInfo registrations, ModuleMonitorInfo modules, ICommunicator? channel = null, ILogger<ProcessInfoCollector>? logger = null)
            : this(envs, cons, channel, logger)
        {
            Data.Registrations = registrations;
            Data.Modules = modules;
        }

        public ProcessInfoCollector(ConnectionMonitor cons, ICollection<RegistrationInfo> regs, ICommunicator? channel = null, ILogger<ProcessInfoCollector>? logger = null)
            : this(EnvironmentMonitorInfo.FromEnvironment(), cons, RegistrationMonitorInfo.FromCollection(regs), ModuleMonitorInfo.FromAssembly(), channel, logger)
        {

        }

        public ProcessInfoCollector(ConnectionMonitor cons, IServiceCollection regs, ICommunicator? channel = null, ILogger<ProcessInfoCollector>? logger = null)
            : this(EnvironmentMonitorInfo.FromEnvironment(), cons, RegistrationMonitorInfo.FromCollection(regs), ModuleMonitorInfo.FromAssembly(), channel, logger)
        {

        }
        #endregion

        private void SetConnectionChangedEvent()
        {
            ConnectionMonitor?.SetSendConnectionStatusChanged(SendMessageAboutConnectionChangedEvent);
        }

        public void SetCommunicator(ICommunicator communicator)
        {
            this.channel = communicator;
            channel.AddRuntimeInfo(Assembly.GetCallingAssembly().GetName().Name, Data);
        }
        private bool CheckCommunicationChannel()
            => channel is not null;

        public async Task SendMessageAboutConnectionChangedEvent(ConnectionInfo? conn)
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
                if(CheckCommunicationChannel())
                    await channel.UpdateConnectionInformation(assemblyId, conn);
            }
        }

        public async Task SendRuntimeInfo()
        {
            if(CheckCommunicationChannel())
            {
                await channel.AddRuntimeInfo(assemblyId, this.Data);
            }
        }

        public async void AddConnectionMonitor(ConnectionMonitorInfo connections)
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
                if (flag && CheckCommunicationChannel())
                    await channel.AddConnectionCollection(assemblyId, connections.Connections);
            }
        }
        public void AddConnectionMonitor(ConnectionMonitor connections)
            => AddConnectionMonitor(connections.Data);


        public async void AddEnvironmentVariables(EnvironmentMonitorInfo environmentVariables)
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
            if(CheckCommunicationChannel())
                await channel.UpdateEnvironmentVariableInformation(assemblyId,environmentVariables);
        }

        public async void AddRegistrations(RegistrationMonitorInfo registrations)
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
            if(CheckCommunicationChannel())
                await channel.UpdateRegistrationInformation(assemblyId, registrations);
        }

        public async void AddModules(ModuleMonitorInfo modules)
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
            if(CheckCommunicationChannel())
                await channel.UpdateModuleInformation(assemblyId, modules);
        }

        public void AddRuntimeInformation(ConnectionMonitor connections, EnvironmentMonitorInfo environmentVariables,
            RegistrationMonitorInfo registrations, ModuleMonitorInfo modules)
        {
            AddConnectionMonitor(connections);
            AddEnvironmentVariables(environmentVariables);
            AddRegistrations(registrations);
            AddModules(modules);
        }
    }
}
