using Microsoft.Extensions.DependencyInjection;
using SuperRPC_POC_Client;


var communicatorHelper = new Communicator();

var serviceProvider = new ServiceCollection()
            .AddSingleton<IServiceObject, ServiceObject>()
            .AddSingleton<Communicator>(communicatorHelper)
            .BuildServiceProvider();

var foo = serviceProvider.GetService<IServiceObject>();


//var communicatorHelper = new Communicator();
//var communicator = await communicatorHelper.GetCommunicator();
//var communicator = communicatorHelper.GetCommunicatorObject();
//var service = new ServiceObject(communicator);
