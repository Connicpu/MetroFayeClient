using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MetroFayeClient {
    public static class Helpers {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings {
            NullValueHandling = NullValueHandling.Ignore,
            CheckAdditionalContent = false,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateParseHandling = DateParseHandling.DateTime,
            ConstructorHandling = ConstructorHandling.Default,
            DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
        };

        public static string Serialize(object obj) {
            return JsonConvert.SerializeObject(obj, JsonSettings);
        }

        public static async Task<string> SerializeAsync(object obj) {
            return await JsonConvert.SerializeObjectAsync(obj, Formatting.None, JsonSettings);
        }

        public static T Deserialize<T>(string data) {
            data = data.Trim().TrimStart('[').TrimEnd(']');
            return JsonConvert.DeserializeObject<T>(data, JsonSettings);
        }

        public static async Task<T> DeserializeAsync<T>(string data) {
            return await JsonConvert.DeserializeObjectAsync<T>(data, JsonSettings);
        }

        public static Task WaitAsync(WaitHandle handle) {
            var task = new Task(() => handle.WaitOne());
            task.Start();
            return task;
        }

        public static Task<bool> WaitAsync(WaitHandle handle, int timeout) {
            var task = new Task<bool>(() => handle.WaitOne(timeout));
            task.Start();
            return task;
        }
    }

    public delegate void EventHandler<in TSender, in TArgs>(TSender sender, TArgs e);
}
