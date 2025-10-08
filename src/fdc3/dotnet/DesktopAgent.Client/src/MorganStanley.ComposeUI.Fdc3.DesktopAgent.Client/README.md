# MorganStanley.ComposeUI.Fdc3.DesktopAgent.Client
MorganStanley.ComposeUI.Fdc3.DesktopAgent.Client is a .NET Standard 2.0 library that provides a client implementation for the FDC3 Desktop Agent API for native applications. It enables .NET applications to interact with FDC3-compliant desktop agents, supporting context sharing, channel management, and intent invocation for financial desktop interoperability.


## Features
- Context Sharing: Add context listeners and broadcast context to channels.
- Channel Management: Join, leave, and query user channels.
- App Metadata: Retrieve metadata for FDC3 applications.
- Extensible Logging: Integrates with `Microsoft.Extensions.Logging`.


## Target Framework
- .NET Standard 2.0

Compatible with .NET (Core), .NET Framework.


## Dependencies
- [Finos.Fdc3](https://www.nuget.org/packages/Finos.Fdc3)
- [Finos.Fdc3.AppDirectory](https://www.nuget.org/packages/Finos.Fdc3.AppDirectory)
- [Microsoft.Extensions.Logging.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Abstractions)
- [System.Text.Json](https://www.nuget.org/packages/System.Text.Json)
- [MorganStanley.ComposeUI.Messaging.Abstractions](https://www.nuget.org/packages/MorganStanley.ComposeUI.Messaging.Abstractions)
- [MorganStanley.ComposeUI.Fdc3.DesktopAgent.Shared](https://www.nuget.org/packages/MorganStanley.ComposeUI.Fdc3.DesktopAgent.Shared)


## Installation
Install via NuGet:
```script
dotnet package add MorganStanley.ComposeUI.Fdc3.DesktopAgent.Client
```

Or via the NuGet Package Manager:

```
PM> Install-Package MorganStanley.ComposeUI.Fdc3.DesktopAgent.Client
```


## Setup
1.	Register IDesktopAgent in your DI container:
```csharp
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// Register IMessaging and other dependencies first
services.AddSingleton<IMessaging, YourMessagingImplementation>();

// Register DesktopAgentClient as IDesktopAgent
services.AddFdc3DesktopAgentClient();
```

2.	Configure environment variables:
Set the following environment variables before starting your application:
- AppId (your app identifier)
- InstanceId (unique instance identifier)

3.	Use IDesktopAgent in your application:
```csharp
public class YourService
{
    private readonly IDesktopAgent _desktopAgent;

    public YourService(IDesktopAgent desktopAgent)
    {
        _desktopAgent = desktopAgent;
    }

    public async Task SendContext(IContext context)
    {
        await _desktopAgent.Broadcast(context);
    }
}
```

## Usage
### Broadcasting Context
An app is not able to broadcast any message unless it has joined a channel. After joining a channel, you can broadcast context to all listeners in that channel.
```csharp
var context = new Instrument(new InstrumentID { Ticker = "test-instrument" }, "test-name");
await desktopAgent.Broadcast(context);
```

### Adding Context Listener
You may register context listeners either before or after joining a channel; however, listeners will not receive any context unless the DesktopAgent has joined a channel.
When an application is launched using the fdc3.open call with a context, the appropriate listener in the opened app will be invoked with that context before any other context messages are delivered.
```csharp
var listener = await desktopAgent.AddContextListener("fdc3.instrument", (ctx, ctxMetadata) =>
{
    Console.WriteLine($"Received context: {ctx}");
});
```

### Joining a Channel
Based on the standard you should join to a channel before broadcasting context to it or before you can receive the messages. If an app is joined to a channel every top-level already registered context listener will call its handler with the latest context on that channel.
```csharp
var channel = await desktopAgent.JoinChannel("fdc3.channel.1");
```

### Leaving a Channel
You can leave the current channel by calling the LeaveCurrentChannel method. After leaving a channel, the app will not receive any context messages until it joins another channel.
```csharp
await desktopAgent.LeaveCurrentChannel();
```

### Getting the Current Channel
You can get the current channel using the GetCurrentChannel method. If the app is not joined to any channel, it will return null.
```csharp
var currentChannel = await desktopAgent.GetCurrentChannel();
```

### Getting Info about the current App
You can get the metadata of the current app using the GetInfo method.
```csharp
var implementationMetadata = await desktopAgent.GetInfo();
```

### Getting Metadata of an App
You can get the metadata of an app using the GetAppMetadata method by providing the AppIdentifier.
```csharp
var appMetadata = await desktopAgent.GetAppMetadata(new AppIdentifier("your-app-id", "your-instance-id"));
```

### Getting User Channels
You can retrieve user channels by using:
```csharp
var channels = await desktopAgent.GetUserChannels();
await desktopAgent.JoinUserChannel(channels[0].Id);
```

### Getting or Creating a App Channel
You can get or create an app channel by using:
```csharp
var appChannel = await desktopAgent.GetOrCreateChannel("your-app-channel-id");
var listener = await appChannel.AddContextListener("fdc3.instrument", (ctx, ctxMetadata) =>
{
    Console.WriteLine($"Received context on app channel: {ctx}");
});

//Initiator shouldn't receive back the broadcasted context
await appChannel.Broadcast(context);
```

### Finding apps based on the specified intent
You can find/search for applications from the AppDirectory by using the `FindIntent` function:
```csharp
var apps = await desktopAgent.FindIntent("ViewChart", new Instrument(new InstrumentID { Ticker = "AAPL" }), "expected_resultType"));
```

### Finding instances for the specified app
You can find the currently FDC3 enabled instances for the specified app by using the `FindInstances` function:
```csharp
var instances = await desktopAgent.FindInstances("your-app-id");
```

### Finding intents by context
You can find the apps that can handle for the specified context by using the `FindIntentsByContext` function:
```csharp
var context = new Instrument(new InstrumentID { Ticker = "AAPL" }, "Apple Inc.");
var appIntents = await desktopAgent.FindIntentsByContext(context);
```

### Raising intent for context
You can raise an intent for the specified context by using the `RaiseIntentForContext` function and return its result by using the `GetResult` of the returned `IIntentResolution`:
```csharp
var intentResolution = await desktopAgent.RaiseIntentForContext(context, appIdentifier);
var intentResult = await intentResolution.GetResult();
```

### Adding Intent Listener
You can register an intent listener by using the `AddIntentListener` function:
```csharp
var currentChannel = await desktopAgent.GetCurrentChannel();
var listener = await desktopAgent.AddIntentListener<Instrument>("ViewChart", async (ctx, ctxMetadata) =>
{
    Console.WriteLine($"Received intent with context: {ctx}");
    return currentChannel;
});
```

### Raising intents
You can raise an intent by using the `RaiseIntent` function and return its result by using the `GetResult` of the returned `IIntentResolution`:
```csharp
var intentResolution = await desktopAgent.RaiseIntent("ViewChart", context, appIdentifier);
var intentResult = await intentResolution.GetResult();
```

### Opening an app
You can open an app by using the `Open` function:
```csharp
var appIdentifier = new AppIdentifier("your-app-id");
var instrument = new Instrument();

var appInstance = await desktopAgent.Open(appIdentifier, instrument);
//The opened app should handle the context if it has registered a listener for that context type; if it does not register its context listener in time the open call will fail
```

### Creating Private Channel
You can create a private channel by using the `CreatePrivateChannel` function:
```csharp
var privateChannel = await desktopAgent.CreatePrivateChannel("your-private-channel-id");
var contextListenerHandler = privateChannel.OnAddContextListener((ctx) => {
    Console.WriteLine($"Private channel context listener has been added for context: {ctx}");
});

var unsubscribeHandler = privateChannel.OnUnsubscribe((ctx) => {
    Console.WriteLine($"Private channel context listener has been unsubscribed for context: {ctx}");
});

var disconnectHandler = privateChannel.OnDisconnect(() => {
    Console.WriteLine("Private channel has been disconnected");
});
```

## Documentation

For more details, see the [ComposeUI documentation](https://morganstanley.github.io/ComposeUI/).

&copy; Morgan Stanley. See NOTICE file for additional information.