﻿/*
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
using MorganStanley.ComposeUI.Shell.Fdc3.ResolverUI.Pages;
using System.Threading;
using System.Windows.Controls;
using MorganStanley.ComposeUI.Shell.Fdc3.ResolverUI;
using Finos.Fdc3.AppDirectory;
using System.Threading.Channels;


namespace MorganStanley.ComposeUI.Shell.Fdc3.ChannelSelector
{
    public class Fdc3ChannelSelectorViewModel : INotifyPropertyChanged
    {
        public IChannelSelectorCommunicator ChannelSelector;
        public string InstanceId;
        private ICommand? _joinChannelCommand;

        private readonly List<ChannelSelectorAppData> _appData = [];

        public event PropertyChangedEventHandler? PropertyChanged;
        private IUserChannelSetReader _userChannelSetReader;

        public ICommand SelectCurrentChannelCommand { get; }


        public Fdc3ChannelSelectorViewModel(IChannelSelectorCommunicator channelSelector, string color)
        {
            ChannelSelector = channelSelector;

            //SelectCurrentChannelCommand = new RelayCommand(SelectCurrentChannelClick);


            if (channelSelector == null)
            {
                throw new ArgumentNullException("channelSelector");
            }

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
                 PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
         }


        public ICommand UpdateChannelColorCommand = new RelayCommand(() => {
            /*foreach (var item in _appData)
            {
                
                //item.AppMetadata.InstanceId
                var foo = item.DisplayMetadata.Color;
            }*/
        });
        


        /*
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
         }*/

    }

}
