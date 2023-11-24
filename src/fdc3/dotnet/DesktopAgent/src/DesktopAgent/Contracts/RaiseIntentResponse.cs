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

using MorganStanley.Fdc3;

namespace MorganStanley.ComposeUI.Fdc3.DesktopAgent.Contracts;

/// <summary>
/// Response, for handling <see cref="RaiseIntentRequest"/> originated via fdc.raiseIntent by clients.
/// </summary>
internal sealed class RaiseIntentResponse
{
    /// <summary>
    /// Unique identifier for the raised intent message, which was generated from the gotten MessageId as int from the client and a <seealso cref="Guid"/>.
    /// </summary>
    public string? MessageId { get; set; }
    /// <summary>
    /// Intent, for which the raiseIntent was executed.
    /// </summary>
    public string? Intent { get; set; }

    /// <summary>
    /// Apps, that could handle the raiseIntent.
    /// </summary>
    public IEnumerable<AppMetadata>? AppMetadata { get; set; }

    /// <summary>
    /// Error, which indicates that some error has happened during the raiseIntent's execution.
    /// </summary>
    public string? Error { get; set; }

    public static RaiseIntentResponse Success(string messageId, AppIntent appIntent) => new() { MessageId = messageId, Intent = appIntent.Intent.Name, AppMetadata = appIntent.Apps.Select(appMetadata => (AppMetadata)appMetadata) };
    public static RaiseIntentResponse Success(string messageId, string intent, IAppMetadata appMetadata) => new() { MessageId = messageId, Intent = intent, AppMetadata = new[] { (AppMetadata)appMetadata } };
    public static RaiseIntentResponse Success(string messageId, string intent, AppMetadata appMetadata) => new() {MessageId = messageId, Intent = intent, AppMetadata = new[] { appMetadata } };
    public static RaiseIntentResponse Failure(string error) => new() { Error = error };
}
