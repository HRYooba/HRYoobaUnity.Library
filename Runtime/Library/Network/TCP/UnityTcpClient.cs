using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;

namespace HRYooba.Library.Network
{
    public class UnityTcpClient : IDisposable
    {
        private const int BufferSize = 1024;

        private TcpClient _client;
        private CancellationTokenSource _cancellation;

        private Subject<string> _onMessageReceived = new Subject<string>();
        private Subject<IPEndPoint> _onServerClosed = new Subject<IPEndPoint>();

        public UnityTcpClient()
        {
            OnServerClosed.Subscribe(endPoint =>
            {
                var ipAddress = ((IPEndPoint)_client.Client.RemoteEndPoint).Address;
                var port = ((IPEndPoint)_client.Client.RemoteEndPoint).Port;
                Debug.Log($"Server({ipAddress}:{port}) closed.");
            });
        }

        ~UnityTcpClient()
        {
            Dispose();
        }

        public IObservable<string> OnMessageReceived
        {
            get { return _onMessageReceived.ObserveOnMainThread(); }
        }

        public IObservable<IPEndPoint> OnServerClosed
        {
            get { return _onServerClosed.ObserveOnMainThread(); }
        }

        public void Connect(string ipAddress, int port)
        {
            try
            {
                _client = new TcpClient(ipAddress, port);
                _cancellation = new CancellationTokenSource();
                var task = Task.Run(() => Receive(_cancellation.Token));
                Debug.Log($"UnityTcpClient connect server({ipAddress}:{port})");
            }
            catch (SocketException)
            {
                Debug.LogWarning($"UnityTcpClient not connect server({ipAddress}:{port})");
            }
        }

        public void Disconnect()
        {
            if (_client != null)
            {
                var ipAddress = ((IPEndPoint)_client.Client.RemoteEndPoint).Address;
                var port = ((IPEndPoint)_client.Client.RemoteEndPoint).Port;
                Debug.Log($"UnityTcpClient disconnect server({ipAddress}:{port})");
            }

            _client?.Dispose();
            _client = null;

            _cancellation?.Cancel();
            _cancellation?.Dispose();
            _cancellation = null;
        }

        public void Send(string message)
        {
            if (_client == null) return;

            var buffer = Encoding.UTF8.GetBytes(message + "\n");
            var stream = _client.GetStream();
            stream.Write(buffer, 0, buffer.Length);
        }

        public void Dispose()
        {
            Disconnect();

            _onMessageReceived.Dispose();
            _onMessageReceived = null;

            _onServerClosed.Dispose();
            _onServerClosed = null;
        }

        private async Task Receive(CancellationToken cancellationToken)
        {
            // データ受け取り
            try
            {
                var stream = _client.GetStream();
                var message = new StringBuilder();

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var buffer = new byte[BufferSize];
                    var bytesRead = await stream.ReadAsync(buffer, 0, BufferSize);

                    if (bytesRead > 0)
                    {
                        message.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                    }
                    else
                    {
                        // サーバークローズ
                        _onServerClosed.OnNext((IPEndPoint)_client.Client.RemoteEndPoint);

                        Disconnect();
                        break;
                    }

                    // データ終了文字があれば読み取り完了
                    if (message.ToString().Contains("\n"))
                    {
                        _onMessageReceived.OnNext(message.Replace("\n", "").ToString());
                        message = null; // リソース解放

                        // 再度データ受け取り待ちに
                        var task = Task.Run(() => Receive(cancellationToken));

                        // データ受け取りスレッド終了
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Debug.LogException(ex);
            }
        }
    }
}