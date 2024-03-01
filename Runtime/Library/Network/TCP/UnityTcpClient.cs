using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using R3;

namespace HRYooba.Library.Network
{
    public class UnityTcpClient : IDisposable
    {
        private const int BufferSize = 1024;

        private TcpClient _client;
        private CancellationTokenSource _cancellation;

        private readonly Subject<string> _onMessageReceivedSubject = new();
        private readonly Subject<(string IpAddress, int Port)> _onServerClosedSubject = new();
        private readonly Subject<(string IpAddress, int Port, bool IsConnect)> _onConnectedSubject = new();

        public UnityTcpClient()
        {

        }

        ~UnityTcpClient()
        {
            Dispose();
        }

        public Observable<string> OnMessageReceivedObservable => _onMessageReceivedSubject.ObserveOnMainThread();
        public Observable<(string IpAddress, int Port)> OnServerClosedObservable => _onServerClosedSubject.ObserveOnMainThread();
        public Observable<(string IpAddress, int Port, bool IsConnect)> OnConnectedObservable => _onConnectedSubject.ObserveOnMainThread();
        public bool IsConnected => _client != null && _client.Connected;

        public void Connect(string ipAddress, int port)
        {
            if (_client != null)
            {
                throw new InvalidOperationException("[UnityTcpClient] Already connected.");
            }

            _cancellation ??= new CancellationTokenSource();

            _ = ConnectAsync(ipAddress, port, _cancellation.Token);
        }

        public void Disconnect()
        {
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
            }
            catch
            {
                throw;
            }
        }

        public void Dispose()
        {
            Disconnect();
            _onMessageReceivedSubject.Dispose();
            _onServerClosedSubject.Dispose();
            _onConnectedSubject.Dispose();
        }

        private void PublishServerClosed()
        {
            if (_client != null && _client.Client != null && _client.Client.RemoteEndPoint != null)
            {
                var serverEndPoint = (IPEndPoint)_client.Client.RemoteEndPoint;
                _onServerClosedSubject.OnNext((serverEndPoint.Address.ToString(), serverEndPoint.Port));
            }
        }

        private async Task ConnectAsync(string ipAddress, int port, CancellationToken cancellationToken)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(ipAddress, port);
                _onConnectedSubject.OnNext((ipAddress, port, true));
            }
            catch (SocketException)
            {
                _onConnectedSubject.OnNext((ipAddress, port, false));
                Disconnect();
            }
            catch
            {
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
                        var dataArray = dataBuilder.ToString().Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
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
                                    _onMessageReceivedSubject.OnNext(message);
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