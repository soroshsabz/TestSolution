using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
namespace BSN.Resa.Commons
{
    public static class TemplateExtension
    {
        /// <summary>
        /// Returns object as Json string which used for web api.
        /// Notice that this serialization does not work with C# JavaScriptSerializationTool: [C# BUG]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
		public static string AsJsonContent<T>(this T t)
        {
            return new ObjectContent<T>(t, GlobalConfiguration.Configuration.Formatters.JsonFormatter).ReadAsStringAsync().Result;
        }

        public static string SerializeToJsonString<T>(this T t)
        {
            return JsonConvert.SerializeObject(t);
        }

        public static string SerializeToJsonString<T>(this T t, TypeNameHandling typeNameHandling)
        {
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = typeNameHandling
            };

            return JsonConvert.SerializeObject(t, jsonSerializerSettings);
        }

        public static T DeserializeFromJsonString<T>(this string t)
        {
            return JsonConvert.DeserializeObject<T>(t);
        }

        public static T DeserializeFromJsonString<T>(this string t, TypeNameHandling typeNameHandling)
        {
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = typeNameHandling
            };

            return JsonConvert.DeserializeObject<T>(t, jsonSerializerSettings);
        }
    }
}