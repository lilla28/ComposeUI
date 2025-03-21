/*
 * Morgan Stanley makes this available to you under the Apache License,
 * Version 2.0 (the "License"). You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0.
 *
 * See the NOTICE file distributed with this work for additional information
 * regarding copyright ownership. Unless required by applicable law or agreed
 * to in writing, software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
 * or implied. See the License for the specific language governing permissions
 * and limitations under the License.
 */

using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Contracts;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Converters;
using MorganStanley.ComposeUI.Messaging;
using MorganStanley.ComposeUI.Messaging.Abstractions;

namespace MorganStanley.ComposeUI.Fdc3.DesktopAgent.Infrastructure.Internal;

internal class MessageRouterChannelSelectorBridgeCommunicator : IChannelSelectorBridgeCommunicator
{
    private readonly IMessageRouter _messageRouterClient;
    private readonly ILogger<MessageRouterChannelSelectorBridgeCommunicator> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new AppMetadataJsonConverter(), new IconJsonConverter() }
    };

    public MessageRouterChannelSelectorBridgeCommunicator(
        IMessageRouter messageRouter,
        ILogger<MessageRouterChannelSelectorBridgeCommunicator>? logger = null)
    {
        _messageRouterClient = messageRouter;
        _logger = logger ?? NullLogger<MessageRouterChannelSelectorBridgeCommunicator>.Instance;
    }

    public async Task RegisterChannelChangedServiceAsync(string instanceId, CancellationToken cancellationToken = default)
    {
        await _messageRouterClient.RegisterServiceAsync(
                Fdc3Topic.ChannelSelectorUI(instanceId),
                ChannelSelectorBridgeHandler,
                cancellationToken);
    }

    private async ValueTask<IMessageBuffer?> ChannelSelectorBridgeHandler(string endpoint, IMessageBuffer? payload, MessageContext? context)
    {
        var request = payload?.ReadJson<JoinUserChannelRequest>(_jsonSerializerOptions);
        string? instanceId = null, color = null, channelId = null;

        if (request != null)
        {
            instanceId = request.InstanceId;
            color = request.Color;
            channelId = request.ChannelId;
        }

        if (!string.IsNullOrEmpty(instanceId) 
            && !string.IsNullOrEmpty(color)
            && !string.IsNullOrEmpty(channelId))
        {
            var channelChangedDueToUIRequest = new UIUserChannelChangedRequest
            {
                InstanceId = instanceId,
                Color = color,
                ChannelId = channelId
            };

            var responseBuffer = await _messageRouterClient.InvokeAsync(
                Fdc3Topic.UserChannelUIChanged(channelChangedDueToUIRequest.InstanceId),
                MessageBuffer.Factory.CreateJson(channelChangedDueToUIRequest, _jsonSerializerOptions));

            var response = responseBuffer?.ReadJson<UIUserChannelChangedResponse>(_jsonSerializerOptions);

            if (response == null)
            {
                _logger.LogError("Error occurred while sending user channel changed request to clients: No response received");
                return null;
            }

            if (response.Error != null)
            {
                _logger.LogError("Error occurred while sending user channel changed request to clients: {Error}", response.Error);
            }

            if (!response.Success)
            {
                _logger.LogError("Error occurred while sending user channel changed request to clients: Request was not successful");
            }
        }

        //We do not want to return anything to the caller
        return null;
    }

}
