using Newtonsoft.Json;

namespace MetroFayeClient.FayeObjects {
    [JsonObject(MemberSerialization.OptIn)]
    public class SubscribeRequest : FayeRequest {
        [JsonProperty("subscription")]
        public string Subscription;
    }
}
