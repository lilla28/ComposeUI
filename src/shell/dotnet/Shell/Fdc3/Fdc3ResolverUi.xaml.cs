// Morgan Stanley makes this available to you under the Apache License,
// Version 2.0 (the "License"). You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0.
// 
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership. Unless required by applicable law or agreed
// to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
// or implied. See the License for the specific language governing permissions
// and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Finos.Fdc3;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MorganStanley.ComposeUI.Shell.Fdc3;

/// <summary>
/// Interaction logic for Fdc3ResolverUi.xaml
/// </summary>
public partial class Fdc3ResolverUi : Window, IDisposable
{
    private ILogger<Fdc3ResolverUi> _logger;
    private readonly CancellationTokenSource _userCancellationTokenSource;

    public IAppMetadata? AppMetadata { get; private set; }
    internal CancellationToken UserCancellationToken => _userCancellationTokenSource.Token;
    
    //TODO: When closing via the Close button
    public Fdc3ResolverUi(
        IEnumerable<IAppMetadata> apps,
        ILogger<Fdc3ResolverUi>? logger = null)
    {
        _userCancellationTokenSource = new();
        _logger = logger ?? NullLogger<Fdc3ResolverUi>.Instance;

        InitializeComponent();
        ResolverUiDataSource.ItemsSource = apps;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        if (AppMetadata == null)
        {
            _userCancellationTokenSource.Cancel();
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        _userCancellationTokenSource.Cancel();
        Close();
    }

    private void ResolverUiDataSource_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (ResolverUiDataSource.SelectedItem != null)
        {
            AppMetadata = (IAppMetadata) ResolverUiDataSource.SelectedItem;
            Close();
        }
    }

    public void Dispose()
    {
        _userCancellationTokenSource.Cancel();
        _userCancellationTokenSource.Dispose();
    }
}
