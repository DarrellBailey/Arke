# Arke
Arke is an event driven and message based network library for .NET Core

#Features
###ArkeMessage
Each request is contained in what is called an "ArkeMessage". This object encapsulates the request and any associated data and is copied 1-to-1 on both the sender and receiver sides.
###Channels
Each request is transmitted on an individual channel. The channel is a 32 bit signed integer value that the receiving end can use to choose different paths or options for the request. All requests are given the default channel "0" but this value can be changed on the ArkeMessage. An application can have as many channels as there are values in a 32 bit integer.
###RequestReply
Alongside standard "send and forget" messages, Arke allows developers to send a request and asynchronously wait for a reply to that specific request. The Request-Reply pattern can also be used with Channels to provide even more control.

#Usage
The most basic network operation is to connect to a server. Lets connect to localhost at port 1000.

    ArkeTcpClient client = new ArkeTcpClient();
    client.Connect("localhost", 1000);
    
From the server side, we can start up a listener on port 1000 by doing the following.

    ArkeTcpServer server = new ArkeTcpServer(1000);
    server.StartListening();
  
Now we need an ArkeMessage that we can send to the server. An ArkeMessage can be constructed from both a byte array and string. (In the future a POCO can be used - See Roadmap)

    ArkeMessage message = new ArkeMessage("Hello World!");
    
It can also be constructed with a channel selection. In this example, channel 5 is used.

    ArkeMessage message = new ArkeMessage("Hello World!", 5);
    
To send a message we simply drop it into a connected client.

    ArkeTcpClient client = new ArkeTcpClient();
    client.Connect("localhost", 1000);
    ArkeMessage message = new ArkeMessage("Hello World!");
    client.Send(message);
    
The client can also send the message using .NET async/await pattern.

    ArkeTcpClient client = new ArkeTcpClient();
    client.Connect("localhost", 1000);
    ArkeMessage message = new ArkeMessage("Hello World!");
    await client.SendAsync(message);
    
On the server side, we need to set up a listener for incoming messages. The incoming message object is the same ArkeMessage object that was sent by the client. The connection is the connection to the client that sent the message.
    
    ArkeTcpServer server = new ArkeTcpServer(1000);
    server.StartListening();
    server.MessageReceived += (message, connection) =>
    {
        Console.WriteLine("Message: " + message.GetContentAsString());
    };

From this event we can process the message and use the given connection object to send some message back to the client.

    ArkeTcpServer server = new ArkeTcpServer(1000);
    server.StartListening();
    server.MessageReceived += (message, connection) =>
    {
        Console.WriteLine("Message: " + message.GetContentAsString());
        connection.Send(new ArkeMessage("Hello Client, I am Server!"));
    };
    
In the previous example we use the message received event to listen for incoming messages. We can also listen for messages for specific channels. For this, we will need to pass in a callback for that specific channel.


    

    

  

