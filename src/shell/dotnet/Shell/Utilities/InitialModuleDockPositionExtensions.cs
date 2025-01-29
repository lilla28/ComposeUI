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

using Infragistics.Windows.DockManager;
using MorganStanley.ComposeUI.ModuleLoader;
using System;

namespace MorganStanley.ComposeUI.Shell.Utilities;

internal static class InitialModuleDockPositionExtensions
{
    /// <summary>
    /// Converts the <see cref="InitialModuleDockPosition"/> to <see cref="InitialPaneLocation"/> for Infragistics compatibility.
    /// </summary>
    /// <param name="dockPosition"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static InitialPaneLocation ConvertPaneLocation(
        this InitialModuleDockPosition dockPosition)
    {
        return dockPosition switch
        {
            InitialModuleDockPosition.Floating => InitialPaneLocation.DockableFloating,
            InitialModuleDockPosition.FloatingOnly => InitialPaneLocation.FloatingOnly,
            InitialModuleDockPosition.DockLeft => InitialPaneLocation.DockedLeft,
            InitialModuleDockPosition.DockRight => InitialPaneLocation.DockedRight,
            InitialModuleDockPosition.DockTop => InitialPaneLocation.DockedTop,
            InitialModuleDockPosition.DockBottom => InitialPaneLocation.DockedBottom,
            _ => throw new NotSupportedException($"The pane location type is not supported: {dockPosition.ToString()}"),
        };
    }
}
