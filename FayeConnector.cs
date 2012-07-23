using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MetroFayeClient.FayeObjects;
using Windows.Networking.Sockets;
using System.Threading;
using Windows.Storage.Streams;

namespace MetroFayeClient {
    public partial class FayeConnector {
        private MessageWebSocket _socket;
        public string ClientID;
        private readonly Dictionary<Guid, ManualResetEventSlim> _requestWaiters = new Dictionary<Guid, ManualResetEventSlim>();
        private readonly Dictionary<Guid, bool> _requestSuccesses = new Dictionary<Guid, bool>();
        private readonly Dictionary<Guid, string> _requestResponses = new Dictionary<Guid, string>(); 

        public event EventHandler<FayeConnector, FayeMessageEventArgs> MessageReceived;
        public event EventHandler<FayeConnector, HandshakeResponse> HandshakeComplete;
        public event EventHandler<FayeConnector, HandshakeResponse> HandshakeFailed;

        public bool Connected { get; private set; }

        public async void Connect(Uri address) {
            _socket = new MessageWebSocket();
            _socket.MessageReceived += FayeMessageReceived;
            _socket.Closed += (sender, args) => Connected = false;
            Connected = true;
            await _socket.ConnectAsync(address);
            Send(new HandshakeRequest());
        }

        public void FinishHandshake(string message) {
            var response = Helpers.Deserialize<HandshakeResponse>(message);
            if (!(response.Successful ?? false)) {
                Event(HandshakeFailed, response);
            } else {
                ClientID = response.ClientID;
                Event(HandshakeComplete, response);
            }
        }

        async Task Send(FayeObject o) {
            if (o is FayeRequest) (o as FayeRequest).ClientID = ClientID;
            using (var writer = new DataWriter(_socket.OutputStream)) {
                writer.UnicodeEncoding = UnicodeEncoding.Utf8;
                writer.WriteString(await Helpers.SerializeAsync(o));
                await writer.StoreAsync();
                await writer.FlushAsync();
                writer.DetachStream();
            }
        }

        void FayeMessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args) {
            string message;
            using (var reader = args.GetDataReader()) {
                message = reader.ReadString(reader.UnconsumedBufferLength);
            }
            message = message.Trim().TrimStart('[').TrimEnd(']');
            var obj = Helpers.Deserialize<FayeResponse>(message);

            if (obj.Channel == "/meta/handshake") {
                FinishHandshake(message);
                return;
            }

            if (obj.Channel == "/meta/connect") {
                Send(new ConnectRequest{ClientID = ClientID});
            }

            if (obj.Successful != null && obj.Id != null) {
                Guid guid;
                if (Guid.TryParse(obj.Id, out guid)) {
                    if (_requestWaiters.ContainsKey(guid)) {
                        _requestSuccesses[guid] = (bool)obj.Successful;
                        _requestResponses[guid] = message;
                        _requestWaiters[guid].Set();
                        return;
                    }
                } else {
                    return;
                }
            }
            Event(MessageReceived, new FayeMessageEventArgs { Data = message, Channel = obj.Channel });
        }

        void ClearResponse(Guid id) {
            _requestResponses.Remove(id);
            _requestSuccesses.Remove(id);
            _requestWaiters.Remove(id);
        }

        void Event<T>(EventHandler<FayeConnector, T> handler, T value) {
            if (handler != null) handler(this, value);
        }
    }

    public class FayeMessageEventArgs : EventArgs {
        public string Data { get; internal set; }
        public string Channel { get; internal set; }
    }

    public class ServerResult : EventArgs {
        public string Data { get; internal set; }
        public bool Success { get; internal set; }
    }
}
