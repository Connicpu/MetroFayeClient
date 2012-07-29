using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MetroFayeClient.FayeObjects {
    [JsonObject(MemberSerialization.OptIn)]
    public class EventMessage<T> : FayeResponse {
        [JsonProperty("data")]
        public T Data;
    }
}
