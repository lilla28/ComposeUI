﻿/* Morgan Stanley makes this available to you under the Apache License, Version 2.0 (the "License"). You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. See the NOTICE file distributed with this work for additional information regarding copyright ownership. Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License. */

namespace ProcessExplorer.LocalCollector.Registrations
{
    public class RegistrationInfo
    {
        #region Properties
        public string? ImplementationType { get; set; }
        public string? LifeTime { get; set; }
        public string? ServiceType { get; set; }
        #endregion
        public static RegistrationInfo FromProperties(string implementationType, string serviceType, string lifeTime)
        {
            return new RegistrationInfo()
            {
                ImplementationType = implementationType,
                LifeTime = lifeTime,
                ServiceType = serviceType
            };
        }
    }
}
