using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finos.Fdc3;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Contracts;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Protocol;

namespace MorganStanley.ComposeUI.Fdc3.DesktopAgent
{
    public interface IChannelSelectorDACommunicator
    {
        //public void SendChannelSelectorColorUpdateRequest(string? color);
        //public Task<ChannelSelectorResponse?> SendChannelSelectorColorUpdateRequest(JoinUserChannelRequest request, string? color, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends request for the shell to show a window for the user to select the wished intent for resolving the RaiseIntentForContext.
        /// </summary>
        /// <param name="intents"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        // public Task<ResolverUIIntentResponse?> SendResolverUIIntentRequest(IEnumerable<string> intents, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a request for the shell to show a window, aka ResolverUI, with the appropriate AppMetadata that can solve the raised intent.
        /// </summary>
        /// <param name="appMetadata"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ChannelSelectorResponse?> SendChannelSelectorRequest(string channelId, string instanceId, CancellationToken cancellationToken = default);

        //public void UpdateChannelColor(string? channelColor);
        //public void /*Task<ChannelSelectorResponse?>*/ SendChannelSelectorColorUpdateRequest(JoinUserChannelRequest request, string? color, CancellationToken cancellationToken = default);

        public Task<ChannelSelectorResponse?> SendChannelSelectorColorUpdateRequest(JoinUserChannelRequest req, string? color, CancellationToken cancellationToken = default);
    }
}
