using System;
using System.Threading.Tasks;
using Infragistics.Windows.DockManager;
using Infragistics.Windows.DockManager.Events;
using MorganStanley.ComposeUI.ModuleLoader;

namespace MorganStanley.ComposeUI.Shell;

internal class WebWindowContentPane : ContentPane
{
    public WebWindowContentPane(WebWindow webWindow, IModuleLoader moduleLoader)
    {
        WebWindow = webWindow;
        _moduleLoader = moduleLoader;

        Header = webWindow.Options.Title ?? "New tab";
        Content = webWindow.Content;

        Closing += Pane_Closing;
        Closed += Pane_Closed;
    }

    private void Pane_Closed(object? sender, PaneClosedEventArgs e)
    {
        WebWindow.Dispose();
        base.OnClosed(e);
    }

    private void Pane_Closing(object? sender, PaneClosingEventArgs e)
    {
        if (WebWindow.ModuleInstance == null)
            return;

        switch (WebWindow.LifetimeEvent)
        {
            case LifetimeEventType.Stopped:
                return;

            case LifetimeEventType.Stopping:
                e.Cancel = true;
                Visibility = System.Windows.Visibility.Hidden;
                return;

            default:
                e.Cancel = true;
                Visibility = System.Windows.Visibility.Hidden;
                Task.Run(() => _moduleLoader.StopModule(new StopRequest(WebWindow.ModuleInstance.InstanceId)));
                return;
        }
    }

    public WebWindow WebWindow { get; }

    private readonly IModuleLoader _moduleLoader;
}
