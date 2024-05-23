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

using System;
using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MorganStanley.ComposeUI.Shell.Core;

namespace MorganStanley.ComposeUI.Shell;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public new static App Current => (App) Application.Current;

    private readonly Framework _framework = new();
    public App()
    {
        //TODO
        _framework.SetConfigurationFile($"{Directory.GetCurrentDirectory()}\\appsettings.json");
        _framework.SetLogLevel(LogLevel.Debug);
        _framework.SetMessageRouterAccessToken(MessageRouterAccessToken);
    }

    // TODO: Assign a unique token for each module
    internal readonly string MessageRouterAccessToken = Guid.NewGuid().ToString("N");
}