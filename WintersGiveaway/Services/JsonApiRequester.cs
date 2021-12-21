using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WintersGiveaway.Interfaces;

namespace WintersGiveaway.Services
{
    public class JsonApiRequester : IApiRequester
    {
        private readonly HttpClient client = new HttpClient();

        public async Task<T> MakeRequest<T>(HttpRequestMessage message) where T : class
        {
            var response = await client.SendAsync(message);
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonString); ;
        }
    }
}
