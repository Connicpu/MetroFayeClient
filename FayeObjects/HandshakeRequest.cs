using Newtonsoft.Json;

namespace MetroFayeClient.FayeObjects {
    [JsonObject(MemberSerialization.OptIn)]
    public class HandshakeRequest : FayeObject {
        public HandshakeRequest() {
            Channel = "/meta/handshake";
            Id = "handshake";
        }

        [JsonProperty("version")]
        public string Version = "1.0";

        [JsonProperty("supportedConnectionTypes")]
        public string[] SupportedConnectionTypes = {"websocket"};
    }
}
