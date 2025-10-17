/// <reference types="node" />
import { IMessaging } from './IMessaging';
import { TopicMessageHandler, ServiceHandler, TypedServiceHandler } from './Delegates';
import { Unsubscribable } from 'rxjs';
/**
 * JSON wrapper around a lower-level IMessaging implementation.
 * Provides typed helpers for publish/subscribe and request/response using JSON serialization.
 */
export declare class JsonMessaging implements IMessaging {
    private readonly messaging;
    /**
     * Creates a new JsonMessaging adapter.
     * @param messaging Underlying messaging implementation handling raw string messages.
     */
    constructor(messaging: IMessaging);
    /**
     * Subscribes to a topic with a raw string handler.
     * @param topic Topic identifier.
     * @param subscriber Callback invoked with each serialized message string.
     * @param cancellationToken Optional abort signal for subscription setup.
     * @returns Promise resolving to an Unsubscribable to stop receiving messages.
     */
    subscribe(topic: string, subscriber: TopicMessageHandler, cancellationToken?: AbortSignal): Promise<Unsubscribable>;
    /**
     * Publishes a raw string message.
     * @param topic Topic identifier.
     * @param message Serialized message string.
     * @param cancellationToken Optional abort signal.
     */
    publish(topic: string, message: string, cancellationToken?: AbortSignal): Promise<void>;
    /**
     * Registers a raw service handler.
     * @param serviceName Service name used for invocation.
     * @param serviceHandler Handler operating on serialized request/response strings.
     * @param cancellationToken Optional abort signal.
     * @returns Promise resolving to an AsyncDisposable for unregistering.
     */
    registerService(serviceName: string, serviceHandler: ServiceHandler, cancellationToken?: AbortSignal): Promise<AsyncDisposable>;
    /**
     * Invokes a raw service.
     * @param serviceName Service name.
     * @param payload Optional serialized request payload or null.
     * @param cancellationToken Optional abort signal.
     * @returns Promise resolving to a serialized response or null.
     */
    invokeService(serviceName: string, payload?: string | null, cancellationToken?: AbortSignal): Promise<string | null>;
    /**
     * Subscribes with a typed JSON payload handler.
     * @typeParam TPayload Deserialized payload type.
     * @param topic Topic identifier.
     * @param typedSubscriber Callback receiving the typed payload.
     * @param cancellationToken Optional abort signal.
     * @returns Promise resolving to an Unsubscribable for the subscription.
     */
    subscribeJson<TPayload>(topic: string, typedSubscriber: (payload: TPayload) => void | Promise<void>, cancellationToken?: AbortSignal): Promise<Unsubscribable>;
    /**
     * Publishes a typed payload by JSON serializing it.
     * @typeParam TPayload Payload type.
     * @param topic Topic identifier.
     * @param payload Typed payload instance.
     * @param cancellationToken Optional abort signal.
     */
    publishJson<TPayload>(topic: string, payload: TPayload, cancellationToken?: AbortSignal): Promise<void>;
    /**
     * Invokes a service with a typed request and typed response.
     * @typeParam TPayload Request type.
     * @typeParam TResult Response type.
     * @param serviceName Service name.
     * @param payload Typed request payload.
     * @param cancellationToken Optional abort signal.
     * @returns Promise resolving to typed response or null.
     */
    invokeJsonService<TPayload, TResult>(serviceName: string, payload: TPayload, cancellationToken?: AbortSignal): Promise<TResult | null>;
    /**
     * Invokes a service that expects no request body.
     * @typeParam TResult Response type.
     * @param serviceName Service name.
     * @param cancellationToken Optional abort signal.
     * @returns Promise resolving to typed response or null.
     */
    invokeJsonServiceNoRequest<TResult>(serviceName: string, cancellationToken?: AbortSignal): Promise<TResult | null>;
    /**
     * Registers a typed JSON service handler.
     * @typeParam TRequest Request type.
     * @typeParam TResult Response type.
     * @param serviceName Service name to register.
     * @param typedHandler Handler receiving a typed request and returning a typed response or null.
     * @param cancellationToken Optional abort signal.
     * @returns Promise resolving to an AsyncDisposable for unregistering.
     */
    registerJsonService<TRequest, TResult>(serviceName: string, typedHandler: TypedServiceHandler<TRequest, TResult>, cancellationToken?: AbortSignal): Promise<AsyncDisposable>;
    /**
     * Creates an internal raw service handler that performs JSON serialization/deserialization.
     * @typeParam TRequest Request type.
     * @typeParam TResult Response type.
     * @param realHandler Typed handler to wrap.
     * @returns A ServiceHandler operating on serialized strings.
     */
    private createJsonServiceHandler;
}
