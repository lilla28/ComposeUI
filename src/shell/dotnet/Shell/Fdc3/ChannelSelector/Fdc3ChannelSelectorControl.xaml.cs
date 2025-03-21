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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Contracts;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Protocol;
using static MorganStanley.ComposeUI.Shell.Fdc3.ChannelSelector.Fdc3ChannelSelectorViewModel;

namespace MorganStanley.ComposeUI.Shell.Fdc3.ChannelSelector
{
    /// <summary>
    /// Interaction logic for Fdc3ChannelSelectorControl.xaml
    /// </summary>
    public partial class Fdc3ChannelSelectorControl : UserControl
    {
        private readonly string _instanceId;
        private readonly IChannelSelector _channelSelector;
        private readonly IReadOnlyDictionary<string, ChannelItem> _userChannelSet;
        private readonly Fdc3ChannelSelectorViewModel _viewModel;

        public Fdc3ChannelSelectorControl(
            IChannelSelector channelSelector,
            string color, 
            string instanceId,
            IReadOnlyDictionary<string, ChannelItem> userChannelSet) 
        {          
            _instanceId = instanceId;
            _channelSelector = channelSelector;
            _userChannelSet = userChannelSet;

            _viewModel = new Fdc3ChannelSelectorViewModel(channelSelector, instanceId, _userChannelSet.Select(channel => channel.Value).Select(channel => new UserChannel { Id = channel.Id, ColorCode = channel.DisplayMetadata.Color }), color);
            DataContext = _viewModel;

            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button) sender;
            if (btn.DataContext is UserChannel userChannel)
            {
                var channelNumber = userChannel.Id;
                var colorCode = userChannel.ColorCode;

                await Task.Run(async () =>
                {
                    await _channelSelector.SendChannelSelectorColorUpdateRequestToDesktopAgentClientsAsync(
                        new JoinUserChannelRequest
                        {
                            InstanceId = _instanceId,
                            ChannelId = channelNumber, //TODO send the appropriate channel id
                            Color = colorCode
                        },
                        System.Threading.CancellationToken.None);
                });
            }

            var color = btn.Background;
            ChannelSelector.BorderBrush = color;
        }

        internal Task OnChannelChanged(string channelId)
        {
            if (!_userChannelSet.TryGetValue(channelId, out var channel))
            {
                return Task.CompletedTask;
            }

            if (channel.DisplayMetadata?.Color == null)
            {
                return Task.CompletedTask;
            }

            return _viewModel.UpdateChannelSelectorControlColor(channel.DisplayMetadata.Color);
        }
    }
}
