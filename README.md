# AvaloniaTCP

## AvaloniaTCP-v1.0.0：学习使用Avalonia/C#进行TCP通讯的一个简单Demo

## TCP通讯简介

TCP（传输控制协议，Transmission Control Protocol）是一种面向连接的、可靠的、基于字节流的传输层通信协议。它确保数据包按顺序传输，并在必要时进行重传，以保证数据的完整性和准确性。TCP通过三次握手建立连接，通过四次挥手释放连接，确保通信双方在传输数据前已准备好，并在传输结束后正确关闭连接。TCP广泛应用于需要高可靠性的网络应用，如网页浏览、文件传输和电子邮件等。

## Demo效果

启动两个应用，一个当服务端，一个当客户端。

开启服务端：

![image-20241014112528767](https://mingupupup.oss-cn-wuhan-lr.aliyuncs.com/imgs/image-20241014112528767.png)

开启客户端：

![image-20241014112558142](https://mingupupup.oss-cn-wuhan-lr.aliyuncs.com/imgs/image-20241014112558142.png)

客户端向服务端发送消息：

![image-20241014112646168](https://mingupupup.oss-cn-wuhan-lr.aliyuncs.com/imgs/image-20241014112646168.png)

服务端向客户端发送消息：

![image-20241014112730780](https://mingupupup.oss-cn-wuhan-lr.aliyuncs.com/imgs/image-20241014112730780.png)

## Demo代码

启动服务端：

```csharp
[RelayCommand]
private async Task StartServer()
{
    System.Net.IPAddress Ip = System.Net.IPAddress.Parse(IpAddress);
    _tcpServer = new TcpListener(Ip, Port);
    _tcpServer.Start();
    Message += "Server started. Waiting for a connection...\r\n";

    // 接受客户端连接
    _tcpServer_Client = await _tcpServer.AcceptTcpClientAsync();
    Message += "客户端已连接\r\n";

    // Handle client communication
    _ = HandleClientAsync(_tcpServer_Client);
}
```

```csharp
private async Task HandleClientAsync(TcpClient client)
{
    var stream = client.GetStream();
    var buffer = new byte[1024];
    int bytesRead;

    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
    {
        var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Message+=$"Received from client: {message}\r\n";

        // Echo the message back to the client
        //var response = Encoding.UTF8.GetBytes($"Echo: {message}");
        //await stream.WriteAsync(response, 0, response.Length);
    }

    Message += "Client disconnected...\r\n";
    stream.Close();
}
```

启动客户端：

```csharp
 [RelayCommand]
 private async Task StartClient()
 {
     System.Net.IPAddress Ip = System.Net.IPAddress.Parse(IpAddress);
     _tcpClient = new TcpClient();
     await _tcpClient.ConnectAsync(Ip, Port);
     Message += "Connected to server...\r\n";
     
     _ = HandleServerCommunicationAsync(_tcpClient);
 }
```

```csharp
private async Task HandleServerCommunicationAsync(TcpClient client)
{
    var stream = client.GetStream();
    var buffer = new byte[1024];
    int bytesRead;

    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
    {
        var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Message += $"Received from server: {message}\r\n";
       
    }

    Message += "Disconnected from server...\r\n";
    stream.Close();
}
```

向服务端发消息：

```csharp
 [RelayCommand]
 private async Task SendMessageToServer()
 {
     if (_tcpClient == null || !_tcpClient.Connected)
     {
         Message += "Not connected to server.\r\n";
         return;
     }

     var stream = _tcpClient.GetStream();
     var data = Encoding.UTF8.GetBytes(Text);
     await stream.WriteAsync(data, 0, data.Length);
     Message += $"Sent: {Text}\r\n";
 }
```

向客户端发消息：

```csharp
[RelayCommand]
private async Task SendMessageToClient()
{
    if (_tcpServer_Client == null || !_tcpServer_Client.Connected)
    {
        Message += "Not connected to client.\r\n";
        return;
    }

    var stream = _tcpServer_Client.GetStream();
    var data = Encoding.UTF8.GetBytes(Text);
    await stream.WriteAsync(data, 0, data.Length);
    Message += $"Sent: {Text}\r\n";
}
```

全部代码已上传至https://github.com/Ming-jiayou/AvaloniaTCP。

希望通过我的点滴分享，能够让对Avalonia感兴趣的朋友，更快入门。