/* 
 *  Morgan Stanley makes this available to you under the Apache License,
 *  Version 2.0 (the "License"). You may obtain a copy of the License at
 *       http://www.apache.org/licenses/LICENSE-2.0.
 *  See the NOTICE file distributed with this work for additional information
 *  regarding copyright ownership. Unless required by applicable law or agreed
 *  to in writing, software distributed under the License is distributed on an
 *  "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
 *  or implied. See the License for the specific language governing permissions
 *  and limitations under the License.
 */

import { AppIdentifier, Channel, Context, OpenError } from "@finos/fdc3";
import { JsonMessaging } from "@morgan-stanley/composeui-messaging-abstractions";
import { ComposeUIErrors } from "./ComposeUIErrors";
import { ComposeUITopic } from "./ComposeUITopic";
import { Fdc3OpenRequest } from "./messages/Fdc3OpenRequest";
import { Fdc3OpenResponse } from "./messages/Fdc3OpenResponse";
import { OpenClient } from "./OpenClient";
import { Fdc3GetOpenedAppContextRequest } from "./messages/Fdc3GetOpenedAppContextRequest";
import { Fdc3GetOpenedAppContextResponse } from "./messages/Fdc3GetOpenedAppContextResponse";
import { OpenAppIdentifier } from "./OpenAppIdentifier";

export class MessagingOpenClient implements OpenClient{
    private channel?: Channel | null;

    constructor(
        private readonly instanceId: string,
        private readonly jsonMessaging: JsonMessaging,
        private readonly openAppIdentifier?: OpenAppIdentifier) {}

    public async open(app?: string | AppIdentifier | undefined, context?: Context | undefined): Promise<AppIdentifier> {
        if (!app) {
            throw new Error(OpenError.AppNotFound);
        }

        let appIdentifier: AppIdentifier;
        if (typeof app === "string") {
            appIdentifier = { appId: app };
        } else {
            appIdentifier = app;
        }

        if (context && !('type' in context)) {
            throw new Error(OpenError.MalformedContext);
        }

        if (window.fdc3) {
            this.channel = await window.fdc3.getCurrentChannel();
        }

        const request = new Fdc3OpenRequest(
            this.instanceId,
            appIdentifier,
            context,
            this.channel?.id);

        const response = await this.jsonMessaging.invokeJsonService<Fdc3OpenRequest, Fdc3OpenResponse>(ComposeUITopic.open(), request);
        if (!response) {
            throw new Error(ComposeUIErrors.NoAnswerWasProvided);
        }

        if (response?.error) {
            throw new Error(response.error);
        }

        if (!response || !response.appIdentifier) {
            throw new Error(ComposeUIErrors.NoAnswerWasProvided);
        }

        return response.appIdentifier;
    }

    public async getOpenedAppContext(): Promise<Context> {
        if (!this.openAppIdentifier?.openedAppContextId) {
            throw new Error("Context id is not defined on the window object.");
        }

        const request: Fdc3GetOpenedAppContextRequest = {
            contextId: this.openAppIdentifier.openedAppContextId
        };

        const response = await this.jsonMessaging.invokeJsonService<Fdc3GetOpenedAppContextRequest, Fdc3GetOpenedAppContextResponse>(ComposeUITopic.getOpenedAppContext(), request);
        if (!response) {
            throw new Error(ComposeUIErrors.NoAnswerWasProvided);
        }

        if (!response) {
            throw new Error(ComposeUIErrors.NoAnswerWasProvided);
        }

        if (response.error) {
            throw new Error(response.error);
        }

        if (!response.context) {
            throw new Error(ComposeUIErrors.NoAnswerWasProvided);
        }

        const context = response.context;
        return context;
    }
}