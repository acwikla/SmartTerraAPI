using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using SmartTerraWebApp.Data;
using SmartTerraAPI.DTO;
using System.Net.Http;
using System.Net.Http.Json;
using SmartTerra.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace SmartTerraWebApp.Data
{
    public class UserService
    {
        //login
        public async Task<UserDTO> LogIn(UserToLogInDTO userToLogInDTO)
        {
            string URL = $"http://localhost:5000/api/Users/login";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);
            HttpResponseMessage responseMessage = new HttpResponseMessage();

            try
            {
                responseMessage = await client.PostAsJsonAsync(URL, userToLogInDTO);
            }
            catch
            {
            }

            UserDTO user = new UserDTO();
            if (responseMessage.IsSuccessStatusCode)
            {
                user = await GetUser(userToLogInDTO);
            }

            else
            {
                user = null;
            }
            return user;
        }

        public async Task<UserDTO> GetUser( UserToLogInDTO userToLogInDTO)
        {
            string URL = $"http://localhost:5000/api/Users";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            //get all users
            UserDTO[] users = new UserDTO[0];
            try
            {
                users = await client.GetFromJsonAsync<UserDTO[]>(URL);
            }
            catch
            {
            }

            //find proper user
            UserDTO user = new UserDTO();
            foreach (var u in users)
            {
                if (u.Email.Equals(userToLogInDTO.Email))
                {
                    user.Id = u.Id;
                    user.Login = u.Login;
                    user.Email = u.Email;
                }
            }

            return user;
        }

        //getdevices
        public async Task<DeviceDTO[]> GetDevices(int userId)
        {
            string URL = $"http://localhost:5000/api/Users/{userId}/devices";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            //get all users
            DeviceDTO[] devices = new DeviceDTO[0];
            try
            {
                devices = await client.GetFromJsonAsync<DeviceDTO[]>(URL);
            }
            catch
            {
            }

            return devices;
        }
        //postdevice
        public async Task<HttpResponseMessage> PostNewDevice(int userId, string name)
        {
            // create DeviceJob object
            var newDevice = new DeviceAddDTO { Name = name };
            // parse to json
            string newDevJSONString = Newtonsoft.Json.JsonConvert.SerializeObject(newDevice);

            string URL = $"http://localhost:5000/api/users/{userId}/devices";
            var client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // post 
            //JObject.Parse(newDevJSONString)
            HttpResponseMessage response = await client.PostAsJsonAsync(URL, newDevice);

            return response;
        }

    }
}
