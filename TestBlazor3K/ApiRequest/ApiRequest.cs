using System.Text.Json;
using Microsoft.Extensions.Logging;
using TestBlazor3K.ApiRequest.Model;

namespace TestBlazor3K.ApiRequest
{
    public class ApiRequestService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiRequestService> _logger;

        public ApiRequestService(HttpClient httpClient, ILogger<ApiRequestService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<UserData> GetAllUsersAsync()
        {
            var url = "api/UsersLogins/getAllUsers";

            try
            {
                var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (string.IsNullOrEmpty(responseContent))
                {
                    _logger.LogWarning("Ответ от сервера пуст.");
                    return new UserData();
                }

                var usersData = JsonSerializer.Deserialize<UserData>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return usersData ?? new UserData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при запросе");
                return new UserData();
            }
        }

        public async Task<UserAddData> AddNewUser(ReqDataUser user)
        {
            var url = "api/UsersLogins/createNewUserAndLogin";

            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, user);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                var userAddData = JsonSerializer.Deserialize<UserAddData>(responseContent);

                return userAddData ?? new UserAddData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при запросе: {ex.Message}");
                return new UserAddData();
            }
        }

        public async Task<CurUser> AuthorizationServ(string login, string password)
        {
            var url = "api/UsersLogins/Authorization";

            var userLoginData = new UserLoginType()
            {
                login = login,
                password = password
            };
            if (userLoginData == null) return new CurUser();

            var response = await _httpClient.PostAsJsonAsync(url, userLoginData);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            var userAddData = JsonSerializer.Deserialize<CurUser>(responseContent);
            return userAddData ?? new CurUser();
        }

        public async Task<UserDataShort> UpdateUser(UserDataShort user)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/UsersLogins/updateUser/{user.id_User}", user);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserDataShort>();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                return null;
            }
        }

        public async Task<ApiResponse> DeleteUser(int userId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/UsersLogins/deleteUser/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse
                    {
                        status = true,
                        message = "User deleted successfully"
                    };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new ApiResponse
                    {
                        status = false,
                        message = $"Failed to delete user: {errorContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    status = false,
                    message = $"Error deleting user: {ex.Message}"
                };
            }
        }


        public async Task<ApiResponse> RegisterUser(CreateNewUserAndLogin registrationModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/UsersLogins/createNewUserAndLogin", registrationModel);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ApiResponse>();
                }
                else
                {
                    return new ApiResponse { status = false, message = "Ошибка при регистрации" };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse { status = false, message = ex.Message };
            }
        }


    }

    public class CreateNewUserAndLogin
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
    public class ApiResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
    }
    public class UserLoginType
    {
        public string login { get; set; }
        public string password { get; set; }
    }
}
