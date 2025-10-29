export class AsyncDisposableWrapper {
    messageRouterClient;
    serviceName;
    constructor(messageRouterClient, serviceName) {
        this.messageRouterClient = messageRouterClient;
        this.serviceName = serviceName;
    }
    [Symbol.asyncDispose]() {
        return this.messageRouterClient.unregisterService(this.serviceName);
    }
}
