using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieDataFetcher.Entities;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MovieDataFetcher.Pages
{
    public class EditDatabaseDataModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditDatabaseDataModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public Movie movie { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var apiUrl = $"https://localhost:7236/api/db/singlemovie/{id}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                movie = JsonSerializer.Deserialize<Movie>(jsonContent);
            }
            else
            {
                return NotFound();
            }

            return Page();
        }

    }
}
