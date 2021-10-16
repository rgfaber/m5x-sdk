using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace M5x.Camunda
{
    internal static class CamundaClientExtensions
    {
        public static async Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            return await Task.FromResult(
                JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync()));
        }

        public static async Task<HttpResponseMessage> PostAsJsonAsync(this HttpClient client, string uri,
            object request)
        {
            var content = JsonConvert.SerializeObject(
                request,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            );
            return await client.PostAsync(uri, new StringContent(content, Encoding.UTF8, "application/json"));
        }
    }
}