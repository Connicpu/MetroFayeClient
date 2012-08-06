using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MetroFayeClient.FayeObjects;
using Windows.Networking.Sockets;
using System.Threading;

namespace MetroFayeClient {
    public partial class FayeConnector {
        private MessageWebSocket _socket;
        public string ClientID;
        private readonly Dictionary<Guid, ManualResetEventSlim> _requestWaiters = new Dictionary<Guid, ManualResetEventSlim>();
        private readonly Dictionary<Guid, bool> _requestSuccesses = new Dictionary<Guid, bool>();
        private readonly Dictionary<Guid, string> _requestResponses = new Dictionary<Guid, string>();
        private bool _asyncHandshake;

        public event EventHandler<FayeConnector, FayeMessageEventArgs> MessageReceived;
        public event EventHandler<FayeConnector, HandshakeResponse> HandshakeComplete;
        public event EventHandler<FayeConnector, HandshakeResponse> HandshakeFailed;

        public MessageWebSocket Socket { get { return _socket; } }

        public bool Connected { get; private set; }

        public async void Connect(Uri address) {
            _subbedChans.Clear();
            _asyncHandshake = false;
            _socket = new MessageWebSocket();
            _socket.MessageReceived += FayeMessageReceived;
            _socket.Closed += (sender, args) => Connected = false;
            Connected = true;
            await _socket.ConnectAsync(address);
            Send(new HandshakeRequest());
        }

        public async Task<HandshakeResponse> ConnectAsync(Uri address) {
#if DEBUG
            Debug.WriteLine("Transitioning to a task");
#endif
            await Task.Delay(10);
            _asyncHandshake = true;
#if DEBUG
            Debug.WriteLine("Creating socket");
#endif
            _socket = new MessageWebSocket();
#if DEBUG
            Debug.WriteLine("Registering received message handler");
#endif
            _socket.MessageReceived += FayeMessageReceived;
            _socket.Closed += (sender, args) => Connected = false;
            Connected = true;
#if DEBUG
            Debug.WriteLine("Connecting socket");
#endif
            if (!_socket.ConnectAsync(address).AsTask().Wait(5000)) {
#if DEBUG
                Debug.WriteLine("Connection failed :(");
#endif
                _socket.Dispose();
                _socket = null;
                return null;
            }
#if DEBUG
            Debug.WriteLine("Generating GUID");
#endif
            var guid = Guid.NewGuid();
#if DEBUG
            Debug.WriteLine("Creating handshake request");
#endif
            var request = new HandshakeRequest {
                Id = guid.ToString()
            };

#if DEBUG
            Debug.WriteLine("Creating waiter");
#endif
            var waiter = new ManualResetEventSlim();
#if DEBUG
            Debug.WriteLine("Registering waiter");
#endif
            _requestWaiters[guid] = waiter;
#if DEBUG
            Debug.WriteLine("Sending request");
#endif
            await Send(request);
            if (!await Helpers.WaitAsync(waiter.WaitHandle, 5000)) {
#if DEBUG
                Debug.WriteLine("Response never received");
#endif
                _socket.Dispose();
                _socket = null;
                return null;
            }
#if DEBUG
            Debug.WriteLine("Deserializing response");
#endif
            var response = await Helpers.DeserializeAsync<HandshakeResponse>(_requestResponses[guid]);
#if DEBUG
            Debug.WriteLine("Clearing response data");
#endif
            ClearResponse(guid);
            if (response.Successful ?? false) {
                ClientID = response.ClientID;
                Send(new ConnectRequest());
            }
            return response;
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

        public ChannelMessage<T> Deserialize<T>() {
            return Helpers.Deserialize<ChannelMessage<T>>(Data);
        }
    }

    public class ServerResult : EventArgs {
        public string Data { get; internal set; }
        public bool Success { get; internal set; }
    }
}
