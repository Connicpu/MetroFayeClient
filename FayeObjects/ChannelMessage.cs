using Newtonsoft.Json;

namespace MetroFayeClient.FayeObjects {
    [JsonObject(MemberSerialization.OptIn)]
    public class ChannelMessage<T> : FayeResponse {
        [JsonProperty("data")]
        public T Data;
    }
}
