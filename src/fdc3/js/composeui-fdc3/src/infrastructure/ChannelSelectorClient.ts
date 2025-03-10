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

import { Channel } from "@finos/fdc3";
import { MessageRouter, TopicMessage } from "@morgan-stanley/composeui-messaging-client";
import { Fdc3JoinUserChannelRequest } from "./messages/Fdc3JoinUserChannelRequest";


export class ChannelSelectorClient {
    private channel?: Channel | null;
    private channelId?: string | undefined;

    constructor(private readonly messageRouterClient: MessageRouter,  private readonly instanceId: string){
        console.log("ChannelSelectorClient.constructor");
        console.log("\tinstanceId", instanceId);
        

        (window as any).client = messageRouterClient;
    }

   

    public async subscribe() : Promise<string>{
        console.log("ChannelSelectorClient.subscribe()");
        await this.messageRouterClient.connect();
         let unsubscribable = await this.messageRouterClient.subscribe("ComposeUI/fdc3/v2.0/channelSelector2", (topicMessage: TopicMessage) => {
         console.log("\tunsubscribable", unsubscribable);
           /* if (topicMessage.context.sourceId == this.messageRouterClient.clientId) {
                return;
            }*/

          

            console.log("\tmessage=", topicMessage);
            const payload = <Fdc3JoinUserChannelRequest>JSON.parse(topicMessage.payload!); //todo check parsing
            console.log("\tpayload= ", payload)
            if((payload as any).InstanceId === this.instanceId){
                window.fdc3.joinUserChannel("fdc3.channel." + (payload as any).ChannelId );
            }

            
            
            //this.channelId = payload.channelId;
        });

        

        return this.channelId!;
    }
       
        
       

 
}