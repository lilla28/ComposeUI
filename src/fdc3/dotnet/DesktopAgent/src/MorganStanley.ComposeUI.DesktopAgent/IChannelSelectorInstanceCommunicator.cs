using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Contracts;
using Windows.Media.AppBroadcasting;

namespace MorganStanley.ComposeUI.Fdc3.DesktopAgent
{
    public interface IChannelSelectorInstanceCommunicator
    {
        public Task RegisterMessageRouterForInstance(string instanceId);

        public void InvokeColorUpdate(ChannelSelectorRequest request, CancellationToken cancellationToken = default);
        public void InvokeChannelSelectorRequest(ChannelSelectorRequest request, CancellationToken cancellationToken = default);


    }
}
