using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Contracts;

namespace MorganStanley.ComposeUI.Fdc3.DesktopAgent
{
    public interface IChannelSelector
    {
        //ChannelSelectorRequest
        public Task<ChannelSelectorResponse?> SendChannelSelectorColorUpdateRequest(JoinUserChannelRequest req, string? color, CancellationToken cancellationToken = default);
        public Task<ChannelSelectorResponse?> SendChannelSelectorRequest(string channelId, string instanceId, CancellationToken cancellationToken = default);
    }
}
