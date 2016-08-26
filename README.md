# Arke [![license](https://img.shields.io/github/license/mashape/apistatus.svg?maxAge=2592000)]() 
Arke is an event driven and message based network library for .NET Core. Arke is written in fully managed c#, has no external dependencies, and relies on nothing but the .NET framework itself.

#Features
###ArkeMessage
Each request is contained in what is called an "ArkeMessage". This object encapsulates the request and any associated data and is copied 1-to-1 on both the sender and receiver sides.
###Channels
Each request is transmitted on an individual channel. The channel is a 32 bit signed integer value that the receiving end can use to choose different paths or options for the request. All requests are given the default channel "0" but this value can be changed on the ArkeMessage. An application can have as many channels as there are values in a 32 bit integer.
###Request Response
Alongside standard "send and forget" messages, Arke allows developers to send a request and asynchronously wait for a reply to that specific request. The Request-Response pattern can also be used with Channels to provide even more control.

#Install

Get the NuGet Package:

```PM> Install-Package Arke```

[View on NuGet](https://www.nuget.org/packages/Arke/)

#Usage
####1.Basic Messages
The most basic network operation is to connect to a server. Lets connect to localhost at port 1000.
```c#
ArkeTcpClient client = new ArkeTcpClient();
client.Connect("localhost", 1000);
```
From the server side, we can start up a listener on port 1000 by doing the following.
```c#
ArkeTcpServer server = new ArkeTcpServer(1000);
server.StartListening();
```
Now we need an ArkeMessage that we can send to the server. An ArkeMessage can be constructed from both a byte array and string. (In the future a POCO can be used - See Roadmap)
```c#
ArkeMessage message = new ArkeMessage(new byte[100]);
ArkeMessage message = new ArkeMessage("Hello World!");
```
It can also be constructed with a channel selection. In this example, channel 5 is used.
```c#
ArkeMessage message = new ArkeMessage(new byte[100], 5);
ArkeMessage message = new ArkeMessage("Hello World!", 5);
```
To get data from the ArkeMessage we use the GetContent methods.
```c#
bytes[] b = message.GetContentAsBytes();
string s = message.GetContentAsString();
```
To send a message we simply drop it into a connected client.
```c#
ArkeTcpClient client = new ArkeTcpClient();
client.Connect("localhost", 1000);
ArkeMessage message = new ArkeMessage("Hello World!");
client.Send(message);
``` 
The client can also send the message using .NET async/await pattern.
```c#
ArkeTcpClient client = new ArkeTcpClient();
client.Connect("localhost", 1000);
ArkeMessage message = new ArkeMessage("Hello World!");
await client.SendAsync(message);
```  
On the server side, we need to set up a listener for incoming messages. The incoming message object is the same ArkeMessage object that was sent by the client. The connection is the connection to the client that sent the message.
```c#
server.MessageReceived += (message, connection) =>
{
    Console.WriteLine("Message: " + message.GetContentAsString());
};
```
From this event we can process the message and use the given connection object to send some message back to the client.
```c#
server.MessageReceived += (message, connection) =>
{
    Console.WriteLine("Message: " + message.GetContentAsString());
    connection.Send(new ArkeMessage("Hello Client, I am Server!"));
};
```
In the previous example we use the message received event to listen for incoming messages. We can also listen for messages for specific channels. For this, we will need to pass in a callback for that specific channel.
```c#
server.RegisterChannelCallback(5, (message, connection) =>
{
    Console.WriteLine("Message: " + message.GetContentAsString());
    connection.Send(new ArkeMessage("Hello Client, I am Server!"));
});
```

You can pass in as many callbacks for each channel as you would like, and each will get called in the order that they were registered. Note: Every message that is not given a specific channel assumes the channel 0. Registering a callback on channel 0 will make that callback be called for every non assigned message.

####2.Request Response

Creating a request and waiting for a response is just as easy as sending a fire and forget message. 

On the client side:
```c#
ArkeMessage message = new ArkeMessage("Hello Server! Give Me A Response");
ArkeMessage response = client.SendRequest(message);
```
Thats it! The client waits for the server to return a response before proceeding.

Of course, we can also do it the async/await way:
```c#
ArkeMessage message = new ArkeMessage("Hello Server! Give Me A Response");
ArkeMessage response = await client.SendRequestAsync(message);
```

On the server, we register a request callback just like we register a channel callback.
```c#
server.RegisterRequestResponseCallback(async (message, connection) =>
{
    Console.WriteLine("Message: " + message.GetContentAsString());
    return new ArkeMessage("Hello Client, This Is A Response!");
});
```
The callback delegate recieves the message and connection just like other callbacks, except for a Request Response callback it also needs to return a `Task<ArkeMessage>`. The message is then returned to the sender as the response.

Finally, we can even register Request Response callbacks against different channels for even more control.
```c#
server.RegisterRequestResponseChannelCallback(5, async (message, connection) =>
{
    Console.WriteLine("Message: " + message.GetContentAsString());
    return new ArkeMessage("Hello Client, This Is A Response!");
});
```

#Roadmap
1. Create an ArkeMessage from any object. This requires some serialization support which should be reintroduced to .Net Core sometime soon.
2. Add Publish/Subscribe functionality.  There are many scenarios where, in a networked application, a client may want to subscribe to events on the server. While this could be done in a very rudimentary and inefficient way using channels, introducing true publish/subscribe functionality would be far better.

#Contributing
1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D




    

    

  

