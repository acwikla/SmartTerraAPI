using Newtonsoft.Json;
using SmartTerraAPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartTerraWebApp.Data
{
    public class ManageModeService
    {
        // oninitial override: GET: api/devices/{deviceId}/modes=> if null =>alert:"Please add mode."
        public string requestResultString, reasonPhrase, contentValue;
        public HttpResponseMessage responseMessage;
        public async Task<string> GetMode(int deviceId)
        {
            string URL = $"http://localhost:5000/api/Devices/{deviceId}/modes";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            responseMessage = await client.GetAsync(URL);
            
            reasonPhrase = responseMessage.ReasonPhrase;
            if(reasonPhrase == "Bad Request")
            {
                contentValue = await responseMessage.Content.ReadAsStringAsync();
                return reasonPhrase;
            }

            requestResultString = client.GetStringAsync(URL).Result;
            if (requestResultString == null)
            {
                return "There is no device properties in database";
            }

            return requestResultString;
        }

        // PATCH: api/IsOn/{modeId}
        public async Task PatchIsOnFlag(int id, ModeIsOnDTO modeIsOnDTO)
        {
            string URL = $"http://localhost:5000/api/modes/IsOn/{id}";

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(new HttpMethod("PATCH"), URL))
            {
                var json = JsonConvert.SerializeObject(modeIsOnDTO);
                using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json-patch+json"))
                {
                    request.Content = stringContent;

                    using (var response = await client
                        .SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();
                    }
                }
            }
        }

        // POST: api/devices/{deviceId}/modes
        public async Task<HttpResponseMessage> PostMode( int deviceId, ModeAddDTO newMdeAddDTO)
        {
            // parse to json
            string newMdeAddString = Newtonsoft.Json.JsonConvert.SerializeObject(newMdeAddDTO);

            string URL = $"http://localhost:5000/api/devices/{deviceId}/modes";
            var client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // post 
            //JObject.Parse(newDevJobJSONString)
            HttpResponseMessage response = await client.PostAsJsonAsync(URL, newMdeAddDTO);

            return response;

        }
        // PUT: api/Modes/{modeId}
        public async Task<HttpResponseMessage> ChangeMode(int modeId, ModeDTO modeToUpdateDTO)
        {
            // parse to json
            string modeToUpdateString = Newtonsoft.Json.JsonConvert.SerializeObject(modeToUpdateDTO);

            string URL = $"http://localhost:5000/api/Modes/{modeId}";
            var client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // post 
            //JObject.Parse(newDevJobJSONString)
            HttpResponseMessage response = await client.PutAsJsonAsync(URL, modeToUpdateDTO);

            return response;
        }
    }
}
