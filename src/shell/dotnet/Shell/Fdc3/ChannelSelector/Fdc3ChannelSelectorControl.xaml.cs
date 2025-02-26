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

namespace MorganStanley.ComposeUI.Shell.Fdc3.ChannelSelector
{
    /// <summary>
    /// Interaction logic for Fdc3ChannelSelectorControl.xaml
    /// </summary>
    public partial class Fdc3ChannelSelectorControl : UserControl
    {
        private readonly Fdc3ChannelSelectorViewModel? _viewModel;
        public Fdc3ChannelSelectorControl(Fdc3ChannelSelectorViewModel? viewModel)
        {

            _viewModel = viewModel;
            //InitializeComponent();
            
        }




        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button) sender;
            var channelNumber = btn.Content;
            var color = btn.Background;

            BorderBrush = color;
        }
    }
}
