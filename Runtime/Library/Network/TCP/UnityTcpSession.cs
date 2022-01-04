using System;
using System.Net;
using System.Net.Sockets;

namespace HRYooba.Library.Network
{
    public class UnityTcpSession : IDisposable
    {
        public UnityTcpSession(TcpClient client)
        {
            Id = Guid.NewGuid();
            IPAddress = ((IPEndPoint)client.Client.LocalEndPoint).Address;
            Client = client;
        }

        ~UnityTcpSession()
        {
            Dispose();
        }

        public Guid Id { get; }
        public IPAddress IPAddress { get; }
        internal TcpClient Client { get; }

        public void Dispose()
        {
            Client.Dispose();
        }
    }

    public struct UnityTcpSessionMessage
    {
        public UnityTcpSessionMessage(Guid id, string message)
        {
            Id = id;
            Message = message;
        }

        public Guid Id { get; }
        public string Message { get; }
    }
}