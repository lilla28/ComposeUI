using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Finos.Fdc3;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Contracts;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Converters;
using MorganStanley.ComposeUI.Messaging;
using MorganStanley.ComposeUI.Messaging.Abstractions;

namespace MorganStanley.ComposeUI.Shell.Fdc3.ChannelSelector
{
    internal class ChannelSelectorShellCommunicator : IChannelSelectorCommunicator
    {
        private readonly ILogger<ChannelSelectorShellCommunicator> _logger;
        private readonly IMessageRouter _messageRouter;
        private readonly TimeSpan _defaultTimeout = TimeSpan.FromMinutes(2);

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            Converters = { new AppMetadataJsonConverter() }
        };

        public ChannelSelectorShellCommunicator(
            IMessageRouter messageRouter,
            ILogger<ChannelSelectorShellCommunicator>? logger = null)
        {
            _messageRouter = messageRouter;
            _logger = logger ?? NullLogger<ChannelSelectorShellCommunicator>.Instance;
        }
        
        public async Task<ChannelSelectorResponse?> SendChannelSelectorRequest(string channelId, string instanceId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await SendChannelSelectorRequestCore(channelId, instanceId, cancellationToken);
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

      

        private async Task<ChannelSelectorResponse?> SendChannelSelectorRequestCore(string channelId, string instanceId, CancellationToken cancellationToken = default)
        {
            var request = new ChannelSelectorRequest
            {
                

                ChannelId = channelId,
                InstanceId = instanceId
            };

            /*var responseBuffer = await _messageRouter.InvokeAsync(
                "ComposeUI/fdc3/v2.0/channelSelector",
               
            MessageBuffer.Factory.CreateJson(request, _jsonSerializerOptions),
                cancellationToken: cancellationToken);*/




            await _messageRouter.PublishAsync(
                "ComposeUI/fdc3/v2.0/channelSelector2",
                MessageBuffer.Factory.CreateJson(request, _jsonSerializerOptions),
                cancellationToken: cancellationToken
            );




            /*if (responseBuffer == null)
            {
                return null;
            }*/

            //var response = responseBuffer.ReadJson<ChannelSelectorResponse>(_jsonSerializerOptions);

            return null; // response;
        }










        public async Task<ChannelSelectorResponse?> SendChannelSelectorColorUpdateRequest(JoinUserChannelRequest request, string? color,  CancellationToken cancellationToken = default)
        {
            try
            {
                return await SendChannelSelectorColorUpdateRequestCore(request, color, cancellationToken);
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





        private async Task<ChannelSelectorResponse?> SendChannelSelectorColorUpdateRequestCore(JoinUserChannelRequest req, string color, CancellationToken cancellationToken = default)
        {
            var request = new ChannelSelectorRequest
            {


                ChannelId = req.ChannelId,
                InstanceId = req.InstanceId,
                Color = color
                
            };

            var responseBuffer = await _messageRouter.InvokeAsync(
                "ComposeUI/fdc3/v2.0/channelSelector",

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
