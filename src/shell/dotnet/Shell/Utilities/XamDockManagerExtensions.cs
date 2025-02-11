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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Infragistics.Windows.DockManager;
using MorganStanley.ComposeUI.ModuleLoader;

namespace MorganStanley.ComposeUI.Shell.Utilities;

internal static class XamDockManagerExtensions
{
    /// <summary>
    /// Places the created <see cref="ContentPane"/> via <see cref="App.CreateWebContent(object[])"/> into the <see cref="XamDockManager"/>'s container based on the ModuleCatalog configuration for each module.
    /// </summary>
    /// <param name="xamDockManager">The dock manager which handles the created <see cref="ContentPane"/>'s docking.</param>
    /// <param name="frameworkElement">The created <see cref="ContentPane"/>.</param>
    /// <param name="options">Options to set the created <see cref="ContentPane"/>'s intial configuration, like: location, size etc.</param>
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

        ////TODO: if docktabbed
        //if (options.InitialModuleDockPostion == InitialModuleDockPosition.DockTabbed)
        //{
        //    App.Current.Dispatcher.Invoke(() =>
        //    {
        //        AddDockTabbedSplitPane(xamDockManager, targetHostPane, frameworkElement);
        //    });

        //    return;
        //}

        var moduleDockPosition = options.InitialModuleDockPostion.ConvertPaneLocation();

        if (targetHostPane == null
            || moduleDockPosition == InitialPaneLocation.DockableFloating
            || moduleDockPosition == InitialPaneLocation.FloatingOnly)
        {
            var splitPane = new SplitPane();

            splitPane.SetSplitPaneFloatingLocation(options.Coordinates);
            splitPane.SetSplitPaneFloatingSize(options.Width, options.Height);
            splitPane.SetValue(XamDockManager.InitialLocationProperty, moduleDockPosition);

            //This behaves differently. It takes the previous InitialPaneLocation instead of the current.
            splitPane.Panes.Add(frameworkElement);
            xamDockManager.Panes.Add(splitPane);
            return;
        }

        App.Current.Dispatcher.Invoke(() =>
        {
            var modulePosition = GetModulePosition(moduleDockPosition);

            //If the window is a simple window docked into the XamDockManager main container
            if (targetHostPane.Parent is SplitPane parentHostSplitPane)
            {
               AddLocatedSplitPane(
                   targetHostPane, 
                   parentHostSplitPane, 
                   frameworkElement, 
                   modulePosition);
            }
            //If the active window is tabbed within each other like a DocumentHost
            else if (targetHostPane.Parent is TabGroupPane tabGroupPane
                && tabGroupPane.Parent is SplitPane tabGroupPaneSplitPane)
            {
                AddLocatedSplitPane(
                   tabGroupPane,
                   tabGroupPaneSplitPane,
                   frameworkElement,
                   modulePosition);
            }
        });
    }

    private static void AddDockTabbedSplitPane(
        XamDockManager xamDockManager, 
        ContentPane? targetHostContentPane, 
        FrameworkElement createdContentPane)
    {
        if (targetHostContentPane == null)
        {
            var documentContentHost = (DocumentContentHost) xamDockManager.Content;


            return;
        }

        if (targetHostContentPane.Parent is SplitPane targetHostParentSplitPane)
        {
            var index = targetHostParentSplitPane.Panes.IndexOf(targetHostContentPane);
            var originalSize = SplitPane.GetRelativeSize(targetHostContentPane);

            targetHostParentSplitPane.Panes.Remove(targetHostContentPane);
            var tabGroupPane = new TabGroupPane();
            tabGroupPane.Items.Add(targetHostContentPane);
            tabGroupPane.Items.Add(createdContentPane);

            SplitPane.SetRelativeSize(tabGroupPane, originalSize);
            targetHostParentSplitPane.Panes.Insert(index, tabGroupPane);
        }
        else if (targetHostContentPane.Parent is TabGroupPane targetHostParentTabGroupPane)
        {

        }
    }

    private static void AddLocatedSplitPane(
        FrameworkElement targetHostContentPane,
        SplitPane targetHostParentSplitPane,
        FrameworkElement createdContentPane,
        ModulePosition createdContentPanePosition)
    {
        if (targetHostContentPane.Visibility == Visibility.Collapsed)
        {
            createdContentPane.Visibility = Visibility.Collapsed;
        }

        var targetPaneIndex = targetHostParentSplitPane.Panes.IndexOf(targetHostContentPane);

        if (targetHostParentSplitPane.SplitterOrientation == createdContentPanePosition.Orientation)
        {
            targetHostParentSplitPane.Panes.Insert(
                createdContentPanePosition.IsFirstItem //if it's a Top or Bottom
                    ? targetPaneIndex
                    : targetPaneIndex + 1,
                createdContentPane);
        }
        else
        {
            targetHostParentSplitPane.Panes.Remove(targetHostContentPane);

            var newSplitPane = new SplitPane
            {
                SplitterOrientation = createdContentPanePosition.Orientation
            };

            newSplitPane.Panes.Add(
                createdContentPanePosition.IsFirstItem //if it's a Top or Bottom
                    ? createdContentPane
                    : targetHostContentPane);

            newSplitPane.Panes.Add(
                createdContentPanePosition.IsFirstItem //if it's a Top or Bottom
                    ? targetHostContentPane
                    : createdContentPane);

            targetHostParentSplitPane.Panes.Insert(targetPaneIndex, newSplitPane);
        }
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
