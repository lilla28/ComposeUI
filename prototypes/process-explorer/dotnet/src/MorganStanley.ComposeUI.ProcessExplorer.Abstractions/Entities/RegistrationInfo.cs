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

namespace MorganStanley.ComposeUI.ProcessExplorer.Abstractions.Entities;

public class RegistrationInfo
{
    public string? ImplementationType { get; set; }
    public string? LifeTime { get; set; }
    public string? ServiceType { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

        if (obj.GetType() != typeof(RegistrationInfo)) return false;

        return ImplementationType == ((RegistrationInfo)obj).ImplementationType
            && ServiceType == ((RegistrationInfo)obj).ServiceType;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ImplementationType, ServiceType);
    }
}
