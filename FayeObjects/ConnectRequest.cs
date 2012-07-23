using Newtonsoft.Json;

namespace MetroFayeClient.FayeObjects {
    [JsonObject(MemberSerialization.OptIn)]
    public class ConnectRequest : FayeRequest {
        [JsonProperty("connectionType")]
        public string ConnectionType = "websocket";
    }
}
