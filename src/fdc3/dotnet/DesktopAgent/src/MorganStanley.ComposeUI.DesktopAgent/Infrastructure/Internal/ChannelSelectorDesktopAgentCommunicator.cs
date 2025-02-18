
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Text.Json;
using Finos.Fdc3;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Contracts;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Converters;
using MorganStanley.ComposeUI.Messaging;
using MorganStanley.ComposeUI.Messaging.Abstractions;

namespace MorganStanley.ComposeUI.Fdc3.DesktopAgent.Tests.Infrastructure.Internal
{
    internal class ChannelSelectorDesktopAgentCommunicator : IChannelSelectorCommunicator
    {

        private readonly ILogger<ChannelSelectorDesktopAgentCommunicator> _logger;
        private readonly IMessageRouter _messageRouter;
        private readonly TimeSpan _defaultTimeout = TimeSpan.FromMinutes(2);

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            Converters = { new AppMetadataJsonConverter() }
        };

        public ChannelSelectorDesktopAgentCommunicator(
            IMessageRouter messageRouter,
            ILogger<ChannelSelectorDesktopAgentCommunicator>? logger = null)
        {
            _messageRouter = messageRouter;
            _logger = logger ?? NullLogger<ChannelSelectorDesktopAgentCommunicator>.Instance;
        }

        public Task<ChannelSelectorResponse?> SendChannelSelectorColorUpdateRequest(JoinUserChannelRequest request, string? color, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<ChannelSelectorResponse?> SendChannelSelectorRequest(string channelId, string instanceId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await SendChannelSelectorRequestCore(channelId,  instanceId,  cancellationToken);
            }
            catch (TimeoutException ex)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug(ex, "MessageRouter didn't receive response from the Channel Selector.");
                }

                return new ChannelSelectorResponse()
                {
                    Error = ResolveError.ResolverTimeout
                };
            }
        }

       


        //p//ublic string ChannelId { get; set; }

        /// <summary>
        /// Unique identifier of the app which sent the request.
        /// </summary>
        //public string InstanceId { get; set; }

        private async Task<ChannelSelectorResponse?> SendChannelSelectorRequestCore(string channelId, string instanceId, CancellationToken cancellationToken = default)
        {
            var request = new ChannelSelectorRequest
            {
                //AppMetadata = appMetadata

                ChannelId = channelId,
                InstanceId =instanceId
            };

            var responseBuffer = await _messageRouter.InvokeAsync(
                Fdc3Topic.ChannelSelector,
                MessageBuffer.Factory.CreateJson(request, _jsonSerializerOptions),
                cancellationToken: cancellationToken);

            if (responseBuffer == null)
            {
                return null;
            }

            var response = responseBuffer.ReadJson<ChannelSelectorResponse>(_jsonSerializerOptions);

            return response;
        }


    }

}
