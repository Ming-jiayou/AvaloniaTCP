using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Sockets;
using System.Threading.Tasks;
using System;
using System.Text;

namespace AvaloniaTCP.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string ipAddress = "127.0.0.1";

    [ObservableProperty]
    private int port = 5000;

    [ObservableProperty]
    private string message = " ";

    [ObservableProperty]
    private string text = "";

    TcpListener? _tcpServer;

    TcpClient? _tcpServer_Client;

    TcpClient? _tcpClient;

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

    [RelayCommand]
    private async Task StartClient()
    {
        System.Net.IPAddress Ip = System.Net.IPAddress.Parse(IpAddress);
        _tcpClient = new TcpClient();
        await _tcpClient.ConnectAsync(Ip, Port);
        Message += "Connected to server...\r\n";
        
        _ = HandleServerCommunicationAsync(_tcpClient);
    }

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

    [RelayCommand]
    private void CloseClient()
    {
        _tcpClient?.Close();
    } 
}
