/* Morgan Stanley makes this available to you under the Apache License, Version 2.0 (the "License"). You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. See the NOTICE file distributed with this work for additional information regarding copyright ownership. Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License. */

namespace ProcessExplorer.Subsystems
{
    public class SubsystemLauncher : ISubsystemLauncher
    {
        public Task LaunchAllRegisteredSubsystem()
        {
            throw new NotImplementedException();
        }

        public Task LaunchSubsystemAfterTime(int subsystemId, int periodOfTime)
        {
            throw new NotImplementedException();
        }

        public Task LaunchSubsystemAutomatically(int subsytemId)
        {
            throw new NotImplementedException();
        }

        public Task LaunchSubsystems(IEnumerable<int> subsystems)
        {
            throw new NotImplementedException();
        }

        public Task LaunchSubsytem(int subsytemId)
        {
            throw new NotImplementedException();
        }

        public Task RestartSubsystems(IEnumerable<int> subsystems)
        {
            throw new NotImplementedException();
        }

        public Task RestartSubsytem(int subsytemId)
        {
            throw new NotImplementedException();
        }

        public Task ShutdownAllRegisteredSubsystem()
        {
            throw new NotImplementedException();
        }

        public Task ShutdownSubsystem(int subsytemId)
        {
            throw new NotImplementedException();
        }

        public Task ShutdownSubsystems(IEnumerable<int> subsystems)
        {
            throw new NotImplementedException();
        }
    }
}
