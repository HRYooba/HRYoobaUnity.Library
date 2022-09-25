using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace HRYooba.Library.Network
{
    public class UnityTcpServer : IDisposable
    {
        private const int BufferSize = 1024;

        private TcpListener _listener;
        private List<UnityTcpSession> _sessions = new List<UnityTcpSession>();
        private CancellationTokenSource _cancellation;
        private bool _useDebugLog;

        private Subject<UnityTcpSession> _onSessionConnected = new Subject<UnityTcpSession>();
        private Subject<UnityTcpSession> _onSessionDisconnected = new Subject<UnityTcpSession>();
        private Subject<(UnityTcpSession Session, string Message)> _onMessageReceived = new Subject<(UnityTcpSession, string)>();

        public UnityTcpServer(bool useDebugLog = false)
        {
            _useDebugLog = useDebugLog;
        }

        ~UnityTcpServer()
        {
            Dispose();
        }

        public IObservable<UnityTcpSession> OnSessionConnected
        {
            get { return _onSessionConnected.ObserveOnMainThread(); }
        }

        public IObservable<UnityTcpSession> OnSessionDisconnected
        {
            get { return _onSessionDisconnected.ObserveOnMainThread(); }
        }

        public IObservable<(UnityTcpSession Session, string Message)> OnMessageReceived
        {
            get { return _onMessageReceived.ObserveOnMainThread(); }
        }

        public void Open(int port)
        {
            if (_useDebugLog) Debug.Log($"UnityTcpServer port({port}) open.");

            var localEndPoint = new IPEndPoint(IPAddress.Any, port);
            _listener = new TcpListener(localEndPoint);
            _listener.Start();

            if (_cancellation == null)
            {
                _cancellation = new CancellationTokenSource();
            }

            // 別スレッドで接続待機を行う
            var listenTask = Task.Run(() => ListenAsync(_cancellation.Token));
        }

        public void Close()
        {
            Send("unityTcpServerClose");

            if (_listener != null)
            {
                var port = ((IPEndPoint)_listener.LocalEndpoint).Port;
                if (_useDebugLog) Debug.Log($"UnityTcpServer port({port}) close.");
            }

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
                    _onSessionDisconnected.OnNext(session);
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
                _onSessionDisconnected.OnNext(session);
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

            _onSessionConnected.Dispose();
            _onSessionConnected = null;

            _onSessionDisconnected.Dispose();
            _onSessionDisconnected = null;

            _onMessageReceived.Dispose();
            _onMessageReceived = null;
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
                    _onSessionConnected.OnNext(session);

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
                        _onSessionDisconnected.OnNext(session);
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
                                _onMessageReceived.OnNext((session, message));
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