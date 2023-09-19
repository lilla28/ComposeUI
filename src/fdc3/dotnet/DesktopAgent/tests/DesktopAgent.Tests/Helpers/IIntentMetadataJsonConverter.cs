﻿/*
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

using System.Text.Json;
using System.Text.Json.Serialization;
using MorganStanley.Fdc3;

namespace MorganStanley.ComposeUI.Fdc3.DesktopAgent.Tests.Helpers;

internal class IIntentMetadataJsonConverter : JsonConverter<IIntentMetadata>
{
    public override IIntentMetadata? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var result = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
        var name = result.GetProperty("name").GetString();
        var displayName = result.GetProperty("displayName").GetString();
        return new IntentMetadata(name, displayName);
    }

    public override void Write(Utf8JsonWriter writer, IIntentMetadata value, JsonSerializerOptions options)
    {
        if (value is IntentMetadata intentMetadata)
        {
            JsonSerializer.Serialize(writer, intentMetadata, options);
        }
    }
}
