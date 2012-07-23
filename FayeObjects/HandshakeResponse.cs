using Newtonsoft.Json;

namespace MetroFayeClient.FayeObjects {
    [JsonObject(MemberSerialization.OptIn)]
    public class HandshakeResponse : FayeResponse {
        [JsonProperty("clientId")]
        public string ClientID;

        [JsonProperty("version")]
        public string Version;

        [JsonProperty("supportedConnectionTypes")]
        public string[] SupportedConnectionTypes;

        [JsonProperty("error")]
        public string Error;
    }
}
