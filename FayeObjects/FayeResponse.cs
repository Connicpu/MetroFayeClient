using Newtonsoft.Json;

namespace MetroFayeClient.FayeObjects {
    [JsonObject(MemberSerialization.OptIn)]
    public class FayeResponse : FayeObject {
        [JsonProperty("successful")]
        public bool? Successful;
    }
}
