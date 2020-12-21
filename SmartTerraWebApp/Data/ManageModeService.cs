using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        public void PatchIsOnFlag()
        {

        }
        // POST: api/devices/{deviceId}/modes
        public void PostMode()
        {

        }
        // PUT: api/Modes/{modeId}
        public void ChangeMode()
        {

        }
    }
}
