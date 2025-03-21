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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.DependencyInjection;
using MorganStanley.ComposeUI.ModuleLoader;

namespace MorganStanley.ComposeUI.Shell.Fdc3.ChannelSelector;

internal class Fdc3ChannelSelectorStartupAction : IStartupAction
{
    private readonly IChannelSelector _channelSelector;
    private readonly IUserChannelSetReader _userChannelSetReader;
    private readonly Fdc3DesktopAgentOptions _options;
    private readonly ILogger<Fdc3ChannelSelectorStartupAction> _logger;

    public Fdc3ChannelSelectorStartupAction(
        IChannelSelector channelSelector,
        IUserChannelSetReader userChannelSetReader,
        IOptions<Fdc3DesktopAgentOptions> options,
        ILogger<Fdc3ChannelSelectorStartupAction>? logger = null)
    {
        _channelSelector = channelSelector;
        _userChannelSetReader = userChannelSetReader;
        _options = options.Value;
        _logger = logger ?? NullLogger<Fdc3ChannelSelectorStartupAction>.Instance;
    }

    [STAThread]
    public async Task InvokeAsync(StartupContext startupContext, Func<Task> next)
    {
        var fdc3InstanceId = startupContext.GetOrAddProperty<Fdc3StartupProperties>().InstanceId;
        if (string.IsNullOrEmpty(fdc3InstanceId))
        {
            await (next.Invoke());
        }

        var userChannelSet = await _userChannelSetReader.GetUserChannelSet();
        var channelId = startupContext.StartRequest.Parameters
            .FirstOrDefault(parameter => parameter.Key == "Fdc3ChannelId").Value ?? _options.ChannelId ?? userChannelSet.FirstOrDefault().Key;

        var channelColor = userChannelSet[channelId].DisplayMetadata.Color;

        var fdc3ChannelSelectorControl = new Fdc3ChannelSelectorControl(_channelSelector, channelColor!, fdc3InstanceId, userChannelSet);
        await Task.Run(async () =>
        {
            await _channelSelector.RegisterChannelChangedServiceAsync(fdc3InstanceId, fdc3ChannelSelectorControl.OnChannelChanged);
        });

        Thread.Yield();

        var webProperties = startupContext.GetOrAddProperty<WebStartupProperties>();

        webProperties.UIElements.Add(fdc3ChannelSelectorControl);

        await (next.Invoke());
    }
}
