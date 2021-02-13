using WebApiRoutingDemo.API;
using WebApiRoutingDemo.API.Models;
using WebApiRoutingDemo.IntegrationTests.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace WebApiRoutingDemo.IntegrationTests.Controllers
{
    public class UserSettingsControllerTests
    {
        private HttpClient client;
        private LoginResponse _jwtToken;

        public UserSettingsControllerTests()
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configuration = (new ConfigurationBuilder().SetBasePath(projectDir).AddJsonFile("appsettings.json")).Build();
            var builder = new WebHostBuilder()
                .UseConfiguration(configuration)
                .UseEnvironment("Development")
                .UseStartup<Startup>();

            TestServer testServer = new TestServer(builder);
            client = testServer.CreateClient();
        }

        [Fact]
        public async Task UserSetting_Test()
        {
            // Arrange
            // Login as testuser1
            _jwtToken = await Login("testuser1");
            UserSettingModel setting = new UserSettingModel
            {
                UserSettingId = 4,
                UserId = "testuser1",
                SettingKey = "test key3",
                SettingValue = "test value3",
            };

            // This setting is invalid, since UserId in the UserSettingModel is marked as Required, but it is not provided in the object
            UserSettingModel invalidSetting = new UserSettingModel
            {
                UserSettingId = 5,
                SettingKey = "test key3",
                SettingValue = "test value3",
            };

            // Add
            HttpResponseMessage response = await Invoke_UserSetting_Api(HttpMethod.Post, "v1/usersettings", setting);
            response.EnsureSuccessStatusCode();

            // Add invalid setting would cause BadRequest response
            response = await Invoke_UserSetting_Api(HttpMethod.Post, "v1/usersettings", invalidSetting);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);

            // Get All
            response = await Invoke_UserSetting_Api(HttpMethod.Get, "v1/usersettings");
            response.EnsureSuccessStatusCode();

            // Get One
            response = await Invoke_UserSetting_Api(HttpMethod.Get, "v1/usersettings/1");
            response.EnsureSuccessStatusCode();
            setting = JsonConvert.DeserializeObject<UserSettingModel>(await response.Content.ReadAsStringAsync());

            // Update
            setting.SettingValue = "testvalue";
            response = await Invoke_UserSetting_Api(HttpMethod.Put, "v1/usersettings", setting);
            response.EnsureSuccessStatusCode();

            // Delete
            response = await Invoke_UserSetting_Api(HttpMethod.Delete, $"v1/usersettings/{setting.UserSettingId}");
            response.EnsureSuccessStatusCode();
        }

        private async Task<LoginResponse> Login(string name)
        {
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "v1/account/login")
            {
                Content = new JsonContent(new LoginRequest() { Username = name, Password = "Password@1" })
            };

            HttpResponseMessage response = await client.SendAsync(postRequest);
            response.EnsureSuccessStatusCode();

            // Get the response as a string
            return JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync());
        }

        private async Task<HttpResponseMessage> Invoke_UserSetting_Api(HttpMethod httpMethod, string apiUrl, UserSettingModel setting = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(httpMethod, apiUrl);
            if (setting != null)
            {
                request.Content = new JsonContent(setting);
            }
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken.Token);

            return await client.SendAsync(request).ConfigureAwait(false);
        }
    }
}