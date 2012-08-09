using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MetroFayeClient.FayeObjects;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace MetroFayeClient {
    public partial class FayeConnector {

        void FayeMessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args) {
            
            string message;
            using (var reader = args.GetDataReader()) {
                message = reader.ReadString(reader.UnconsumedBufferLength);
            }
            message = message.Trim().TrimStart('[').TrimEnd(']');

            var obj = Helpers.Deserialize<FayeResponse>(message);

            if (obj.Channel == "/meta/handshake" && !_asyncHandshake) {
                FinishHandshake(message);
                Send(new ConnectRequest());
                return;
            }

            if (obj.Channel == "/meta/connect") {
                Send(new ConnectRequest());
            }

            if (obj.Successful != null && obj.Id != null) {
                Guid guid;
                if (Guid.TryParse(obj.Id, out guid)) {
                    if (_requestWaiters.ContainsKey(guid)) {
                        _requestSuccesses[guid] = (bool)obj.Successful;
                        _requestResponses[guid] = message;
                        Debug.WriteLine("Doing set for response on " + obj.Channel);
                        _requestWaiters[guid].Set();
                        return;
                    }
                }
            }
            Event(MessageReceived, new FayeMessageEventArgs { Data = message, Channel = obj.Channel });
        }


        async Task Send(FayeObject o) {
            if (o is FayeRequest) 
                (o as FayeRequest).ClientID = ClientID;
            using (var writer = new DataWriter(_socket.OutputStream)) {
                writer.UnicodeEncoding = UnicodeEncoding.Utf8;
                var stringd = await Helpers.SerializeAsync(o);
                writer.WriteString(stringd);
                await writer.StoreAsync();
                await writer.FlushAsync();
                writer.DetachStream();
            }
        }
    }
}
