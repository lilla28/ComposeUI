using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Finos.Fdc3;


namespace MorganStanley.ComposeUI.Shell.Fdc3.ChannelSelector
{
    public class Fdc3ChannelSelectorViewModel
    {
        private readonly IDesktopAgent _desktopAgent;
        private ICommand? _joinChannelCommand;


        public Fdc3ChannelSelectorViewModel(IDesktopAgent desktopAgent)
        {
            _desktopAgent = desktopAgent;
        }

        public IEnumerable<IChannel>? AvailableChannels
        {
            get { return _desktopAgent?.GetUserChannels()?.Result; }
        }

        public IChannel? SelectedChannel { get; set; }

        /*public ICommand JoinChannelCommand
        {
            get
            {
                return _joinChannelCommand ?? (_joinChannelCommand = new DelegateCommand(() =>
                {
                    if (this.SelectedChannel != null)
                    {
                        _desktopAgent.JoinUserChannel(this.SelectedChannel.Id);
                    }
                }));
            }
        }*/

    }

}
