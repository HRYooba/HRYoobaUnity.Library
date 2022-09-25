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
    public class UnityTcpClient : System.IDisposable
    {
        private const int BufferSize = 1024;

        private TcpClient _client;
        private CancellationTokenSource _cancellation;
        private CompositeDisposable _disposables = new();
        private bool _useDebugLog;

        private Subject<string> _onMessageReceived = new Subject<string>();
        private Subject<(string IpAddress, int Port)> _onServerClosed = new Subject<(string, int)>();
        private Subject<(string IpAddress, int Port, bool IsConnect)> _onConnected = new Subject<(string, int, bool)>();

        public UnityTcpClient(bool useDebugLog = false)
        {
            _useDebugLog = useDebugLog;
            if (_useDebugLog)
            {
                OnServerClosed.Subscribe(endPoint => Debug.Log($"Server({endPoint.IpAddress}:{endPoint.Port}) closed.")).AddTo(_disposables);
                OnConnected.Subscribe(connectData => Debug.Log($"Server({connectData.IpAddress}:{connectData.Port}) connect " + (connectData.IsConnect ? "success." : "failed."))).AddTo(_disposables);
            }
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

        public bool IsConnected
        {
            get { return _client == null ? false : _client.Connected; }
        }

        public void Connect(string ipAddress, int port)
        {
            if (_client != null)
            {
                if (_useDebugLog) Debug.LogWarning($"UnityTcpClient already connected server({ipAddress}:{port})");
                return;
            }

            if (_cancellation == null)
            {
                _cancellation = new CancellationTokenSource();
            }

            var connectTask = ConnectAsync(ipAddress, port, _cancellation.Token);
        }

        public void Disconnect()
        {
            if (_client != null && _client.Client != null && _client.Client.RemoteEndPoint != null)
            {
                var ipAddress = ((IPEndPoint)_client.Client.RemoteEndPoint).Address;
                var port = ((IPEndPoint)_client.Client.RemoteEndPoint).Port;
                if (_useDebugLog) Debug.Log($"UnityTcpClient disconnect server({ipAddress}:{port})");
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
            try
            {
                var stream = _client.GetStream();
                stream.Write(buffer, 0, buffer.Length);
            }
            catch (InvalidOperationException)
            {
                PublishServerClosed();
                Disconnect();
                throw;
            }
        }

        public void Dispose()
        {
            Disconnect();

            _disposables.Dispose();

            _onMessageReceived.Dispose();
            _onMessageReceived = null;

            _onServerClosed.Dispose();
            _onServerClosed = null;

            _onConnected.Dispose();
            _onConnected = null;
        }

        private void PublishServerClosed()
        {
            var serverEndPoint = (IPEndPoint)_client.Client.RemoteEndPoint;
            _onServerClosed.OnNext((serverEndPoint.Address.ToString(), serverEndPoint.Port));
        }

        private async Task ConnectAsync(string ipAddress, int port, CancellationToken cancellationToken)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(ipAddress, port);
                _onConnected.OnNext((ipAddress, port, true));
            }
            catch
            {
                _onConnected.OnNext((ipAddress, port, false));
                Disconnect();
                throw;
            }
            cancellationToken.ThrowIfCancellationRequested();

            var receiveTask = Task.Run(() => ReceiveAsync(_cancellation.Token));
        }

        private async Task ReceiveAsync(CancellationToken cancellationToken)
        {
            // データ受け取り
            try
            {
                var stream = _client.GetStream();
                var dataBuilder = new StringBuilder();

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var buffer = new byte[BufferSize];
                    var bytesRead = await stream.ReadAsync(buffer, 0, BufferSize, cancellationToken);

                    if (bytesRead > 0)
                    {
                        dataBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                    }
                    else
                    {
                        // サーバークローズ
                        PublishServerClosed();
                        Disconnect();
                        break;
                    }

                    // データ終了文字があれば読み取り完了
                    if (dataBuilder.ToString().Contains("\n"))
                    {
                        var dataArray = dataBuilder.ToString().Split(new[] { "\r\n", "\n", "\r" }, System.StringSplitOptions.None);
                        foreach (var data in dataArray)
                        {
                            if (data.Length > 0)
                            {
                                var message = data.Replace("\n", "").ToString();

                                // サーバークローズのメッセージなら
                                if (message == "unityTcpServerClose")
                                {
                                    PublishServerClosed();
                                    Disconnect();
                                }
                                else
                                {
                                    _onMessageReceived.OnNext(message);
                                }
                            }
                        }
                        dataBuilder = null; // リソース解放

                        // 再度データ受け取り待ちに
                        var receiveTask = Task.Run(() => ReceiveAsync(cancellationToken));

                        // データ受け取りスレッド終了
                        break;
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}