using System;
using System.Threading;
using System.Threading.Tasks;
using MetroFayeClient.FayeObjects;

namespace MetroFayeClient {
    public partial class FayeConnector {
        public async Task<bool> Publish<T>(string channel, T data) {
            var guid = Guid.NewGuid();
            var request = new PublishRequest<T>(channel, data) { Id = guid.ToString() };
            var waiter = new ManualResetEventSlim();
            _requestWaiters[guid] = waiter;
            await Send(request);
            waiter.Wait();
            var result = _requestSuccesses[guid];
            ClearResponse(guid);
            return result;
        }
    }
}
