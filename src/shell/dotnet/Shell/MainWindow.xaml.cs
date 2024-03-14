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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls.Ribbon;
using CommunityToolkit.Mvvm.ComponentModel;
using MorganStanley.ComposeUI.ModuleLoader;
using MorganStanley.ComposeUI.Shell.ImageSource;
using MorganStanley.ComposeUI.Shell.Utilities;
using IconUtilities = MorganStanley.ComposeUI.Shell.Utilities.IconUtilities;
using Infragistics.Windows.DockManager;
using System;
using System.Windows.Controls;
using System.ComponentModel;
using Infragistics.Windows.DockManager.Events;

namespace MorganStanley.ComposeUI.Shell;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : RibbonWindow
{
    private readonly IModuleLoader _moduleLoader;
    private readonly IModuleCatalog _moduleCatalog;
    private readonly ImageSourceProvider _iconProvider;
    private readonly XamDockManager _xamDockManager;

    public MainWindow(
        IModuleCatalog moduleCatalog,
        IModuleLoader moduleLoader,
        IImageSourcePolicy? imageSourcePolicy = null)
    {
        _moduleCatalog = moduleCatalog;
        _moduleLoader = moduleLoader;
        _iconProvider = new ImageSourceProvider(imageSourcePolicy ?? new DefaultImageSourcePolicy());
        _xamDockManager = new XamDockManager();

        InitializeComponent();
    }

    private async void RibbonWindow_Initialized(object sender, System.EventArgs e)
    {
        var moduleIds = await _moduleCatalog.GetModuleIds();

        var modules = new List<ModuleViewModel>();
        foreach (var moduleId in moduleIds)
        {
            var manifest = await _moduleCatalog.GetManifest(moduleId);
            modules.Add(new ModuleViewModel(manifest, _iconProvider));
        }

        ViewModel = new MainWindowViewModel
        {
            Modules = new ObservableCollection<ModuleViewModel>(modules)
        };

        _documentContentHost = new DocumentContentHost();
        _xamDockManager.Content = _documentContentHost;
        layoutRoot.Children.Add(_xamDockManager);
    }

    private static int _number = 0;

    public void CreateDockableWindow(WebWindow webWindow)
    {
        var splitPane = new SplitPane()
        {
            Name = "testSplitPane" + _number
        };

        var contentPane = new WebWindowContentPane(webWindow, _moduleLoader)
        {
            Name = "contentPane" + _number
        };

        _number++;

        splitPane.Panes.Add(contentPane);

        XamDockManager.SetInitialLocation(splitPane, InitialPaneLocation.DockableFloating);

        _xamDockManager.Panes.Add(splitPane);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        using (var fileStream = new FileStream("layout.xml", FileMode.Create, FileAccess.Write))
        {
            _xamDockManager.SaveLayout(fileStream);
        }
    }

    internal MainWindowViewModel ViewModel
    {
        get => (MainWindowViewModel) DataContext;
        private set => DataContext = value;
    }

    private DocumentContentHost _documentContentHost;

    private async void StartModule_Click(object sender, RoutedEventArgs e)
    {
        // I ❤️ C#
        if (sender is FrameworkElement
            {
                DataContext: ModuleViewModel module
            })
        {
            await _moduleLoader.StartModule(new StartRequest(module.Manifest.Id));
        }
    }

    internal sealed class MainWindowViewModel : ObservableObject
    {
        public ObservableCollection<ModuleViewModel> Modules
        {
            get => _modules;
            set => SetProperty(ref _modules, value);
        }

        private ObservableCollection<ModuleViewModel> _modules = new();
    }

    internal sealed class ModuleViewModel
    {
        public ModuleViewModel(IModuleManifest manifest, ImageSourceProvider imageSourceProvider)
        {
            Manifest = manifest;

            if (manifest.TryGetDetails<WebManifestDetails>(out var webManifestDetails))
            {
                if (webManifestDetails.IconUrl != null)
                {
                    ImageSource = imageSourceProvider.GetImageSource(
                        webManifestDetails.IconUrl,
                        webManifestDetails.Url);
                }
            }
            else if (manifest.TryGetDetails<NativeManifestDetails>(out var nativeManifestDetails))
            {
                using var icon =
                    System.Drawing.Icon.ExtractAssociatedIcon(Path.GetFullPath(nativeManifestDetails.Path.ToString()));

                if (icon != null)
                {
                    using var bitmap = icon.ToBitmap();

                    ImageSource = bitmap.ToImageSource();
                }
            }
        }

        public IModuleManifest Manifest { get; }

        public System.Windows.Media.ImageSource? ImageSource { get; }
    }
}