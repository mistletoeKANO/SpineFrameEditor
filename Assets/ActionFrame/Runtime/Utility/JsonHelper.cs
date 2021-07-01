using Newtonsoft.Json;

namespace ActionFrame.Runtime
{
    public static class JsonHelper
    {
        private static JsonSerializerSettings m_JsonSetting = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented,
        };

        public static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, m_JsonSetting);
        }

        public static T ToObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, m_JsonSetting);
        }
    }
}