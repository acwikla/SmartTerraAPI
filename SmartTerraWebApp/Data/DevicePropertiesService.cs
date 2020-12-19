using Newtonsoft.Json;
using SmartTerraAPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SmartTerraWebApp.Data
{
    public class DevicePropertiesService
    {
        //get latest device properties
        public string RequestResultString = "";
        public async Task<string> GetLatestDeviceProperties(int deviceId)
        {
            string URL = $"http://localhost:5000/api/devices/{deviceId}/latestDeviceProperties";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            RequestResultString = client.GetStringAsync(URL).Result;
            if (RequestResultString == null)
            {
                return "There is no device properties in database";
            }

            return RequestResultString;
        }

        //get all device properties
        public async Task<DevicePropertiesDTO[]> GetAllDeviceProperties(int deviceId)
        {
            string URL = $"http://localhost:5000/api/Devices/{deviceId}/deviceProperties";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            var AllDeviceProp = new DevicePropertiesDTO[0];

            try
            {
                AllDeviceProp = await client.GetFromJsonAsync<DevicePropertiesDTO[]>(URL);
            }
            catch
            {
            }

            return AllDeviceProp;
        }
    }
}
