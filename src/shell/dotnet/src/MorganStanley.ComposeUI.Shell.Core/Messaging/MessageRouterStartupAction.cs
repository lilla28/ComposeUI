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

using System.Text.Json;
using MorganStanley.ComposeUI.Messaging.Client.WebSocket;
using MorganStanley.ComposeUI.Messaging.Server.WebSocket;
using MorganStanley.ComposeUI.ModuleLoader;

namespace MorganStanley.ComposeUI.Shell.Core.Messaging;

internal sealed class MessageRouterStartupAction(
    IMessageRouterWebSocketServer? webSocketServer = null,
    string? accessToken = null) : IStartupAction
{
    private readonly IMessageRouterWebSocketServer? _webSocketServer = webSocketServer;
    private readonly string? _accessToken = accessToken;

    public Task InvokeAsync(StartupContext startupContext, Func<Task> next)
    {
        if (_webSocketServer == null || _accessToken == null)
        {
            return next();
        }

        if (startupContext.ModuleInstance.Manifest.ModuleType == ModuleType.Web)
        {
            var webProperties = startupContext.GetOrAddProperty<WebStartupProperties>();

            webProperties.ScriptProviders.Add(
                _ => new ValueTask<string>(
                    $$"""
                            window.composeui = {
                                ...window.composeui,
                                messageRouterConfig: {
                                    accessToken: "{{JsonEncodedText.Encode(_accessToken)}}",
                                    webSocket: {
                                        url: "{{_webSocketServer.WebSocketUrl}}"
                                    }
                                }
                            };
                            """));
        }

        startupContext.AddProperty(new EnvironmentVariables([ new KeyValuePair<string, string>(WebSocketEnvironmentVariableNames.Uri, _webSocketServer.WebSocketUrl.AbsoluteUri),
            new KeyValuePair<string, string>(ComposeUI.Messaging.EnvironmentVariableNames.AccessToken, _accessToken)]));

        return next();
    }
}