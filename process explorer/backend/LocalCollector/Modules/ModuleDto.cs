﻿/* Morgan Stanley makes this available to you under the Apache License, Version 2.0 (the "License"). You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. See the NOTICE file distributed with this work for additional information regarding copyright ownership. Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License. */

using System.Reflection;

namespace ProcessExplorer.Entities.Modules
{
    public class ModuleDto 
    {
        #region Properties
        public string? Name { get; set; }
        public Guid? Version { get; set; }
        public string? VersionRedirectedFrom { get; set; }
        public byte[]? PublicKeyToken { get; set; }
        public string? Location { get; internal set; }
        public SynchronizedCollection<CustomAttributeData> Information { get; set; } = new SynchronizedCollection<CustomAttributeData>();
        #endregion

        public static ModuleDto FromModule(Assembly assembly, Module module)
        {
            string? location = string.Empty;
            try
            {
                location = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(module.Assembly.Location).Path));
            }
            catch (Exception)
            {
                location = null;
            }
            return new ModuleDto()
            {
                Name = assembly.GetName().Name,
                Version = module.ModuleVersionId,
                VersionRedirectedFrom = assembly.ManifestModule.ModuleVersionId.ToString(),
                PublicKeyToken = assembly.GetName()?.GetPublicKeyToken(),
                Location = location
            };
        }

        public static ModuleDto FromProperties(string name, Guid version, string versionrf, byte[] publickey,
            string path, SynchronizedCollection<CustomAttributeData> information)
        {
            return new ModuleDto()
            {
                Name = name,
                Version = version,
                VersionRedirectedFrom = versionrf,
                PublicKeyToken = publickey,
                Location = path,
                Information = information
            };
        }
    }
}