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
            IPAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
            Client = client;
        }

        ~UnityTcpSession()
        {
            Dispose();
        }

        public Guid Id { get; }
        public IPAddress IPAddress { get; private set; }
        internal TcpClient Client { get; private set; }

        public void Dispose()
        {
            Client.Dispose();
            Client = null;
        }
    }
}