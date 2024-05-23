// /*
//  * Morgan Stanley makes this available to you under the Apache License,
//  * Version 2.0 (the "License"). You may obtain a copy of the License at
//  *
//  *      http://www.apache.org/licenses/LICENSE-2.0.
//  *
//  * See the NOTICE file distributed with this work for additional information
//  * regarding copyright ownership. Unless required by applicable law or agreed
//  * to in writing, software distributed under the License is distributed on an
//  * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
//  * or implied. See the License for the specific language governing permissions
//  * and limitations under the License.
//  */

using Microsoft.Extensions.Logging;
using MorganStanley.ComposeUI.Shell.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class FrameworkExtensions
{
    public static string? AccessToken { get; private set; }
    public static string? ConfigurationFile { get; private set; }
    public static LogLevel MinimumLogLevel { get; private set; } = LogLevel.Information;

    /// <summary>
    /// Sets the MessageRouter's AccessToken for providing unique token to each module.
    /// </summary>
    /// <param name="framework"></param>
    /// <param name="accessToken"></param>
    public static void SetMessageRouterAccessToken(this Framework framework, string accessToken)
    {
        AccessToken = accessToken;
    }

    /// <summary>
    /// Sets the configuration file's path, which can contain the settings of the modules.
    /// </summary>
    /// <param name="framework"></param>
    /// <param name="configFile"></param>
    public static void SetConfigurationFile(this Framework framework, string configFile)
    {
        ConfigurationFile = configFile;
    }

    /// <summary>
    /// Sets the minimum LogLevel. By default it's <seealso cref="LogLevel.Information"/>.
    /// </summary>
    /// <param name="framework"></param>
    /// <param name="level"></param>
    public static void SetLogLevel(this Framework framework, LogLevel level)
    {
        MinimumLogLevel = level;
    }
}
