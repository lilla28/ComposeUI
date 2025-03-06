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
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Finos.Fdc3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Contracts;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Converters;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Exceptions;
using MorganStanley.ComposeUI.Messaging;
using MorganStanley.ComposeUI.Messaging.Abstractions;

namespace MorganStanley.ComposeUI.Shell.Fdc3.ChannelSelector;

internal class Fdc3ChannelSelectorSerice : IHostedService
{
    private readonly object _disposeLock = new();

    private readonly List<Func<ValueTask>> _disposeTask = new();
    private readonly IHost _host;
    private IChannelSelectorCommunicator _channelSelectorComm; //= //new ChannelSelectorCommunicator();  //todo reference to the control or the model.. 
    //private Fdc3ChannelSelectorControl _channelSelector;  //todo reference to the control or the model.. 
    private Fdc3ChannelSelectorViewModel _channelSelector;
    private string? _instanceId;
    private string? _channelId;
    private string? _color;
    private IDesktopAgent _desktopAgent;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters = { new AppMetadataJsonConverter(), new IconJsonConverter() }
    };

    public Fdc3ChannelSelectorSerice(IHost host)
    {
        _host = host;
        var messageRouter = _host.Services.GetService<IMessageRouter>();
        _channelSelectorComm = new ChannelSelectorShellCommunicator(messageRouter);
        _channelSelector = new Fdc3ChannelSelectorViewModel(_channelSelectorComm, _color); //comm is null
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await StartMessageRouterServiceAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Func<ValueTask>[]? reversedList = null;

        lock (_disposeLock)
        {
            reversedList = _disposeTask.AsEnumerable().Reverse().ToArray();
        }


        if (reversedList != null)
        {
            for (var i = 0; i < reversedList.Length; i++)
            {
                await reversedList[i].Invoke();
            }
        }
    }

    private async Task StartMessageRouterServiceAsync(CancellationToken cancellationToken)
    {
        var messageRouter = _host.Services.GetRequiredService<IMessageRouter>();

        await messageRouter.RegisterServiceAsync(
            "ComposeUI/fdc3/v2.0/channelSelector", 
            async (endpoint, payload, context) =>
            {
                var request = payload?.ReadJson<ChannelSelectorRequest>(_jsonSerializerOptions); //Todo delete ChannelSelectorRequest
                if (request == null)
                {
                    _instanceId = request.InstanceId;
                    return null;
                }

                //var response = await JoinUserChannel(request.ChannelId); //todo
                var response = await UpdateChannelSelectorColor(request.Color);

               // var response = await HandleJoinUserChannel(request);

                return response is null ? null : MessageBuffer.Factory.CreateJson(response, _jsonSerializerOptions);
            },
            cancellationToken: cancellationToken);



        lock (_disposeLock)
        {
            _disposeTask.Add(
                async () =>
                {
                    if (messageRouter != null)
                    {
                        await messageRouter.UnregisterServiceAsync("ComposeUI/fdc3/v2.0/channelSelector", cancellationToken);

                        await messageRouter.DisposeAsync();
                    }
                });
        }
    }








    private ValueTask<ChannelSelectorResponse> UpdateChannelSelectorColor(string color) 
    {
        var currentColor = (Color) ColorConverter.ConvertFromString(color);

        _channelSelector.CurrentChannelColor = new SolidColorBrush(currentColor);
        //_channelSelector.
        return new ValueTask<ChannelSelectorResponse>(); //todo replace fake return value

    }
    //JoinUserChannelResponse?
    internal async ValueTask<ChannelSelectorResponse> HandleJoinUserChannel(ChannelSelectorRequest? request) //JoinUserChannelRequest
    {
        if (request == null)
        {
            //return JoinUserChannelResponse.Failed(Fdc3DesktopAgentErrors.PayloadNull);
        }

        return null;//_desktopAgent.JoinUserChannel(request.ChannelId);
    }




}