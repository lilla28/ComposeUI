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

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Finos.Fdc3;

namespace MorganStanley.ComposeUI.DesktopAgent.ResolverUI.WPF;

public partial class ResolverUI : Window, IDisposable
{
    public IAppMetadata? SelectedAppMetadata { get; private set; }
    internal CancellationToken CancellationToken => _cancellationTokenSource.Token;

    private readonly CancellationTokenSource _cancellationTokenSource;

    public ResolverUI(IEnumerable<IAppMetadata> appMetadata)
    {
        InitializeComponent();
        AppMetadataList.ItemsSource = appMetadata;
        _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1));
    }

    private void SelectAppMetadata(object sender, MouseButtonEventArgs e)
    {
        if (AppMetadataList.SelectedItem != null)
        {
            SelectedAppMetadata = (IAppMetadata)AppMetadataList.SelectedItem;
            _cancellationTokenSource.Cancel();
            Close();
        }
    }

    private void OnCancelButtonClick(object sender, RoutedEventArgs e)
    {
        _cancellationTokenSource.Cancel();
        Close();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        _cancellationTokenSource.Cancel();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        Dispose();
    }

    public static IAppMetadata? UseResolverUI(
        IEnumerable<IAppMetadata> apps)
    {
        //Determine the owner application
        Window? owner = null;

        Application.Current.Dispatcher.Invoke(() =>
        {
            owner =
                //First window which is active
                Application.Current.Windows
                    .Cast<Window>()
                    .FirstOrDefault(window => window.IsActive) ??

                //OR the first window which is visible
                Application.Current.Windows
                    .Cast<Window>()
                    .FirstOrDefault(window => window.Visibility == Visibility.Visible);
        });

        var dispatcher = owner == null
            ? Application.Current.Dispatcher
            : owner.Dispatcher;

        ResolverUI? resolverUI = null;
        dispatcher.Invoke(() =>
        {
            if (Application.Current.Dispatcher == null
                || Application.Current.Dispatcher.HasShutdownStarted
                || Application.Current.Dispatcher.HasShutdownFinished)
            {
                return;
            }

            resolverUI = new ResolverUI(apps);
            resolverUI.Show();
        });

        while (resolverUI?.SelectedAppMetadata == null
            && !resolverUI!.CancellationToken.IsCancellationRequested)
        {
            continue;
        }

        return resolverUI?.SelectedAppMetadata;
    }

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
    }
}
