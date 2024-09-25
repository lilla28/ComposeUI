// Morgan Stanley makes this available to you under the Apache License,
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

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent;
using MorganStanley.ComposeUI.ModuleLoader;
using MorganStanley.ComposeUI.Utilities;

namespace MorganStanley.ComposeUI.Shell.Wrappers;

internal class WrapperStartupAction : IStartupAction
{
    //TODO: Remove the whole thing
    public async Task InvokeAsync(StartupContext startupContext, Func<Task> next)
    {
        if (startupContext.ModuleInstance.Manifest.ModuleType == ModuleType.Web)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var t = assembly.GetManifestResourceNames();
            using var stream = assembly.GetManifestResourceStream(Constants.WindowCloseWrapper);
            using var reader = new StreamReader(stream!);
            var script = reader.ReadToEnd();

            var webProperties = startupContext.GetOrAddProperty<WebStartupProperties>();

            webProperties.ScriptProviders.Add(_ => new ValueTask<string>(script));
        }

        await next.Invoke();
    }
}
