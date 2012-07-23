using Newtonsoft.Json;

namespace MetroFayeClient.FayeObjects {
    [JsonObject(MemberSerialization.OptIn)]
    public class PublishRequest<T> : FayeRequest {
        public PublishRequest(string channel, T data) {
            Channel = channel;
            Data = data;
        }

        [JsonProperty("data")]
        public T Data;
    }
}
