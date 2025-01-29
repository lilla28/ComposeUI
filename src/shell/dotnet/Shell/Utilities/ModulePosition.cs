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

using System.Windows.Controls;

namespace MorganStanley.ComposeUI.Shell.Utilities;

internal class ModulePosition
{
    /// <summary>
    /// Sets the orientation of the docked window. Default value: Horizontal.
    /// </summary>
    public Orientation Orientation { get; set; } = Orientation.Horizontal;

    /// <summary>
    /// Sets the order of the docked elements. By default puts at the end of the document.
    /// </summary>
    public bool IsFirstItem { get; set; } = false;
}
