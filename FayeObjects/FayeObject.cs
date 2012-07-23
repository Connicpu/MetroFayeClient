using Newtonsoft.Json;

namespace MetroFayeClient.FayeObjects {
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class FayeObject {
        [JsonProperty("channel")]
        public string Channel;

        [JsonProperty("id")]
        public string Id;

        public string Serialize() {
            return Helpers.Serialize(this);
        }
    }
}
