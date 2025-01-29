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

using MorganStanley.ComposeUI.ModuleLoader;

namespace MorganStanley.ComposeUI.Fdc3.AppDirectory;

internal class ComposeUIHostManifest
{
    /// <summary>
    /// The initial docked location of the <see cref="Fdc3App"/>.
    /// </summary>
    public InitialModuleDockPosition InitialModuleDockPosition { get; set; } = InitialModuleDockPosition.Floating;

    /// <summary>
    /// Initial width of the application when it is started as floating.
    /// </summary>
    public double? Width { get; set; }

    /// <summary>
    /// Initial height of the application when it is started as floating.
    /// </summary>
    public double? Height { get; set; }

    /// <summary>
    /// Initial coordinate positions of the window if it is opened as floating window.
    /// </summary>
    public Coordinates? Coordinates { get; set; }
}
