
  /**
   * @license
   * author: Morgan Stanley
   * composeui-messaging-abstractions.js v0.1.0-alpha.9
   * Released under the Apache-2.0 license.
   */

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
/**
 * JSON wrapper around a lower-level IMessaging implementation.
 * Provides typed helpers for publish/subscribe and request/response using JSON serialization.
 */
class JsonMessaging {
    messaging;
    /**
     * Creates a new JsonMessaging adapter.
     * @param messaging Underlying messaging implementation handling raw string messages.
     */
    constructor(messaging) {
        this.messaging = messaging;
    }
    /**
     * Subscribes to a topic with a raw string handler.
     * @param topic Topic identifier.
     * @param subscriber Callback invoked with each serialized message string.
     * @param cancellationToken Optional abort signal for subscription setup.
     * @returns Promise resolving to an Unsubscribable to stop receiving messages.
     */
    async subscribe(topic, subscriber, cancellationToken) {
        return this.messaging.subscribe(topic, subscriber, cancellationToken);
    }
    /**
     * Publishes a raw string message.
     * @param topic Topic identifier.
     * @param message Serialized message string.
     * @param cancellationToken Optional abort signal.
     */
    async publish(topic, message, cancellationToken) {
        return this.messaging.publish(topic, message, cancellationToken);
    }
    /**
     * Registers a raw service handler.
     * @param serviceName Service name used for invocation.
     * @param serviceHandler Handler operating on serialized request/response strings.
     * @param cancellationToken Optional abort signal.
     * @returns Promise resolving to an AsyncDisposable for unregistering.
     */
    async registerService(serviceName, serviceHandler, cancellationToken) {
        return this.messaging.registerService(serviceName, serviceHandler, cancellationToken);
    }
    /**
     * Invokes a raw service.
     * @param serviceName Service name.
     * @param payload Optional serialized request payload or null.
     * @param cancellationToken Optional abort signal.
     * @returns Promise resolving to a serialized response or null.
     */
    async invokeService(serviceName, payload, cancellationToken) {
        return this.messaging.invokeService(serviceName, payload, cancellationToken);
    }
    /**
     * Subscribes with a typed JSON payload handler.
     * @typeParam TPayload Deserialized payload type.
     * @param topic Topic identifier.
     * @param typedSubscriber Callback receiving the typed payload.
     * @param cancellationToken Optional abort signal.
     * @returns Promise resolving to an Unsubscribable for the subscription.
     */
    async subscribeJson(topic, typedSubscriber, cancellationToken) {
        const jsonSubscriber = async (message) => {
            const payload = JSON.parse(message);
            await typedSubscriber(payload);
        };
        return this.messaging.subscribe(topic, jsonSubscriber, cancellationToken);
    }
    /**
     * Publishes a typed payload by JSON serializing it.
     * @typeParam TPayload Payload type.
     * @param topic Topic identifier.
     * @param payload Typed payload instance.
     * @param cancellationToken Optional abort signal.
     */
    async publishJson(topic, payload, cancellationToken) {
        const stringPayload = JSON.stringify(payload);
        return this.messaging.publish(topic, stringPayload, cancellationToken);
    }
    /**
     * Invokes a service with a typed request and typed response.
     * @typeParam TPayload Request type.
     * @typeParam TResult Response type.
     * @param serviceName Service name.
     * @param payload Typed request payload.
     * @param cancellationToken Optional abort signal.
     * @returns Promise resolving to typed response or null.
     */
    async invokeJsonService(serviceName, payload, cancellationToken) {
        const stringPayload = JSON.stringify(payload);
        const response = await this.messaging.invokeService(serviceName, stringPayload, cancellationToken);
        if (response == null) {
            return null;
        }
        return JSON.parse(response);
    }
    /**
     * Invokes a service that expects no request body.
     * @typeParam TResult Response type.
     * @param serviceName Service name.
     * @param cancellationToken Optional abort signal.
     * @returns Promise resolving to typed response or null.
     */
    async invokeJsonServiceNoRequest(serviceName, cancellationToken) {
        const response = await this.messaging.invokeService(serviceName, null, cancellationToken);
        return response == null ? null : JSON.parse(response);
    }
    /**
     * Registers a typed JSON service handler.
     * @typeParam TRequest Request type.
     * @typeParam TResult Response type.
     * @param serviceName Service name to register.
     * @param typedHandler Handler receiving a typed request and returning a typed response or null.
     * @param cancellationToken Optional abort signal.
     * @returns Promise resolving to an AsyncDisposable for unregistering.
     */
    async registerJsonService(serviceName, typedHandler, cancellationToken) {
        const jsonServiceHandler = this.createJsonServiceHandler(typedHandler);
        return this.messaging.registerService(serviceName, jsonServiceHandler, cancellationToken);
    }
    /**
     * Creates an internal raw service handler that performs JSON serialization/deserialization.
     * @typeParam TRequest Request type.
     * @typeParam TResult Response type.
     * @param realHandler Typed handler to wrap.
     * @returns A ServiceHandler operating on serialized strings.
     */
    createJsonServiceHandler(realHandler) {
        return async (payload) => {
            const request = payload == null ? null : JSON.parse(payload);
            const result = await realHandler(request);
            if (typeof result === 'string') {
                return result;
            }
            return result == null ? null : JSON.stringify(result);
        };
    }
}

export { JsonMessaging };
//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiaW5kZXguanMiLCJzb3VyY2VzIjpbIi4uL3NyYy9zZXJ2aWNlcy9Kc29uTWVzc2FnaW5nLnRzIl0sInNvdXJjZXNDb250ZW50IjpbbnVsbF0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiI7Ozs7Ozs7O0FBQUE7Ozs7Ozs7Ozs7O0FBV0c7QUFNSDs7O0FBR0c7TUFDVSxhQUFhLENBQUE7QUFLTyxJQUFBLFNBQUE7QUFKN0I7OztBQUdHO0FBQ0gsSUFBQSxXQUFBLENBQTZCLFNBQXFCLEVBQUE7UUFBckIsSUFBQSxDQUFBLFNBQVMsR0FBVCxTQUFTO0lBQWU7QUFFckQ7Ozs7OztBQU1HO0FBQ0gsSUFBQSxNQUFNLFNBQVMsQ0FBQyxLQUFhLEVBQUUsVUFBK0IsRUFBRSxpQkFBK0IsRUFBQTtBQUMzRixRQUFBLE9BQU8sSUFBSSxDQUFDLFNBQVMsQ0FBQyxTQUFTLENBQUMsS0FBSyxFQUFFLFVBQVUsRUFBRSxpQkFBaUIsQ0FBQztJQUN6RTtBQUVBOzs7OztBQUtHO0FBQ0gsSUFBQSxNQUFNLE9BQU8sQ0FBQyxLQUFhLEVBQUUsT0FBZSxFQUFFLGlCQUErQixFQUFBO0FBQ3pFLFFBQUEsT0FBTyxJQUFJLENBQUMsU0FBUyxDQUFDLE9BQU8sQ0FBQyxLQUFLLEVBQUUsT0FBTyxFQUFFLGlCQUFpQixDQUFDO0lBQ3BFO0FBRUE7Ozs7OztBQU1HO0FBQ0gsSUFBQSxNQUFNLGVBQWUsQ0FBQyxXQUFtQixFQUFFLGNBQThCLEVBQUUsaUJBQStCLEVBQUE7QUFDdEcsUUFBQSxPQUFPLElBQUksQ0FBQyxTQUFTLENBQUMsZUFBZSxDQUFDLFdBQVcsRUFBRSxjQUFjLEVBQUUsaUJBQWlCLENBQUM7SUFDekY7QUFFQTs7Ozs7O0FBTUc7QUFDSCxJQUFBLE1BQU0sYUFBYSxDQUFDLFdBQW1CLEVBQUUsT0FBdUIsRUFBRSxpQkFBK0IsRUFBQTtBQUM3RixRQUFBLE9BQU8sSUFBSSxDQUFDLFNBQVMsQ0FBQyxhQUFhLENBQUMsV0FBVyxFQUFFLE9BQU8sRUFBRSxpQkFBaUIsQ0FBQztJ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
