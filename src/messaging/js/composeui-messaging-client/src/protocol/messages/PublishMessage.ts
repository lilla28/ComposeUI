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
 *  
 */

import { MessageBuffer } from "../../MessageBuffer";
import { MessageScope } from "../../MessageScope";
import { Message } from "./Message";

export interface PublishMessage extends Message {
    type: "Publish";
    topic: string;
    payload?: MessageBuffer;
    scope?: MessageScope;
    correlationId?: string;
}

export function isPublishMessage(message: Message): message is PublishMessage {
    return message.type == "Publish";
}