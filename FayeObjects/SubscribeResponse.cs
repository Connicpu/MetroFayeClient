using Newtonsoft.Json;

namespace MetroFayeClient.FayeObjects {
    [JsonObject(MemberSerialization.OptIn)]
    public class SubscribeResponse : FayeResponse {
        [JsonProperty("subscription")]
        public string Subscription;
        [JsonProperty("error")]
        public string Error;
    }
}
