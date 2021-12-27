using System;
using System.Net.Sockets;

public class UnityTcpSession : IDisposable
{
    public UnityTcpSession(TcpClient client)
    {
        Id = Guid.NewGuid();
        Client = client;
    }

    ~UnityTcpSession()
    {
        Dispose();
    }

    public Guid Id { get; }
    internal TcpClient Client { get; }

    public void Dispose()
    {
        Client.Dispose();
    }
}
