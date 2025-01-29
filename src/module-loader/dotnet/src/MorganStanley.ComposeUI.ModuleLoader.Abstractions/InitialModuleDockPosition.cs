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

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MorganStanley.ComposeUI.ModuleLoader;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InitialModuleDockPosition
{
    [EnumMember(Value = $"{nameof(Floating)}")]
    Floating,

    [EnumMember(Value = $"{nameof(FloatingOnly)}")]
    FloatingOnly,

    [EnumMember(Value = $"{nameof(DockLeft)}")]
    DockLeft,

    [EnumMember(Value = $"{nameof(DockRight)}")]
    DockRight,

    [EnumMember(Value = $"{nameof(DockTop)}")]
    DockTop,

    [EnumMember(Value = $"{nameof(DockBottom)}")]
    DockBottom,
}
