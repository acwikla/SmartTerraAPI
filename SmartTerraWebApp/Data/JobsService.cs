using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;
using SmartTerraAPI.DTO;

namespace SmartTerraWebApp.Data
{
    public class JobsService
    {

        public string ReuestResultString = "";

        public JobsService()
        {
        }
        
        public string GetJobs()
         {
            const string URL = "http://localhost:5000/api/Jobs";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

             ReuestResultString = client.GetStringAsync(URL).Result;
             if (ReuestResultString==null)
             {
                 return "There is no jobs in database";
             }

             return ReuestResultString;
         }


        public async Task<JobDTO[]> GetJobsAsync()
        {
            var DBJobs = new JobDTO[0];

            const string URL = "http://localhost:5000/api/Jobs";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            try
                {
                    DBJobs = await client.GetFromJsonAsync<JobDTO[]>(
                        URL);
                }
                catch
                {
                }
            return DBJobs;
        }


        public async Task<HttpResponseMessage> PostNewJob(int jobId, int deviceId, DateTime ExecutionTime, string Body)
        {
            // create DeviceJob object
            var newDeviceJob = new DeviceJobAddDTO { ExecutionTime = ExecutionTime, Body = Body };
            // parse to json
            string newDevJobJSONString = Newtonsoft.Json.JsonConvert.SerializeObject(newDeviceJob);

            string URL = $"http://localhost:5000/api/DeviceJobs/deviceId={deviceId}/jobId={jobId}";
            var client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // post 
            //JObject.Parse(newDevJobJSONString)
            HttpResponseMessage response = await client.PostAsJsonAsync(URL, newDeviceJob);

            return response;
        }
    }
}
