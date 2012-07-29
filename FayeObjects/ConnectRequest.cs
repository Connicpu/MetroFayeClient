using Newtonsoft.Json;

namespace MetroFayeClient.FayeObjects {
    [JsonObject(MemberSerialization.OptIn)]
    public class ConnectRequest : FayeRequest {
        public ConnectRequest() {
            Channel = "/meta/connect";
        }

        [JsonProperty("connectionType")]
        public string ConnectionType = "websocket";
    }
}
