using DeviesReadsBlazorApp.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace DeviesReadsBlazorApp.Data
{
    public class DeviesReadAuthentication : IDeviesReadAuthentication
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _baseAddress;
        private readonly string _url;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IDeviesReadUser _deviesReadUser;

        public DeviesReadAuthentication(
            IConfiguration configuration,
            IDeviesReadUser deviesReadUser)
        {
            _configuration = configuration;
            _deviesReadUser = deviesReadUser;
            _httpClient = new HttpClient();
            _baseAddress = _configuration.GetValue<string>("DeviesReadBaseAddress")!;
            _url = $"{_baseAddress}/auth";

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<UserAuthResponseDTO> Register(RegisterDTO user)
        {
            UserAuthResponseDTO authResponseDTO= new UserAuthResponseDTO();

            try
            {
                string jsonNewUser = JsonSerializer.Serialize<RegisterDTO>(user, _jsonSerializerOptions);
                StringContent stringContent = new StringContent(jsonNewUser, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync($"{_url}/register", stringContent);
                if(response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    authResponseDTO = JsonSerializer.Deserialize<UserAuthResponseDTO>(content, _jsonSerializerOptions)!;
                }
                else
                {
                    Debug.WriteLine($"There was an error getting Http 2xx response: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Exception: {ex.Message}");
            }

            return authResponseDTO;
        }

        public async Task<UserLocalStorage> Login(RegisterDTO user)
        {
            UserLocalStorage userLocalStorage = new UserLocalStorage();

            try
            {
                string jsonNewUser = JsonSerializer.Serialize<RegisterDTO>(user, _jsonSerializerOptions);
                StringContent stringContent = new StringContent(jsonNewUser, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync($"{_url}/login", stringContent);
                
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var authResponseDTO = JsonSerializer.Deserialize<UserAuthResponseDTO>(content, _jsonSerializerOptions)!;

                    if(!string.IsNullOrEmpty(authResponseDTO.UserId))
                    {
                        var currentUser = await _deviesReadUser.GetUserById(authResponseDTO.UserId);
                        if (currentUser != null)
                        {
                            userLocalStorage.UserId = authResponseDTO.UserId;
                            userLocalStorage.UserName = currentUser.Username;
                            userLocalStorage.AccessToken = authResponseDTO.AccessToken!;
                        }
                    }
                }
                else
                {
                    Debug.WriteLine($"There was an error getting Http 2xx response: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Exception: {ex.Message}");
            }

            return userLocalStorage;
        }

        public async Task<bool> IsLoggedIn(string accessToken)
        {
            if(!string.IsNullOrEmpty(accessToken))
            {
                try
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage response = await _httpClient.GetAsync($"{_baseAddress}/is-logged-in");
                    return response.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception: {ex.Message}");
                }
            }
            
            return false;
        }
    }
}
