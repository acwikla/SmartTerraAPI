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
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

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
                 return "there is no jobs in database";
             }

             return ReuestResultString;
         }


        public async Task<Jobs[]> GetJobsAsync()
        {
            var DBJobs = new Jobs[0];

            const string URL = "http://localhost:5000/api/Jobs";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            try
                {
                    DBJobs = await client.GetFromJsonAsync<Jobs[]>(
                        URL);
                }
                catch
                {
                }
            return DBJobs;
        }


        public async Task<HttpResponseMessage> PostNewJob(int jobId, int deviceId, DateTime ExecutionTime, string Body)
        {
            var newDeviceJob = new DeviceJobAdd();
            newDeviceJob.ExecutionTime = ExecutionTime;
            newDeviceJob.Body = Body;

            string testObjc = "{ Body : " + Body + "}";

            string URL = $"http://localhost:5000/api/DeviceJobs/deviceId={deviceId}/jobId={jobId}";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            HttpResponseMessage response = await client.PostAsJsonAsync(URL, testObjc);

            return response;
        }
    }
}
