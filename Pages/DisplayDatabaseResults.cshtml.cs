using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using MovieDataFetcher.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MovieDataFetcher.Pages
{
    public class DisplayDatabaseResultsModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<Movie> movies { get; private set; }


        public DisplayDatabaseResultsModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task OnGetAsync()
        {
            var apiUrl = "https://localhost:7236/api/db/all";
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                movies = JsonSerializer.Deserialize<List<Movie>>(jsonContent);
            }
            else
            {
                movies = new List<Movie>();
            }
        }
    }
}
