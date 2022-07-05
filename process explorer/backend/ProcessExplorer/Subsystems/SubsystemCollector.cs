﻿/* Morgan Stanley makes this available to you under the Apache License, Version 2.0 (the "License"). You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. See the NOTICE file distributed with this work for additional information regarding copyright ownership. Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License. */

using System.Collections.Concurrent;

namespace ProcessExplorer.Subsystems
{
    public class SubsystemCollector : ISubsystemCollector
    {
        public ConcurrentDictionary<int, SubsystemInfo> subsystems = new ConcurrentDictionary<int, SubsystemInfo>();
        ConcurrentDictionary<int, SubsystemInfo> ISubsystemCollector.subsystems { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task AddSubsytem(SubsystemInfo subsystem)
        {
            throw new NotImplementedException();
        }

        public Task RemoveSubsytem(SubsystemInfo subsystem)
        {
            throw new NotImplementedException();
        }
    }
}
