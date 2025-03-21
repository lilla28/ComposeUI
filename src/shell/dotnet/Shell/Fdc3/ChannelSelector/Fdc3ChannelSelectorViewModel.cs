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
using MorganStanley.ComposeUI.Fdc3.DesktopAgent;
using System.Windows.Media;
using System.ComponentModel;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Protocol;
using System.Text.Json;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Converters;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Collections.Generic;


namespace MorganStanley.ComposeUI.Shell.Fdc3.ChannelSelector
{
    public class Fdc3ChannelSelectorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
       
        private readonly string _fdc3InstanceId;
        private ObservableCollection<UserChannel> _userChannelSet;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            Converters = { new AppMetadataJsonConverter(), new IconJsonConverter() }
        };

        public Fdc3ChannelSelectorViewModel(
            IChannelSelector channelSelector,
            string fdc3InstanceId,
            IEnumerable<UserChannel> channels,
            string color = "Gray")
        {
            _fdc3InstanceId = fdc3InstanceId;
            CurrentChannelColor = _currentChannelColor;

            if (color != null)
            {
                var brushColor = GetBrushForColor(color);

                CurrentChannelColor = brushColor;
            }

            UserChannelSet = new ObservableCollection<UserChannel>(channels);
        }

        private Brush GetBrushForColor(string color)
        {
            var myColor = (Color) ColorConverter.ConvertFromString(color);
            SolidColorBrush brush = new SolidColorBrush(myColor);

            return brush;
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

        public ObservableCollection<UserChannel> UserChannelSet
        {
            get => _userChannelSet;
            set
            {
                _userChannelSet = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public Task UpdateChannelSelectorControlColor(string color)
        {
            var brushColor = GetBrushForColor(color);
            CurrentChannelColor = brushColor;
            return Task.CompletedTask;
        }

        public class UserChannel
        {
            public string Id { get; set; }
            public string? ColorCode { get; set; }
        }
    }
}
