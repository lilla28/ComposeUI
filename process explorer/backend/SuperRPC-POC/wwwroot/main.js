const { SuperRPC } = superrpc;

let rpc, service, testDTO, process, InfoAggregatorDto, RegistrationMonitorDto, RegistrationDto, EnvironmentMonitorDto,
    ConnectionMonitorDto, ConnectionDto, ModuleMonitorDto, ModuleDto;

const ws = new WebSocket('ws://localhost:5056/super-rpc');
ws.addEventListener('open', async () => {
    rpc = new SuperRPC(() => (Math.random()*1e17).toString(36));

    rpc.connect({
        sendAsync: (message) => ws.send(JSON.stringify(message)),
        receive: (callback) => {
            ws.addEventListener('message', (msg) => callback(JSON.parse(msg.data)));
        }
    });

    await rpc.requestRemoteDescriptors();

    process = rpc.getProxyObject('process');

    rpc.sendRemoteDescriptors();
});


