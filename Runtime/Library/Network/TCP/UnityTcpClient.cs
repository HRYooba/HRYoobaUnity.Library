using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;

namespace HRYooba.Library.Network
{
    public class UnityTcpClient : System.IDisposable
    {
        private const int BufferSize = 1024;

        private TcpClient _client;
        private CancellationTokenSource _cancellation;

        private Subject<string> _onMessageReceived = new Subject<string>();
        private Subject<(string IpAddress, int Port)> _onServerClosed = new Subject<(string, int)>();
        private Subject<(string IpAddress, int Port, bool IsConnect)> _onConnected = new Subject<(string, int, bool)>();

        public UnityTcpClient()
        {
            OnServerClosed.Subscribe(endPoint => Debug.Log($"Server({endPoint.IpAddress}:{endPoint.Port}) closed."));
            OnConnected.Subscribe(connectData => Debug.Log($"Server({connectData.IpAddress}:{connectData.Port}) connect " + (connectData.IsConnect ? "success." : "failed.")));
        }

        ~UnityTcpClient()
        {
            Dispose();
        }

        public System.IObservable<string> OnMessageReceived
        {
            get { return _onMessageReceived.ObserveOnMainThread(); }
        }

        public System.IObservable<(string IpAddress, int Port)> OnServerClosed
        {
            get { return _onServerClosed.ObserveOnMainThread(); }
        }

        public System.IObservable<(string IpAddress, int Port, bool IsConnect)> OnConnected
        {
            get { return _onConnected.ObserveOnMainThread(); }
        }

        public void Connect(string ipAddress, int port)
        {
            if (_client != null)
            {
                Debug.LogWarning($"UnityTcpClient already connected server({ipAddress}:{port})");
                return;
            }

            if (_cancellation == null)
            {
                _cancellation = new CancellationTokenSource();
            }

            _client = new TcpClient();
            var connectTask = Task.Run(() =>
            {
                _client.Connect(ipAddress, port);
                var receiveTask = Task.Run(() => ReceiveAsync(_cancellation.Token));
            });
            connectTask.ContinueWith(completeTask =>
            {
                _onConnected.OnNext((ipAddress, port, _client.Connected));
            });
        }

        public void Disconnect()
        {
            if (_client != null && _client.Client != null && _client.Client.RemoteEndPoint != null)
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

            _onConnected.Dispose();
            _onConnected = null;
        }

        private async Task ReceiveAsync(CancellationToken cancellationToken)
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
                        var serverEndPoint = (IPEndPoint)_client.Client.RemoteEndPoint;
                        _onServerClosed.OnNext((serverEndPoint.Address.ToString(), serverEndPoint.Port));

                        Disconnect();
                        break;
                    }

                    // データ終了文字があれば読み取り完了
                    if (message.ToString().Contains("\n"))
                    {
                        var dataArray = message.ToString().Split(new[] { "\r\n", "\n", "\r" }, System.StringSplitOptions.None);
                        foreach (var data in dataArray)
                        {
                            if (data.Length > 0)
                            {
                                _onMessageReceived.OnNext(data.Replace("\n", "").ToString());
                            }
                        }
                        message = null; // リソース解放

                        // 再度データ受け取り待ちに
                        var receiveTask = Task.Run(() => ReceiveAsync(cancellationToken));

                        // データ受け取りスレッド終了
                        break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Debug.LogException(ex);
            }
        }
    }
}