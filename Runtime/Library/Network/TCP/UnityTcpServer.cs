using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using R3;

namespace HRYooba.Library.Network
{
    public class UnityTcpServer : IDisposable
    {
        private const int BufferSize = 1024;

        private TcpListener _listener;
        private List<UnityTcpSession> _sessions = new();
        private CancellationTokenSource _cancellation;

        private readonly Subject<UnityTcpSession> _onSessionConnectedSubject = new();
        private readonly Subject<UnityTcpSession> _onSessionDisconnectedSubject = new();
        private readonly Subject<(UnityTcpSession Session, string Message)> _onMessageReceivedSubject = new();

        public UnityTcpServer()
        {

        }

        ~UnityTcpServer()
        {
            Dispose();
        }

        public Observable<UnityTcpSession> OnSessionConnectedObservable => _onSessionConnectedSubject.ObserveOnMainThread();
        public Observable<UnityTcpSession> OnSessionDisconnectedObservable => _onSessionDisconnectedSubject.ObserveOnMainThread();
        public Observable<(UnityTcpSession Session, string Message)> OnMessageReceivedObservable => _onMessageReceivedSubject.ObserveOnMainThread();

        public void Open(int port)
        {
            var localEndPoint = new IPEndPoint(IPAddress.Any, port);
            _listener = new TcpListener(localEndPoint);
            _listener.Start();

            _cancellation ??= new CancellationTokenSource();

            // 別スレッドで接続待機を行う
            var listenTask = Task.Run(() => ListenAsync(_cancellation.Token));
        }

        public void Close()
        {
            if (_listener == null) return;

            Send("unityTcpServerClose");

            var port = ((IPEndPoint)_listener.LocalEndpoint).Port;

            _cancellation?.Cancel();
            _cancellation?.Dispose();
            _cancellation = null;

            _listener?.Stop();
            _listener = null;

            foreach (var session in _sessions)
            {
                session.Dispose();
            }
            _sessions.Clear();
            _sessions = null;
        }

        public void Send(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message + "\n");

            foreach (var session in _sessions)
            {
                try
                {
                    var stream = session.Client.GetStream();
                    stream.Write(buffer, 0, buffer.Length);
                }
                catch (InvalidOperationException)
                {
                    _onSessionDisconnectedSubject.OnNext(session);
                    _sessions.Remove(session);
                    session.Dispose();
                }
                catch
                {
                    throw;
                }
            }
        }

        public void Send(UnityTcpSession session, string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message + "\n");

            try
            {
                var stream = session.Client.GetStream();
                stream.Write(buffer, 0, buffer.Length);
            }
            catch (InvalidOperationException)
            {
                _onSessionDisconnectedSubject.OnNext(session);
                _sessions.Remove(session);
                session.Dispose();
            }
            catch
            {
                throw;
            }
        }

        public void Dispose()
        {
            Close();

            _sessions = null;

            _onSessionConnectedSubject.Dispose();
            _onSessionDisconnectedSubject.Dispose();
            _onMessageReceivedSubject.Dispose();
        }

        private async Task ListenAsync(CancellationToken cancellationToken)
        {
            // クライアントの接続を常に待機
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    var session = new UnityTcpSession(client);
                    _sessions.Add(session);
                    _onSessionConnectedSubject.OnNext(session);

                    // 別スレッドでデータの受け取りを行う
                    var receiveTask = Task.Run(() => ReceiveAsync(session, cancellationToken));
                }
                catch
                {
                    throw;
                }
            }
        }

        private async Task ReceiveAsync(UnityTcpSession session, CancellationToken cancellationToken)
        {
            // データ受け取り
            try
            {
                var stream = session.Client.GetStream();
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
                        // セッション切断
                        _onSessionDisconnectedSubject.OnNext(session);
                        _sessions.Remove(session);
                        session.Dispose();
                        break;
                    }

                    // データ終了文字があれば読み取り完了
                    if (dataBuilder.ToString().Contains("\n"))
                    {
                        var dataArray = dataBuilder.ToString().Split('\n');
                        foreach (var data in dataArray)
                        {
                            if (data.Length > 0)
                            {
                                var message = data.Replace("\n", "").ToString();
                                _onMessageReceivedSubject.OnNext((session, message));
                            }
                        }
                        dataBuilder = null; // リソース解放

                        // 再度データ受け取り待ちに
                        var receiveTask = ReceiveAsync(session, cancellationToken);

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