﻿/*
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

using System.Collections.Concurrent;
using Finos.Fdc3;
using Finos.Fdc3.Context;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Client.Infrastructure.Internal;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Shared.Exceptions;
using MorganStanley.ComposeUI.Messaging.Abstractions;

namespace MorganStanley.ComposeUI.Fdc3.DesktopAgent.Client.Infrastructure;

internal class DesktopAgentClient : IDesktopAgent
{
    private readonly IMessaging _messaging;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<DesktopAgentClient> _logger;
    private readonly string _appId;
    private readonly string _instanceId;
    private readonly IChannelFactory _channelFactory;
    private readonly IMetadataClient _metadataClient;
    private readonly IntentsClient _intentsClient;
    private readonly ConcurrentDictionary<IListener, Func<string, ChannelType, CancellationToken, ValueTask>> _contextListenersWithSubscriptionLastContextHandlingActions = new();

    private IChannel? _currentChannel;
    private readonly SemaphoreSlim _currentChannelLock = new(1, 1);

    private readonly ConcurrentDictionary<string, IChannel> _userChannels = new();
    private readonly ConcurrentDictionary<string, IChannel> _appChannels = new();

    public DesktopAgentClient(
        IMessaging messaging,
        ILoggerFactory? loggerFactory = null)
    {
        _messaging = messaging;
        _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        _logger = _loggerFactory.CreateLogger<DesktopAgentClient>();

        _appId = Environment.GetEnvironmentVariable(nameof(AppIdentifier.AppId)) ?? throw ThrowHelper.MissingAppId(string.Empty);
        _instanceId = Environment.GetEnvironmentVariable(nameof(AppIdentifier.InstanceId)) ?? throw ThrowHelper.MissingInstanceId(_appId, string.Empty);

        _channelFactory = new ChannelFactory(_messaging, _instanceId, _loggerFactory);
        _metadataClient = new MetadataClient(_appId, _instanceId, _messaging, _loggerFactory.CreateLogger<MetadataClient>());
        _intentsClient = new IntentsClient(_messaging, _instanceId, _loggerFactory.CreateLogger<IntentsClient>());
    }

    //TODO: AddContextListener should be revisited when the Open is being implemented as the first context that should be handled is the context which is passed through the fdc3.open call.
    public async Task<IListener> AddContextListener<T>(string? contextType, ContextHandler<T> handler) where T : IContext
    {
        ContextListener<T> listener = null;
        try
        {
            await _currentChannelLock.WaitAsync().ConfigureAwait(false);

            listener = await _channelFactory.CreateContextListener(handler, _currentChannel, contextType);
            if (!_contextListenersWithSubscriptionLastContextHandlingActions.TryAdd(
                listener, 
                async(channelId, channelType, cancellationToken) => 
                { 
                    await listener.SubscribeAsync(channelId, channelType, cancellationToken); 
                    await HandleLastContextAsync(listener); 
                }))
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Failed to add context listener to the internal collection.");
                }
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"Context listener added for context type: {contextType ?? "null"}.");
            }

            return listener;
        }
        finally
        {
            await HandleLastContextAsync(listener);

            _currentChannelLock.Release();
        }
    }

    public Task<IListener> AddIntentListener<T>(string intent, IntentHandler<T> handler) where T : IContext
    {
        throw new NotImplementedException();
    }

    public async Task Broadcast(IContext context)
    {
        try
        {
            await _currentChannelLock.WaitAsync().ConfigureAwait(false);

            if (_currentChannel == null)
            {
                throw ThrowHelper.ClientNotConnectedToUserChannel();
            }

            await _currentChannel.Broadcast(context);
        }
        finally
        {
            _currentChannelLock.Release();
        }
    }

    public Task<IPrivateChannel> CreatePrivateChannel()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<IAppIdentifier>> FindInstances(IAppIdentifier app)
    {
        var instances = await _metadataClient.FindInstancesAsync(app);
        return instances;
    }

    public async Task<IAppIntent> FindIntent(string intent, IContext? context = null, string? resultType = null)
    {
        var result = await _intentsClient.FindIntentAsync(intent, context, resultType);
        return result;
    }

    public async Task<IEnumerable<IAppIntent>> FindIntentsByContext(IContext context, string? resultType = null)
    {
        var appIntents = await _intentsClient.FindIntentsByContextAsync(context, resultType);
        return appIntents;
    }

    public async Task<IAppMetadata> GetAppMetadata(IAppIdentifier app)
    {
        var appMetadata = await _metadataClient.GetAppMetadataAsync(app);
        return appMetadata;
    }

    public Task<IChannel?> GetCurrentChannel()
    {
        return Task.FromResult(_currentChannel);
    }

    public async Task<IImplementationMetadata> GetInfo()
    {
        var implementationMetadata = await _metadataClient.GetInfoAsync();
        return implementationMetadata;
    }

    public async Task<IChannel> GetOrCreateChannel(string channelId)
    {
        if (_appChannels.TryGetValue(channelId, out var existingChannel))
        {
            return existingChannel;
        }

        var channel = await _channelFactory.CreateAppChannelAsync(channelId);

        if (!_appChannels.TryAdd(channelId, channel))
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning($"Failed to add app channel to the internal collection: {channelId}.");
            }
        }

        return channel;
    }

    public async Task<IEnumerable<IChannel>> GetUserChannels()
    {
        var channels = await _channelFactory.GetUserChannelsAsync();
        return channels;
    }

    public async Task JoinUserChannel(string channelId)
    {
        try
        {
            await _currentChannelLock.WaitAsync().ConfigureAwait(false);

            if (_currentChannel != null)
            {
                _logger.LogInformation($"Leaving current channel: {_currentChannel.Id}...");
                await LeaveCurrentChannel().ConfigureAwait(false);
            }

            if (!_userChannels.TryGetValue(channelId, out var channel))
            {
                channel = await _channelFactory.JoinUserChannelAsync(channelId).ConfigureAwait(false);
                _userChannels[channelId] = channel;
            }

            _currentChannel = channel;
        }
        finally
        {
            var contextListeners = _contextListenersWithSubscriptionLastContextHandlingActions.Reverse();
            foreach (var contextListener in contextListeners)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"Subscribing context listener to channel: {_currentChannel?.Id}...");
                }

                await contextListener.Value(_currentChannel!.Id, _currentChannel!.Type, CancellationToken.None);
            }

            _currentChannelLock.Release();
        }
    }

    public async Task LeaveCurrentChannel()
    {
        try
        {
            await _currentChannelLock.WaitAsync().ConfigureAwait(false);
            var contextListeners = _contextListenersWithSubscriptionLastContextHandlingActions.Reverse();

            //The context listeners, that have been added through the `fdc3.addContextListener()` should unsubscribe, but the context listeners should remain registered to the DesktopAgentClient instance.
            foreach (var contextListener in contextListeners)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"Unsubscribing context listener from channel: {_currentChannel?.Id}...");
                }

                contextListener.Key.Unsubscribe();
            }

            _currentChannel = null;
        }
        finally
        {
            _currentChannelLock.Release();
        }
    }

    public Task<IAppIdentifier> Open(IAppIdentifier app, IContext? context = null)
    {
        throw new NotImplementedException();
    }

    public Task<IIntentResolution> RaiseIntent(string intent, IContext context, IAppIdentifier? app = null)
    {
        throw new NotImplementedException();
    }

    public Task<IIntentResolution> RaiseIntentForContext(IContext context, IAppIdentifier? app = null)
    {
        throw new NotImplementedException();
    }

    private async Task HandleLastContextAsync<T>(
        ContextListener<T>? listener = null)
        where T : IContext
    {
        if (listener == null)
        {
            return;
        }

        if (_currentChannel == null)
        {
            return;
        }

        var lastContext = await _currentChannel.GetCurrentContext(listener.ContextType);

        if (lastContext == null)
        {
            _logger.LogDebug(@$"No last context of type: {listener.ContextType ?? "null"} found in channel: {_currentChannel.Id}.");
            return;
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug($"Invoking context handler for the last context of type: {listener.ContextType ?? "null"}.");
        }

        await listener.HandleContextAsync(lastContext);
    }
}