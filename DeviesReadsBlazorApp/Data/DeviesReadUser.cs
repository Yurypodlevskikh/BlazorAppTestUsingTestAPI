using DeviesReadsBlazorApp.Mapping;
using DeviesReadsBlazorApp.Models;
using DeviesReadsBlazorApp.ViewModels;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DeviesReadsBlazorApp.Data
{
    public class DeviesReadUser : IDeviesReadUser
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseAddress;
        private readonly string _url;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IConfiguration _configuration;
        private readonly IBookDataService _bookDataService;

        public DeviesReadUser(IConfiguration configuration, IBookDataService bookDataService)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _baseAddress = _configuration.GetValue<string>("DeviesReadBaseAddress")!;
            _url = $"{_baseAddress}/users/";

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            _bookDataService = bookDataService;
        }
        public async Task<UserDTO> GetUserById(string UserId)
        {
            UserDTO user = new UserDTO();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{_url}{UserId}");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    user = JsonSerializer.Deserialize<UserDTO>(content, _jsonSerializerOptions)!;
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

            return user;
        }

        public async Task<List<UserBookViewModel>> GetUsersBooks(string UserId)
        {
            List<UserBookViewModel> userBooks = new();
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{_url}{UserId}");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var user = JsonSerializer.Deserialize<UserDTO>(content, _jsonSerializerOptions)!;
                    if (user.Shelf.Length > 0)
                    {
                        foreach (var book in user.Shelf)
                        {
                            BookDTO usersBook = await _bookDataService.GetBookByIdAsync(book.BookId);
                            
                            if (usersBook != null)
                            {
                                UserBookViewModel userBookToView = new()
                                {
                                    BookId = book.BookId,
                                    BookTitle = usersBook.Name,
                                    BookStatus = book.Status
                                };
                                userBooks.Add(userBookToView);
                            }
                        }
                    }
                }
                else
                {
                    Debug.WriteLine($"There was an error getting Http 2xx GetUsersBooks response: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetUsersBooks Exception: {ex.Message}");
            }

            return userBooks;
        }

        public async Task<bool> UserExists(string UserId)
        {
            if (string.IsNullOrEmpty(UserId))
                return false;

            HttpResponseMessage response = await _httpClient.GetAsync($"{_url}{UserId}");
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Check if the user has checked this book
        /// </summary>
        /// <param name="bookShelf"></param>
        /// <returns>The status of book</returns>
        public async Task<string?> UserHasThisBook(IfBookOnShelf bookShelf)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{_url}{bookShelf.UserId}");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var user = JsonSerializer.Deserialize<UserDTO>(content, _jsonSerializerOptions)!;
                    return user.Shelf.FirstOrDefault(x => x.BookId == bookShelf.BookId)?.Status;
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

            return null;
        }

        public async Task<bool> AddTheBookOnTheShelf(BookOnTheShelf bookShelf)
        {
            ShelfItem shelfItem = bookShelf.ToDTO()!;

            if (shelfItem != null)
            {
                try
                {
                    string jsonBookShelf = JsonSerializer.Serialize<ShelfItem>(shelfItem, _jsonSerializerOptions);
                    StringContent stringContent = new StringContent(jsonBookShelf, Encoding.UTF8, "application/json");
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", bookShelf.AccessToken);
                    HttpResponseMessage response = await _httpClient.PostAsync($"{_url}{bookShelf.UserId}/shelf", stringContent);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var rateBookResponse = JsonSerializer.Deserialize<UserDTO>(content, _jsonSerializerOptions)!;
                        return true;
                    }
                    else
                    {
                        Debug.WriteLine($"There was an error getting Http 2xx AddTheBookOnTheShelf response: {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"AddTheBookOnTheShelf Exception: {ex.Message}");
                }
            }

            return false;
        }

        public async Task<bool> UpdateBookOnTheShelf(BookOnTheShelf bookShelf)
        {
            ShelfItem shelfItem = bookShelf.ToDTO()!;

            if (shelfItem != null)
            {
                try
                {
                    string jsonBookShelf = JsonSerializer.Serialize<ShelfItem>(shelfItem, _jsonSerializerOptions);
                    StringContent stringContent = new StringContent(jsonBookShelf, Encoding.UTF8, "application/json");
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", bookShelf.AccessToken);
                    HttpResponseMessage response = await _httpClient.PutAsync($"{_url}{bookShelf.UserId}/shelf", stringContent);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var rateBookResponse = JsonSerializer.Deserialize<UserDTO>(content, _jsonSerializerOptions)!;
                        return true;
                    }
                    else
                    {
                        Debug.WriteLine($"There was an error getting Http 2xx UpdateBookOnTheShelf response: {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"UpdateBookOnTheShelf Exception: {ex.Message}");
                }
            }

            return false;
        }
    }
}
