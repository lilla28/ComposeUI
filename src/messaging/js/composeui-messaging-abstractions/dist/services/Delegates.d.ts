/**
 * Handles a topic message already serialized as a string.
 * @param payload Serialized message string.
 * @returns A promise that resolves when processing is complete.
 */
export type TopicMessageHandler = (payload: string) => Promise<void>;
/**
 * Handles a service request where the request and response are JSON-serialized strings.
 * @param request Optional serialized request string or null.
 * @returns A promise resolving to a serialized response string or null.
 */
export type ServiceHandler = (request?: string | null) => Promise<string | null>;
/**
 * Handles a typed service request/response with generic payloads.
 * @typeParam TRequest The request type after deserialization.
 * @typeParam TResponse The response type before serialization.
 * @param request Optional typed request or null.
 * @returns A promise resolving to a typed response or null.
 */
export type TypedServiceHandler<TRequest, TResponse> = (request?: TRequest | null) => Promise<TResponse | null>;
