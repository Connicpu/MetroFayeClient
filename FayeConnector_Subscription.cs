using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MetroFayeClient.FayeObjects;

namespace MetroFayeClient {
    public partial class FayeConnector {
        private readonly List<string> _subbedChans = new List<string>();

        public bool IsSubscribed(string channel) {
            return _subbedChans.Contains(channel);
        }

        public async Task<SubscribeResponse> Subscribe(string channel) {
            var guid = Guid.NewGuid();
            var request = new SubscribeRequest {
                Id = guid.ToString(),
                Channel = "/meta/subscribe",
                Subscription = channel,
            };
            var waiter = new ManualResetEventSlim();
            _requestWaiters[guid] = waiter;
            await Send(request);
            waiter.Wait();
            var response = await Helpers.DeserializeAsync<SubscribeResponse>(_requestResponses[guid]);
            if (response.Successful ?? false) _subbedChans.Add(channel);
            ClearResponse(guid);
            return response;
        }

        public async Task<SubscribeResponse> Unsubscribe(string channel) {
            var guid = Guid.NewGuid();
            var request = new SubscribeRequest {
                Id = guid.ToString(),
                Channel = "/meta/unsubscribe",
                Subscription = channel,
            };
            var waiter = new ManualResetEventSlim();
            _requestWaiters[guid] = waiter;
            await Send(request);
            waiter.Wait();
            var response = await Helpers.DeserializeAsync<SubscribeResponse>(_requestResponses[guid]);
            if (response.Successful ?? true) _subbedChans.Remove(channel);
            ClearResponse(guid);
            return response;
        }
    }
}
