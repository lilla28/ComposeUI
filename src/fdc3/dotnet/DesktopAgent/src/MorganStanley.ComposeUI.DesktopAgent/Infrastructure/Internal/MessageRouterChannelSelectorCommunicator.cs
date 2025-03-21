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
using MorganStanley.ComposeUI.Messaging;

namespace MorganStanley.ComposeUI.Fdc3.DesktopAgent.Infrastructure.Internal;

internal class MessageRouterChannelSelectorCommunicator : IChannelSelectorCommunicator
{
    private readonly IMessageRouter _messageRouter;
    private readonly ILogger<MessageRouterChannelSelectorCommunicator> _logger;
    private readonly JsonSerializerOptions? _jsonSerializerOptions = new(JsonSerializerDefaults.Web);

    public MessageRouterChannelSelectorCommunicator(
        IMessageRouter messageRouter,
        ILogger<MessageRouterChannelSelectorCommunicator>? logger = null)
    {
        _messageRouter = messageRouter;
        _logger = logger ?? NullLogger<MessageRouterChannelSelectorCommunicator>.Instance;
    }

    public async Task SendChannelSelectorColorUpdateRequestToTheUI(
        JoinUserChannelRequest request, 
        string? color, 
        CancellationToken cancellationToken = default)
    {
        Thread.Yield();

        var topic = Fdc3Topic.UserChannelClientChanged(request.InstanceId);

        try
        {
            await _messageRouter.PublishAsync(
                topic,
                MessageBuffer.Factory.CreateJson(request, _jsonSerializerOptions),
                cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to send channel selector color update request to the UI.");
        }

        return;
    }
}
