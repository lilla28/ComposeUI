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
import { Fdc3JoinUserChannelResponse } from "./messages/Fdc3JoinUserChannelResponse";


export class ChannelSelectorClient {
    private channel?: Channel | null;
    //private channelId?: string | undefined;
    //private instanceId?: string | undefined;

    constructor(private readonly messageRouterClient: MessageRouter,  private readonly instanceId: string, private readonly channelId: string){
        console.log("ChannelSelectorClient.constructor");
        console.log("\tinstanceId", instanceId);
        

        (window as any).client = messageRouterClient;
    }

   

    public async subscribe() : Promise<string>{
        console.log("ChannelSelectorClient.subscribe()");
        await this.messageRouterClient.connect();
         let unsubscribable = await this.messageRouterClient.subscribe("ComposeUI/fdc3/v2.0/channelSelector2", (topicMessage: TopicMessage) => {
         console.log("\tunsubscribable", unsubscribable);

          

            console.log("\tmessage=", topicMessage);
            const payload = <Fdc3JoinUserChannelRequest>JSON.parse(topicMessage.payload!); //todo check parsing
            console.log("\tpayload= ", payload)
            if((payload as any).InstanceId === this.instanceId){
                window.fdc3.joinUserChannel("fdc3.channel." + (payload as any).ChannelId );
            }

            
            
            //this.channelId = payload.channelId;
        });

        //const message = JSON.stringify(new Fdc3JoinUserChannelRes(this.channelId, this.instanceId));
        //const payloadObject =
       /* const response = await this.messageRouterClient.invoke("ComposeUI/fdc3/v2.0/channelSelector",  JSON.stringify(payloadObject));
        if (response) {
            const buffer = <Fdc3JoinUserChannelRequest>JSON.parse(response);

            if(buffer.instanceId === this.instanceId){
                window.fdc3.joinUserChannel("fdc3.channel." + buffer.channelId );
            }
        }*/

        return this.channelId!;
    }

   /**/ 
    public async colorUpdate() : Promise<void | undefined>{
       console.log("colorUpdate");
        const message = JSON.stringify(new Fdc3JoinUserChannelRequest(this.channelId, this.instanceId));
        //const message = JSON.stringify(new Fdc3JoinUserChannelResponse(this.channelId, this.instanceId));

        
        //const response = await this.messageRouterClient.invoke(`ComposeUI/fdc3/v2.0/channelSelectorColor-${this.instanceId}`, message);


       //await this.messageRouterClient.publish(`ComposeUI/fdc3/v2.0/channelSelectorColor-${this.instanceId}`, message);
    }
    
           
        
       

}