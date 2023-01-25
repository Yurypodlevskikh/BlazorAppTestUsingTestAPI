using DeviesReadsBlazorApp.Mapping;
using DeviesReadsBlazorApp.Models;
using DeviesReadsBlazorApp.ViewModels;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace DeviesReadsBlazorApp.Data
{
    public class BookDataService : IBookDataService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseAddress;
        private readonly string _url;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IConfiguration _configuration;

        public BookDataService(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _baseAddress = _configuration.GetValue<string>("DeviesReadBaseAddress")!;
            _url = $"{_baseAddress}/books";

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<BookViewModel>> GetAllBooksAsync(string? sortParam)
        {
            List<BookViewModel> books = new List<BookViewModel>();

            try
            {
                HttpResponseMessage response = sortParam != null ? 
                    await _httpClient.GetAsync($"{_url}/?sortBy={sortParam}") : 
                    await _httpClient.GetAsync(_url);
                if(response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    List<BookDTO> booksDTO = JsonSerializer.Deserialize<List<BookDTO>>(json: content,
                                                                        options: _jsonSerializerOptions)!;
                    //books = await response.Content.ReadFromJsonAsync<List<BookModel>>();
                    books = booksDTO.ToView().ToList();
                }
                else
                {
                    Debug.WriteLine($"There was an error getting Http 2xx response: {response.ReasonPhrase}");
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
            }

            return books;
        }

        public async Task<List<BookViewModel>> GetBooksByGenreAsync(string? bookGenre)
        {
            List<BookViewModel> books = new List<BookViewModel>();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_url);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    List<BookDTO> booksDTO = JsonSerializer.Deserialize<List<BookDTO>>(json: content,
                                                                        options: _jsonSerializerOptions)!;
                    if(string.IsNullOrEmpty(bookGenre))
                    {
                        books = booksDTO.ToView().ToList();
                    }
                    else if(bookGenre == "Undefined")
                    {
                        books = booksDTO.ToView().Where(x => x.Genre == null).ToList();
                    }
                    else
                    {
                        books = booksDTO.ToView().Where(x => x.Genre == bookGenre).ToList();
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

            return books;
        }

        public async Task<BookDTO> GetBookByIdAsync(string Id)
        {
            BookDTO book = new BookDTO();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{_url}/{Id}");
                if(response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    book = JsonSerializer.Deserialize<BookDTO>(content, _jsonSerializerOptions)!;
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

            return book;
        }

        public async Task<double?> RateTheBook(RateBook rateBook)
        {
            RateBookDTO rateBookDTO = rateBook.ToDTO()!;
            if(rateBookDTO != null )
            {
                try
                {
                    string jsonRating = JsonSerializer.Serialize<RateBookDTO>(rateBookDTO, _jsonSerializerOptions);
                    StringContent stringContent = new StringContent(jsonRating, Encoding.UTF8, "application/json");
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", rateBook.AccessToken);
                    HttpResponseMessage response = await _httpClient.PostAsync($"{_url}/{rateBook.BookId}/rate", stringContent);

                    if(response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var rateBookResponse = JsonSerializer.Deserialize<RateBookResponse>(content, _jsonSerializerOptions)!;
                        return rateBookResponse.AverageRating;
                    }
                    else
                    {
                        Debug.WriteLine($"There was an error getting Http 2xx RateTheBook response: {response.ReasonPhrase}");
                    }
                }
                catch(Exception ex)
                {
                    Debug.WriteLine($"RateTheBook Exception: {ex.Message}");
                }
            }
            
            return null;
        }
    }
}
