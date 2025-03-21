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

using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Contracts;

namespace MorganStanley.ComposeUI.Fdc3.DesktopAgent;

public delegate Task ChannelChangedEventHandlerAsync(string channelId);
public interface IChannelSelector
{
    //TODO: Better to have different request/response types
    public Task<JoinUserChannelResponse?> SendChannelSelectorColorUpdateRequestToDesktopAgentClientsAsync(
        JoinUserChannelRequest request, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Handles the channel change event derived from the joinUserChannel FDC3 client call.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task RegisterChannelChangedServiceAsync(
        string instanceId,
        ChannelChangedEventHandlerAsync channelChangedEventHandlerAsync,
        CancellationToken cancellationToken = default);
}
