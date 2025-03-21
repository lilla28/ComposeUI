// /*
//  * Morgan Stanley makes this available to you under the Apache License,
//  * Version 2.0 (the "License"). You may obtain a copy of the License at
//  *
//  *      http://www.apache.org/licenses/LICENSE-2.0.
//  *
//  * See the NOTICE file distributed with this work for additional information
//  * regarding copyright ownership. Unless required by applicable law or agreed
//  * to in writing, software distributed under the License is distributed on an
//  * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
//  * or implied. See the License for the specific language governing permissions
//  * and limitations under the License.
//  */

using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Contracts;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Converters;
using MorganStanley.ComposeUI.Messaging;
using MorganStanley.ComposeUI.Messaging.Abstractions;

namespace MorganStanley.ComposeUI.Shell.Fdc3.ChannelSelector;

internal class ComposeUIChannelSelector : IChannelSelector
{
    private readonly IMessageRouter _messageRouter;
    private readonly ILogger<ComposeUIChannelSelector> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new AppMetadataJsonConverter(), new IconJsonConverter() }
    };

    public ComposeUIChannelSelector(
        IMessageRouter messageRouter,
        ILogger<ComposeUIChannelSelector>? logger = null)
    {
        _messageRouter = messageRouter;
        _logger = logger ?? NullLogger<ComposeUIChannelSelector>.Instance;
    }

    public async Task<JoinUserChannelResponse?> SendChannelSelectorColorUpdateRequestToDesktopAgentClientsAsync(
        JoinUserChannelRequest request, 
        string? color, 
        CancellationToken cancellationToken = default)
    {
        var responseBuffer = await _messageRouter.InvokeAsync(
            $"ComposeUI/fdc3/v2.0/channelSelector-{request.InstanceId}",
            MessageBuffer.Factory.CreateJson(request, _jsonSerializerOptions),
            cancellationToken: cancellationToken);

        //TODO: do verification of the succness of the response
        var response = responseBuffer?.ReadJson<JoinUserChannelResponse>(_jsonSerializerOptions);


        return null;
    }
}
