using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent;

namespace MorganStanley.ComposeUI.Shell.Fdc3.ChannelSelector
{
    /// <summary>
    /// Interaction logic for Fdc3ChannelSelectorControl.xaml
    /// </summary>
    public partial class Fdc3ChannelSelectorControl : UserControl
    {
        private readonly Fdc3ChannelSelectorViewModel? _viewModel;
        private string _instanceId;

        //public Color CurrentChannelColor;



        public Fdc3ChannelSelectorControl(IChannelSelectorCommunicator channelSelectorCommunicator, string color, string instanceId) {          
            _viewModel = new Fdc3ChannelSelectorViewModel(channelSelectorCommunicator, color);
            _instanceId = instanceId;
            DataContext = _viewModel;

            InitializeComponent();
        }

       

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button) sender;
            var channelNumber = (string)btn.Content;
            var color = btn.Background;
            await Task.Run(() =>
            {
                _viewModel.ChannelSelector.SendChannelSelectorRequest(channelNumber, _instanceId); //todo instanceid
            });
            ChannelSelector.BorderBrush = color;

            
            
            //this is just for proving we can connect to the DA, and switch channels. Once this works, we populate this with the selected channel ID


            //_desktopAgent.JoinUserChannel("fdc3.channel."+channelNumber );
        }
    }
}
