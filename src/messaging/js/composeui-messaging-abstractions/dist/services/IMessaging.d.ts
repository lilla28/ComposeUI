/// <reference types="node" />
import { TopicMessageHandler, ServiceHandler } from './Delegates';
import { Unsubscribable } from 'rxjs';
/**
 * Abstraction for basic messaging: publish/subscribe and request/response services.
 */
export interface IMessaging {
    /**
     * Subscribes to a topic and receives raw string messages.
     * @param topic Topic identifier.
     * @param subscriber Callback invoked for each serialized message.
     * @param cancellationToken Optional abort signal to cancel subscription establishment.
     * @returns Promise resolving to an Unsubscribable used to stop receiving messages.
     */
    subscribe(topic: string, subscriber: TopicMessageHandler, cancellationToken?: AbortSignal): Promise<Unsubscribable>;
    /**
     * Publishes a raw string message to a topic.
     * @param topic Topic identifier.
     * @param message Serialized message string.
     * @param cancellationToken Optional abort signal to cancel publish operation.
     * @returns Promise that resolves when the message is dispatched.
     */
    publish(topic: string, message: string, cancellationToken?: AbortSignal): Promise<void>;
    /**
     * Registers a service handler for request/response style messaging.
     * @param serviceName Name used by clients to invoke this service.
     * @param serviceHandler Handler receiving an optional serialized request and returning a serialized response or null.
     * @param cancellationToken Optional abort signal to cancel registration.
     * @returns Promise resolving to an AsyncDisposable to unregister the service.
     */
    registerService(serviceName: string, serviceHandler: ServiceHandler, cancellationToken?: AbortSignal): Promise<AsyncDisposable>;
    /**
     * Invokes a registered service.
     * @param serviceName Target service name.
     * @param payload Optional serialized request payload or null.
     * @param cancellationToken Optional abort signal to cancel invocation.
     * @returns Promise resolving to a serialized response string or null.
     */
    invokeService(serviceName: string, payload?: string | null, cancellationToken?: AbortSignal): Promise<string | null>;
}
