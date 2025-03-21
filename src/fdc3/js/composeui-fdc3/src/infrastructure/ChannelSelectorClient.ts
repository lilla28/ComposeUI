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

import { MessageRouter } from "@morgan-stanley/composeui-messaging-client";
import { Fdc3UIUserChannelChangedRequest } from "./messages/Fdc3UIUserChannelChangedRequest";
import { ComposeUIDesktopAgent } from "../ComposeUIDesktopAgent";


export class ChannelSelectorClient {

    constructor(
        private readonly messageRouterClient: MessageRouter,  
        private readonly instanceId: string,
        private readonly composeUIDesktopAgent: ComposeUIDesktopAgent) {

        console.log("ChannelSelectorClient.constructor");
        console.log("\tinstanceId", instanceId);
    }

    public async subscribe() : Promise<string>{
        console.log("ChannelSelectorClient.subscribe()");
        await this.messageRouterClient.connect();
        await this.messageRouterClient.registerService("ComposeUI/fdc3/v2.0/userChannelUIChanged-" + this.instanceId, async (endpoint, messageBuffer, context) => {
            console.log("\tmessage=", messageBuffer);
            if (!messageBuffer) {
                return;
            }

            const payload = <Fdc3UIUserChannelChangedRequest>JSON.parse(messageBuffer); //todo check parsing
            console.log("\tpayload= ", payload)
            if(payload.instanceId === this.instanceId){
                // window.fdc3.joinUserChannel("fdc3.channel." + (payload as any).ChannelId );
                await this.composeUIDesktopAgent.setUserChannel(payload.channelId); //TODO add displaymetadata
            }
        });

        return "";
    }
}