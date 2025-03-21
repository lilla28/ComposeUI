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

namespace MorganStanley.ComposeUI.Fdc3.DesktopAgent.Infrastructure.Internal;

/// <summary>
/// This communicator is used to send the updates about the channel selector window to the bridge to send updates to the joined clients.
/// </summary>
internal interface IChannelSelectorBridgeCommunicator
{
    /// <summary>
    /// Registers service for changing user channels which was derived from the shell UI control.
    /// </summary>
    /// <param name="instanceId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task RegisterChannelChangedServiceAsync(string instanceId, CancellationToken cancellationToken = default);
}
