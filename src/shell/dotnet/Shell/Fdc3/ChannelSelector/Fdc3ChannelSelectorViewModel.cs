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


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Finos.Fdc3;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Threading;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Protocol;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Contracts;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Converters;


namespace MorganStanley.ComposeUI.Shell.Fdc3.ChannelSelector
{
    public class Fdc3ChannelSelectorViewModel : INotifyPropertyChanged, IChannelSelectorShellCommunicator
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand SelectCurrentChannelCommand { get; }


        private string _instanceId;
        private ICommand? _joinChannelCommand;        
        private IUserChannelSetReader _userChannelSetReader;
        private IEnumerable<ChannelItem> _channelSet;

        private readonly List<ChannelSelectorAppData> _appData = [];
        private readonly object _disposeLock = new();
        private readonly List<Func<ValueTask>> _disposeTask = new();

        private string? _channelId;
        private string? _color;
        private IDesktopAgent _desktopAgent;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            Converters = { new AppMetadataJsonConverter(), new IconJsonConverter() }
        };

        public Fdc3ChannelSelectorViewModel(
            IChannelSelector channelSelector,
            string instanceId = "", 
            string color = "Gray")
        {
            _instanceId = instanceId;

            SetCurrentColor(_currentChannelColor);
            SetCurrentChannelColorCommand = new RelayCommand(SetCurrentChannelColor);
            if (color != null)
            {
                var brushColor = GetBrushForColor(color);

                SetCurrentColor(brushColor);
            }
        }

        private Brush GetBrushForColor(string color)
        {
            var myColor = (Color) ColorConverter.ConvertFromString(color);
            SolidColorBrush brush = new SolidColorBrush(myColor);

            return brush;
        }


        public ICommand SetCurrentChannelColorCommand { get; }

        private void SetCurrentColor(Brush color)
        {
            CurrentChannelColor = color;
            OnPropertyChanged(nameof(CurrentChannelColor));
        }

        private void SetCurrentChannelColor()
        {
            CurrentChannelColor = _currentChannelColor;
            SetCurrentColor(_currentChannelColor);
        }

        private Brush _currentChannelColor = new SolidColorBrush(Colors.Gray);

         public Brush CurrentChannelColor
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
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
         }


        public ValueTask<ChannelSelectorResponse> UpdateChannelSelectorColor(string color)
        {
           var brushColor = GetBrushForColor(color);

            SetCurrentColor(brushColor);


            return new ValueTask<ChannelSelectorResponse>(); //todo replace fake return value

        }

        public async Task<ChannelSelectorResponse?> SendChannelSelectorColorUpdateRequestToTheUI(
            JoinUserChannelRequest req, 
            string? color, 
            CancellationToken cancellationToken = default)
        {
            //TODO: Have a better way to handle the request?
            var response = await UpdateChannelSelectorColor(color);

            return response;
        }
    }
}
