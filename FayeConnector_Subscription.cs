using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MetroFayeClient.FayeObjects;

namespace MetroFayeClient {
    public partial class FayeConnector {
        private readonly List<string> _subbedChans = new List<string>();

        public async Task<SubscribeResponse> Subscribe(string channel) {
            var guid = Guid.NewGuid();
            var request = new SubscribeRequest {
                Id = guid.ToString(),
                Channel = "/meta/subscribe",
                Subscription = channel,
            };
            var waiter = new ManualResetEventSlim();
            _requestWaiters[guid] = new ManualResetEventSlim();
            await Send(request);
            await Helpers.WaitAsync(waiter.WaitHandle);
            var response = await Helpers.DeserializeAsync<SubscribeResponse>(_requestResponses[guid]);
            ClearResponse(guid);
            return response;
        }
    }
}
