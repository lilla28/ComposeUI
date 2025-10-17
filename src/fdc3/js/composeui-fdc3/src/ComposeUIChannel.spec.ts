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

import { jest } from '@jest/globals';
import { ComposeUIChannel } from './infrastructure/ComposeUIChannel';
import { ComposeUIContextListener } from './infrastructure/ComposeUIContextListener';
import { ComposeUITopic } from './infrastructure/ComposeUITopic';
import { Channel, ChannelError, Context } from '@finos/fdc3';
import { ComposeUIDesktopAgent } from './ComposeUIDesktopAgent';
import { Fdc3AddContextListenerResponse } from './infrastructure/messages/Fdc3AddContextListenerResponse';
import { IMessaging, JsonMessaging } from '@morgan-stanley/composeui-messaging-abstractions';

const dummyChannelId = "dummyId";
let testChannel: Channel;
let jsonMessagingMock: JsonMessaging;

const testInstrument = {
    type: 'fdc3.instrument',
    id: {
        ticker: 'AAPL'
    }
};
const contextMessageHandlerMock = jest.fn((_) => {
    return "dummy";
});

describe('Tests for ComposeUIChannel implementation API', () => {
    beforeEach(() => {

        window.composeui = {
            fdc3: {
                config: {
                    appId: "testAppId",
                    instanceId: "testInstanceId"
                },
                channelId : "test",
                openAppIdentifier: {
                    openedAppContextId: "test"
                }
            },
            messaging: {
                communicator: undefined
            }
        };

        const response: Fdc3AddContextListenerResponse = {
            success: true,
            id: "testListenerId"
        };

        const messagingMock : IMessaging = {
            subscribe: jest.fn(() => Promise.resolve({ unsubscribe: () => {} })),
            publish: jest.fn(() => Promise.resolve()),
            registerService: jest.fn(() => Promise.resolve({
                unsubscribe: () => {},
                [Symbol.asyncDispose]: () => Promise.resolve()
            })),
            invokeService: jest
                .fn(() => Promise.resolve(JSON.stringify(undefined)))
                .mockImplementationOnce(() => Promise.resolve(JSON.stringify(response)))
        };

        jsonMessagingMock = new JsonMessaging(messagingMock);

        testChannel = new ComposeUIChannel(dummyChannelId, "user", jsonMessagingMock);
    });

    it('broadcast will call messageRouters publish method', async () => {
        await testChannel.broadcast(testInstrument);
        expect(jsonMessagingMock.publishJson).toHaveBeenCalledTimes(1);
        expect(jsonMessagingMock.publishJson).toHaveBeenCalledWith(ComposeUITopic.broadcast(dummyChannelId, "user"), testInstrument);
    });

    it('broadcast will set the lastContext to test instrument', async () => {

        const messagingMock : IMessaging = {
            subscribe: jest.fn(() => Promise.resolve({ unsubscribe: () => {} })),
            publish: jest.fn(() => Promise.resolve()),
            registerService: jest.fn(() => Promise.resolve({
                unsubscribe: () => {},
                [Symbol.asyncDispose]: () => Promise.resolve()
            })),
            invokeService: jest
                .fn(() => Promise.resolve(JSON.stringify(undefined)))
                .mockImplementationOnce(() => Promise.resolve(JSON.stringify(testInstrument)))
        };

        const jsonMessaging = new JsonMessaging(messagingMock);

        testChannel = new ComposeUIChannel(dummyChannelId, "user", jsonMessaging);

        await testChannel.broadcast(testInstrument);
        const resultContext = await testChannel.getCurrentContext();
        expect(jsonMessaging.publish).toHaveBeenCalledTimes(1);
        expect(jsonMessaging.publish).toHaveBeenCalledWith(ComposeUITopic.broadcast(dummyChannelId, "user"), JSON.stringify(testInstrument));
        expect(resultContext).toMatchObject(testInstrument);
    });

    it('getCurrentContext will overwrite the lastContext of the same type', async () => {
        const testInstrument2 = {
            type: 'fdc3.instrument',
            id: {
                ticker: 'SMSN'
            }
        };

        const messagingMock : IMessaging = {
            subscribe: jest.fn(() => Promise.resolve({ unsubscribe: () => {} })),
            publish: jest.fn(() => Promise.resolve()),
            registerService: jest.fn(() => Promise.resolve({
                unsubscribe: () => {},
                [Symbol.asyncDispose]: () => Promise.resolve()
            })),
            invokeService: jest
                .fn(() => Promise.resolve(JSON.stringify(testInstrument)))
                .mockImplementationOnce(() => Promise.resolve(JSON.stringify(testInstrument2)))
                .mockImplementationOnce(() => {return Promise.resolve(`${JSON.stringify(testInstrument2)}`)})
        };

        const jsonMessaging = new JsonMessaging(messagingMock);

        testChannel = new ComposeUIChannel(dummyChannelId, "user", jsonMessaging);

        await testChannel.broadcast(testInstrument);
        await testChannel.broadcast(testInstrument2);

        const resultContext = await testChannel.getCurrentContext();
        const resultContextWithContextType = await testChannel.getCurrentContext(testInstrument2.type);
        expect(jsonMessaging.publish).toBeCalledTimes(2);
        expect(jsonMessaging.publish).toHaveBeenCalledWith(ComposeUITopic.broadcast(dummyChannelId, "user"), JSON.stringify(testInstrument));
        expect(jsonMessaging.publish).toHaveBeenCalledWith(ComposeUITopic.broadcast(dummyChannelId, "user"), JSON.stringify(testInstrument2));
        expect(resultContext).toMatchObject(testInstrument2);
        expect(resultContextWithContextType).toMatchObject<Partial<Context>>(testInstrument2);
    });

    it("getCurrentContext will return null as the given contextType couldn't be found in the saved contexts", async () => {
        const result = await testChannel.getCurrentContext("dummyContextType");
        expect(result).toBe(null);
    });

    // TODO: Test broadcast/getLastContext with different context and contextType combinations
    it('addContextListener will result a ComposeUIContextListener', async () => {
        await testChannel.broadcast(testInstrument);
        const resultListener = await testChannel.addContextListener('fdc3.instrument', contextMessageHandlerMock);
        expect(resultListener).toBeInstanceOf(ComposeUIContextListener);
        expect(contextMessageHandlerMock).toHaveBeenCalledTimes(0);
    });

    // TODO: This doesn't test what it sais it tests
    it('addContextListener will treat contextType is ContextHandler as all types', async () => {
        const resultListener = await testChannel.addContextListener(null, contextMessageHandlerMock);
        expect(resultListener).toBeInstanceOf(ComposeUIContextListener);
        expect(jsonMessagingMock.subscribeJson).toBeCalledTimes(1);
    });
});

describe("AppChanel tests", () => {

    beforeEach(() =>{
        window.composeui = {
            fdc3: {
                config: {
                    appId: "testAppId",
                    instanceId: "testInstanceId"
                },
                channelId : "test",
                openAppIdentifier: {
                    openedAppContextId: "test"
                }
            },
            messaging: {
                communicator: {} as IMessaging
            }
        };
    });

    it("getOrCreateChannel creates a channel", async () => {
        const messagingMock : IMessaging = {
            subscribe: jest.fn(() => Promise.resolve({ unsubscribe: () => {} })),
            publish: jest.fn(() => Promise.resolve()),
            registerService: jest.fn(() => Promise.resolve({
                unsubscribe: () => {},
                [Symbol.asyncDispose]: () => Promise.resolve()
            })),
            invokeService: jest
                .fn(() => Promise.resolve<string | null>(null))
                .mockImplementationOnce(() => Promise.resolve<string | null>(JSON.stringify({ success: true })))
        };

        const jsonMessaging = new JsonMessaging(messagingMock);

        const desktopAgent = new ComposeUIDesktopAgent(jsonMessaging);
        const channel = await desktopAgent.getOrCreateChannel("hello.world");
        expect(channel).toBeInstanceOf(ComposeUIChannel);
    });

    it("getOrCreateChannel throws error as it received error from the DesktopAgent", async () => {
        const messagingMock : IMessaging = {
            subscribe: jest.fn(() => Promise.resolve({ unsubscribe: () => {} })),
            publish: jest.fn(() => Promise.resolve()),
            registerService: jest.fn(() => Promise.resolve({
                unsubscribe: () => {},
                [Symbol.asyncDispose]: () => Promise.resolve()
            })),
            invokeService: jest
                .fn(() => Promise.resolve<string | null>(null))
                .mockImplementationOnce(() => Promise.resolve(JSON.stringify({ success: false, error: "dummy" })))
        };

        const jsonMessaging = new JsonMessaging(messagingMock);

        const desktopAgent = new ComposeUIDesktopAgent(jsonMessaging);
        await expect(desktopAgent.getOrCreateChannel("hello.world"))
            .rejects
            .toThrow("dummy");
    });

    it("getOrCreateChannel throws error as it received no success without error message from the DesktopAgent", async () => {
        const messagingMock : IMessaging = {
            subscribe: jest.fn(() => Promise.resolve({ unsubscribe: () => {} })),
            publish: jest.fn(() => Promise.resolve()),
            registerService: jest.fn(() => Promise.resolve({
                unsubscribe: () => {},
                [Symbol.asyncDispose]: () => Promise.resolve()
            })),
            invokeService: jest
                .fn(() => Promise.resolve<string | null>(null))
                .mockImplementationOnce(() => Promise.resolve(JSON.stringify({ success: false })))
        };

        const jsonMessaging = new JsonMessaging(messagingMock);

        const desktopAgent = new ComposeUIDesktopAgent(jsonMessaging);
        await expect(desktopAgent.getOrCreateChannel("hello.world"))
            .rejects
            .toThrow(ChannelError.CreationFailed);
    });
});
