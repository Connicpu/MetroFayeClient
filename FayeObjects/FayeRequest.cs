using Newtonsoft.Json;

namespace MetroFayeClient.FayeObjects {
    [JsonObject(MemberSerialization.OptIn)]
    public class FayeRequest : FayeObject {
        [JsonProperty("clientId")]
        public string ClientID;
    }
}
