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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Infrastructure.Internal;

namespace MorganStanley.ComposeUI.Fdc3.DesktopAgent.Infrastructure;

internal class Fdc3MessagingFactory : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Fdc3DesktopAgentOptions _options;
    private IHostedService? _messagingLayer;
    private readonly ILogger<Fdc3MessagingFactory> _logger;

    public Fdc3MessagingFactory(
        IServiceProvider serviceProvider,
        IOptions<Fdc3DesktopAgentOptions> options,
        ILogger<Fdc3MessagingFactory>? logger)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _logger = logger ?? NullLogger<Fdc3MessagingFactory>.Instance;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        switch (_options.MessagingOption)
        {
            case Fdc3MessagingOption.MessageRouter:
                _messagingLayer =
                    ActivatorUtilities.CreateInstance<Fdc3DesktopAgentMessageRouterService>(_serviceProvider);

                break;
        }

        if (_messagingLayer == null)
        {
            return;
        }
        
        await _messagingLayer.StartAsync(cancellationToken);

        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace($"Setting up messaging layer for {nameof(Fdc3DesktopAgent)} succeeded.");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_messagingLayer != null)
        {
            await _messagingLayer.StopAsync(cancellationToken);
        }
    }
}