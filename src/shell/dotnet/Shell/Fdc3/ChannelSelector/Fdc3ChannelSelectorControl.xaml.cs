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

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Contracts;

namespace MorganStanley.ComposeUI.Shell.Fdc3.ChannelSelector
{
    /// <summary>
    /// Interaction logic for Fdc3ChannelSelectorControl.xaml
    /// </summary>
    public partial class Fdc3ChannelSelectorControl : UserControl
    {
        private readonly Fdc3ChannelSelectorViewModel _viewModel;
        private readonly string _instanceId;
        private readonly IChannelSelector _channelSelector;

        public Fdc3ChannelSelectorControl(
            IChannelSelector channelSelector,
            string color, 
            string instanceId) 
        {          
            //_viewModel = new Fdc3ChannelSelectorViewModel(channelSelector, instanceId, color);
            _instanceId = instanceId;
            _channelSelector = channelSelector;
            DataContext = new Fdc3ChannelSelectorViewModel(channelSelector, instanceId, color);
            
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button) sender;
            var channelNumber = (string)btn.Content;
            var color = btn.Background;

            ChannelSelector.BorderBrush = color;

            await Task.Run(async () => await _channelSelector.SendChannelSelectorColorUpdateRequestToDesktopAgentClientsAsync(
                new JoinUserChannelRequest
                {
                    InstanceId = _instanceId,
                    ChannelId = channelNumber //TODO send the appropriate channel id
                },
                color.ToString(),
                System.Threading.CancellationToken.None));
        }
    }
}
