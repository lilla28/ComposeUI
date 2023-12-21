﻿// Morgan Stanley makes this available to you under the Apache License,
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

using System.Reactive.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Contracts;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Converters;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.DependencyInjection;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Exceptions;
using MorganStanley.ComposeUI.Messaging;
using MorganStanley.Fdc3;

namespace MorganStanley.ComposeUI.Fdc3.DesktopAgent.Infrastructure.Internal;

internal class Fdc3DesktopAgentMessageRouterService : IHostedService
{
    private readonly IMessageRouter _messageRouter;
    private readonly IFdc3DesktopAgentBridge _desktopAgent;
    private readonly Fdc3DesktopAgentOptions _options;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<Fdc3DesktopAgentMessageRouterService> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new AppMetadataJsonConverter(),
            new IntentMetadataJsonConverter(),
            new AppIntentJsonConverter(),
            new IconJsonConverter(),
            new ImageJsonConverter(),
            new IntentMetadataJsonConverter(),
        }
    };

    public Fdc3DesktopAgentMessageRouterService(
        IMessageRouter messageRouter,
        IFdc3DesktopAgentBridge desktopAgent,
        IOptions<Fdc3DesktopAgentOptions> options,
        ILoggerFactory? loggerFactory = null)
    {
        _messageRouter = messageRouter;
        _desktopAgent = desktopAgent;
        _options = options.Value;
        _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        _logger = _loggerFactory.CreateLogger<Fdc3DesktopAgentMessageRouterService>() ?? NullLogger<Fdc3DesktopAgentMessageRouterService>.Instance;
    }

    public async ValueTask<UserChannel> HandleAddUserChannel(string id)
    {
        var userChannel = new UserChannel(id, _messageRouter, _loggerFactory.CreateLogger<UserChannel>());
        await _desktopAgent.AddUserChannel(userChannel);

        return userChannel;
    }

    internal ValueTask<MessageBuffer?> HandleFindChannel(string endpoint, MessageBuffer? payload, MessageContext context)
    {
        var request = payload?.ReadJson<FindChannelRequest>();
        if (request?.ChannelType != ChannelType.User)
        {
            return ValueTask.FromResult<MessageBuffer?>(MessageBuffer.Factory.CreateJson(FindChannelResponse.Failure(ChannelError.NoChannelFound), _jsonSerializerOptions));
        }

        return ValueTask.FromResult<MessageBuffer?>(
            MessageBuffer.Factory.CreateJson(
                _desktopAgent.FindChannel(request.ChannelId, request.ChannelType)
                    ? FindChannelResponse.Success
                    : FindChannelResponse.Failure(ChannelError.NoChannelFound), _jsonSerializerOptions));
    }

    internal async ValueTask<MessageBuffer?> HandleFindIntent(string endpoint, MessageBuffer? payload, MessageContext context)
    {
        var request = payload?.ReadJson<FindIntentRequest>(_jsonSerializerOptions);
        if (request == null)
        {
            return MessageBuffer.Factory.CreateJson(FindIntentResponse.Failure(ResolveError.IntentDeliveryFailed), _jsonSerializerOptions);
        }

        var response = await _desktopAgent.FindIntent(request);
        return MessageBuffer.Factory.CreateJson(response, _jsonSerializerOptions);
    }

    internal async ValueTask<MessageBuffer?> HandleFindIntentsByContext(string endpoint, MessageBuffer? payload, MessageContext context)
    {
        var request = payload?.ReadJson<FindIntentsByContextRequest>(_jsonSerializerOptions);
        if (request == null)
        {
            return MessageBuffer.Factory.CreateJson(FindIntentResponse.Failure(ResolveError.IntentDeliveryFailed), _jsonSerializerOptions);
        }

        var response = await _desktopAgent.FindIntentsByContext(request);
        return MessageBuffer.Factory.CreateJson(response, _jsonSerializerOptions);
    }

    internal async ValueTask<MessageBuffer?> HandleRaiseIntent(string endpoint, MessageBuffer? payload, MessageContext context)
    {
        var request = payload?.ReadJson<RaiseIntentRequest>(_jsonSerializerOptions);
        if (request == null)
        {
            return MessageBuffer.Factory.CreateJson(RaiseIntentResponse.Failure(ResolveError.IntentDeliveryFailed), _jsonSerializerOptions);
        }

        try
        {
            var result = await _desktopAgent.RaiseIntent(request);
            if (result.RaiseIntentResolutionMessages.Any())
            {
                foreach (var message in result.RaiseIntentResolutionMessages)
                {
                    await _messageRouter.PublishAsync(
                        Fdc3Topic.RaiseIntentResolution(message.Intent, message.TargetModuleInstanceId),
                        MessageBuffer.Factory.CreateJson(message.Request, _jsonSerializerOptions));
                }
            }

            return MessageBuffer.Factory.CreateJson(result.Response, _jsonSerializerOptions);
        }
        catch (Fdc3DesktopAgentException)
        {
            throw;
        }
    }

    internal async ValueTask<MessageBuffer?> HandleAddIntentListener(string endpoint, MessageBuffer? payload, MessageContext context)
    {
        var request = payload?.ReadJson<IntentListenerRequest>(_jsonSerializerOptions);
        if (request == null)
        {
            return MessageBuffer.Factory.CreateJson(IntentListenerResponse.Failure(Fdc3DesktopAgentErrors.PayloadNull), _jsonSerializerOptions);
        }

        var result = await _desktopAgent.AddIntentListener(request);
        if (!string.IsNullOrEmpty(result.Response.Error))
        {
            return MessageBuffer.Factory.CreateJson(result.Response, _jsonSerializerOptions);
        }

        if (result.RaiseIntentResolutionMessages.Any())
        {
            foreach (var message in result.RaiseIntentResolutionMessages)
            {
                await _messageRouter.PublishAsync(
                    Fdc3Topic.RaiseIntentResolution(message.Intent, message.TargetModuleInstanceId),
                    MessageBuffer.Factory.CreateJson(message.Request, _jsonSerializerOptions));
            }
        }

        return MessageBuffer.Factory.CreateJson(result.Response, _jsonSerializerOptions);
    }

    internal async ValueTask<MessageBuffer?> HandleStoreIntentResult(string endpoint, MessageBuffer? payload, MessageContext context)
    {
        var request = payload?.ReadJson<StoreIntentResultRequest>(_jsonSerializerOptions);
        if (request == null)
        {
            return MessageBuffer.Factory.CreateJson(StoreIntentResultResponse.Failure(ResolveError.IntentDeliveryFailed), _jsonSerializerOptions);
        }

        if (request.TargetFdc3InstanceId == null || request.Intent == null)
        {
            return MessageBuffer.Factory.CreateJson(StoreIntentResultResponse.Failure(ResolveError.IntentDeliveryFailed), _jsonSerializerOptions);
        }

        var response = await _desktopAgent.StoreIntentResult(request);
        return MessageBuffer.Factory.CreateJson(response, _jsonSerializerOptions);
    }

    internal async ValueTask<MessageBuffer?> HandleGetIntentResult(string endpoint, MessageBuffer? payload, MessageContext context)
    {
        var request = payload?.ReadJson<GetIntentResultRequest>(_jsonSerializerOptions);
        if (request == null)
        {
            return MessageBuffer.Factory.CreateJson(GetIntentResultResponse.Failure(ResolveError.IntentDeliveryFailed), _jsonSerializerOptions);
        }

        if (request.TargetAppIdentifier?.InstanceId == null || request.Intent == null || request.MessageId == null)
        {
            return MessageBuffer.Factory.CreateJson(GetIntentResultResponse.Failure(ResolveError.IntentDeliveryFailed), _jsonSerializerOptions);
        }

        var response = await _desktopAgent.GetIntentResult(request);
        return MessageBuffer.Factory.CreateJson(response, _jsonSerializerOptions);
    }

    private async ValueTask DisposeSafeAsync(IEnumerable<ValueTask> tasks)
    {
        foreach (var task in tasks)
        {
            try
            {
                await task;
            }
            catch (Exception exception)
            {
                _logger.LogWarning($"Could not dispose task: {task.IsCompleted}. Exception: {exception}");
            }
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _messageRouter.RegisterServiceAsync(Fdc3Topic.FindChannel, HandleFindChannel, cancellationToken: cancellationToken);
        await _messageRouter.RegisterServiceAsync(Fdc3Topic.FindIntent, HandleFindIntent, cancellationToken: cancellationToken);
        await _messageRouter.RegisterServiceAsync(Fdc3Topic.FindIntentsByContext, HandleFindIntentsByContext, cancellationToken: cancellationToken);
        await _messageRouter.RegisterServiceAsync(Fdc3Topic.RaiseIntent, HandleRaiseIntent, cancellationToken: cancellationToken);
        await _messageRouter.RegisterServiceAsync(Fdc3Topic.GetIntentResult, HandleGetIntentResult, cancellationToken: cancellationToken);
        await _messageRouter.RegisterServiceAsync(Fdc3Topic.SendIntentResult, HandleStoreIntentResult, cancellationToken: cancellationToken);
        await _messageRouter.RegisterServiceAsync(Fdc3Topic.AddIntentListener, HandleAddIntentListener, cancellationToken: cancellationToken);

        await _desktopAgent.StartAsync(cancellationToken);

        if (_options.ChannelId != null)
        {
            await HandleAddUserChannel(_options.ChannelId);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var unregisteringTasks = new ValueTask[]
        {
                _messageRouter.UnregisterServiceAsync(Fdc3Topic.FindChannel, cancellationToken),
                _messageRouter.UnregisterServiceAsync(Fdc3Topic.FindIntent, cancellationToken),
                _messageRouter.UnregisterServiceAsync(Fdc3Topic.RaiseIntent, cancellationToken),
                _messageRouter.UnregisterServiceAsync(Fdc3Topic.FindIntentsByContext, cancellationToken),
                _messageRouter.UnregisterServiceAsync(Fdc3Topic.GetIntentResult, cancellationToken),
                _messageRouter.UnregisterServiceAsync(Fdc3Topic.SendIntentResult, cancellationToken),
                _messageRouter.UnregisterServiceAsync(Fdc3Topic.AddIntentListener, cancellationToken)
        };

        await DisposeSafeAsync(unregisteringTasks);
        await _desktopAgent.StopAsync(cancellationToken);
    }
}