using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Finos.Fdc3;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;


namespace MorganStanley.ComposeUI.Shell.Fdc3.ChannelSelector
{
    public class Fdc3ChannelSelectorViewModel : INotifyPropertyChanged
    {
        public IChannelSelectorCommunicator ChannelSelector;
        private ICommand? _joinChannelCommand;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand SelectCurrentChannelCommand { get; }


        public Fdc3ChannelSelectorViewModel(IChannelSelectorCommunicator channelSelector)
        {
            ChannelSelector = channelSelector;

            //SelectCurrentChannelCommand = new RelayCommand(SelectCurrentChannelClick);


            if (channelSelector == null)
                throw new ArgumentNullException("channelSelector");
        }

        private Color _currentChannelColor = Colors.Gray;

        public Color CurrentChannelColor
        {
            get => _currentChannelColor;
            set
            {
                _currentChannelColor = value;
                OnPropertyChanged(nameof(CurrentChannelColor));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public IChannel? SelectedChannel { get; set; }

        //Todo: Implement Command
        public ICommand JoinChannelCommand
        {
            get
            {
                return _joinChannelCommand ?? (_joinChannelCommand = new RelayCommand(() =>
                {
                }));
            }
        }

    }

}
