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

import { Context, ContextHandler, Listener } from "@finos/fdc3";
import { MessageRouter, TopicMessage } from "@morgan-stanley/composeui-messaging-client";
import { ChannelType } from "./ChannelType";
import { Unsubscribable } from "rxjs";
import { ComposeUITopic } from "./ComposeUITopic";

export class ComposeUIContextListener implements Listener {
    private messageRouterClient: MessageRouter;
    private unsubscribable?: Unsubscribable;
    private handler: ContextHandler;
    private channelId: string;
    private channelType: ChannelType;
    private contextType?: string;
    private isSubscribed: boolean = false;
    public latestContext: Context | null = null;
    public readonly Subscribed: boolean = this.isSubscribed;

    constructor(messageRouterClient: MessageRouter, handler: ContextHandler, channelId: string, channelType: ChannelType, contextType?: string) {
        this.messageRouterClient = messageRouterClient;
        this.handler = handler;
        this.channelId = channelId;
        this.channelType = channelType;
        this.contextType = contextType;
    }

    public async subscribe(): Promise<void> {
        const subscribeTopic = ComposeUITopic.broadcast(this.channelId, this.channelType);
        this.unsubscribable = await this.messageRouterClient.subscribe(subscribeTopic, (topicMessage: TopicMessage) => {
            console.log("CONTEXT SUBSCRIBE:", subscribeTopic, ", received: ", topicMessage);
            if (topicMessage.context.sourceId == this.messageRouterClient.clientId) {
                return;
            }
            
            //TODO: integration test
            try {
                const context: Context = <Context>JSON.parse(topicMessage.payload!);
                if (!this.contextType || this.contextType == context!.type) {
                    this.handler!(context!);
                }
            } catch (err) {
                console.error(err);
            }
        });
        this.isSubscribed = true;
        console.log("SUBSCRIBED,", subscribeTopic, this.channelId, this.channelType);
    }

    public async handleContextMessage(context: Context | null = null): Promise<void> {
        if (!this.isSubscribed) {
            throw new Error("The current listener is not subscribed.");
        }

        if (context) {
            this.handler(context);
            return;
        }

        if (this.latestContext) {
            this.handler(this.latestContext);
            return;
        } 

        this.handler({ type: "" });
        return;
    }

    public unsubscribe(): void {
        if (!this.unsubscribable || !this.isSubscribed) {
            return;
        }

        try{
            this.unsubscribable?.unsubscribe();
        } catch (err) {
            console.error(err);
        }
        this.isSubscribed = false;
        return;
    }
}