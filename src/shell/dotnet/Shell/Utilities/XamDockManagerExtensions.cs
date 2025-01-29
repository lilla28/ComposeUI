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

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Infragistics.Windows.DockManager;

namespace MorganStanley.ComposeUI.Shell.Utilities;

internal static class XamDockManagerExtensions
{
    public static void OpenLocatedContentPane(
        this XamDockManager xamDockManager,
        FrameworkElement frameworkElement,
        WebWindowOptions options)
    {
        var panes = xamDockManager.GetPanes(PaneNavigationOrder.ActivationOrder);

        //TODO: Dock in active tab? - or selected one by the user?
        var targetHostPane = /*options.InitialDockingModuleTarget == null*/
            /*? */panes.FirstOrDefault(pane => pane.IsActivePane);
            //: panes.FirstOrDefault(pane => ((WebContent) pane.DataContext).Options.Id == details.InitialDockingModuleTarget);


        if (targetHostPane == null)
        {
            var splitPane = new SplitPane();

            splitPane.SetSplitPaneFloatingLocation(options.Coordinates);
            splitPane.SetSplitPaneFloatingSize(options.Width, options.Height);
            splitPane.SetValue(XamDockManager.InitialLocationProperty, options.InitialModuleDockPostion ?? InitialPaneLocation.DockedLeft);

            //This behaves differently. It takes the previous InitialPaneLocation instead of the current.
            splitPane.Panes.Add(frameworkElement);
            xamDockManager.Panes.Add(splitPane);
            return;
        }

        var modulePosition = GetModulePosition(options.InitialModuleDockPostion);

        App.Current.Dispatcher.Invoke(() =>
        {
            if (targetHostPane.Parent is not SplitPane parentHostSplitPane)
            {
                return;
            }

            if (targetHostPane.Visibility == Visibility.Collapsed)
            {
                frameworkElement.Visibility = Visibility.Collapsed;
            }

            var targetPaneIndex = parentHostSplitPane.Panes.IndexOf(targetHostPane);

            if (parentHostSplitPane.SplitterOrientation == modulePosition.Orientation)
            {
                parentHostSplitPane.Panes.Insert(
                    modulePosition.IsFirstItem 
                        ? targetPaneIndex
                        : targetPaneIndex + 1, 
                    frameworkElement);
            }
            else
            {
                parentHostSplitPane.Panes.Remove(targetHostPane);

                var newSplitPane = new SplitPane
                {
                    SplitterOrientation = modulePosition.Orientation
                };

                newSplitPane.Panes.Add(
                    modulePosition.IsFirstItem
                        ? frameworkElement
                        : targetHostPane);

                newSplitPane.Panes.Add(
                    modulePosition.IsFirstItem
                        ? targetHostPane
                        : frameworkElement);

                parentHostSplitPane.Panes.Insert(targetPaneIndex, newSplitPane);
            }
        });
    }

    private static ModulePosition GetModulePosition(InitialPaneLocation? initialPaneLocation)
    {
        var modulePosition = new ModulePosition();

        if (initialPaneLocation is not InitialPaneLocation initialModuleDockPosition)
        {
            return modulePosition;
        }

        switch (initialModuleDockPosition)
        {
            case InitialPaneLocation.DockedLeft:
                modulePosition.Orientation = Orientation.Vertical;
                modulePosition.IsFirstItem = true;
                break;

            case InitialPaneLocation.DockedRight:
                modulePosition.Orientation = Orientation.Vertical;
                break;

            case InitialPaneLocation.DockedTop:
                modulePosition.IsFirstItem = true;
                break;
        }

        return modulePosition;
    }
}
