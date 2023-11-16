using Microsoft.AspNetCore.Mvc;
using MovieDataFetcher.Entities;
using MovieDataFetcher.Services;
using System.ComponentModel;
using System.Text.Json;

namespace MovieDataFetcher.Controllers
{
    public class MovieInfoController : BaseApiController
    {
        private readonly HttpClient _httpClient;
        private readonly MovieService _movieService;
        private readonly IConfiguration _configuration;

        public MovieInfoController (HttpClient httpClient, MovieService movieService, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _movieService = movieService;
            _configuration = configuration;
        }

        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularMovies()
        {
            string apiKey = _configuration["AppSettings:ApiKey"];
            var response = await _httpClient.GetAsync($"https://api.themoviedb.org/3/movie/popular?api_key={apiKey}");
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(content);

            var uniqueMovies = apiResponse.Movies
                .GroupBy(m => m.Id)
                .Select(g => g.First())
                .ToList();

            //used the below loop to dynamically write the sql needed to insert the records into DB
            //foreach (var movie in uniqueMovies)
            //{
            //    var insertStatement = $"INSERT INTO Movies (Id, OriginalLanguage, OriginalTitle, Overview, PosterPath, ReleaseDate, VoteAverage) VALUES ({movie.Id}, '{movie.OriginalLanguage}', '{movie.OriginalTitle.Replace("'", "''")}', '{movie.Overview.Replace("'", "''")}', '{movie.PosterPath}', '{movie.ReleaseDate:yyyy-MM-dd}', {movie.VoteAverage});";
           //     Console.WriteLine(insertStatement);
            //    Console.WriteLine();
           // }

            foreach (var movie in apiResponse.Movies)
            {
                Console.WriteLine($"Title: {movie.OriginalTitle}");
                Console.WriteLine($"Overview: {movie.Overview}");
                Console.WriteLine();
            }

            return Ok(content);

        }

        [HttpGet("bytitle/{movieTitle}")]
        public async Task<ActionResult> GetMovieByTitle(string movieTitle)
        {
            string apiKey = _configuration["AppSettings:ApiKey"];
            var encodedTitle = Uri.EscapeDataString(movieTitle);
            var response = await _httpClient.GetAsync($"https://api.themoviedb.org/3/search/movie?api_key={apiKey}&query={movieTitle}");
            var content = await response.Content.ReadAsStringAsync();

            var jsonOptions = new JsonSerializerOptions
            {
                Converters = { new DateTimeConverter() },
                PropertyNameCaseInsensitive = true
            };

            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(content);

            var uniqueMovies = apiResponse.Movies
                .GroupBy(m => m.Id)
                .Select(g => g.First())
                .ToList();

            foreach (var movie in apiResponse.Movies)
            {
                Console.WriteLine($"Title: {movie.OriginalTitle}");
                Console.WriteLine($"Overview: {movie.Overview}");
                Console.WriteLine();
            }

            return Ok(content);
        }

        [HttpGet("range")]
        public async Task<IActionResult> GetMoviesReleasedBetween([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            string apiKey = _configuration["AppSettings:ApiKey"];
            var start = startDate.ToString("yyyy-MM-dd");
            var end = endDate.ToString("yyyy-MM-dd");
            var response = await _httpClient.GetAsync($"https://api.themoviedb.org/3/discover/movie?api_key={apiKey}&primary_release_date.gte={start}&primary_release_date.lte={end}");
            var content = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(content);

            var uniqueMovies = apiResponse.Movies
                .GroupBy(m => m.Id)
                .Select(g => g.First())
                .ToList();

            foreach (var movie in apiResponse.Movies)
            {
                Console.WriteLine($"Title: {movie.OriginalTitle}");
                Console.WriteLine($"Overview: {movie.Overview}");
                Console.WriteLine();
            }

            return Ok(content);
        }

        [HttpGet("rating")]
        public async Task<IActionResult> GetMoviesWithRatingAbove([FromQuery] double rating)
        {
            string apiKey = _configuration["AppSettings:ApiKey"];
            var response = await _httpClient.GetAsync($"https://api.themoviedb.org/3/discover/movie?api_key={apiKey}&vote_average.gte={rating}");
            var content = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(content);

            var uniqueMovies = apiResponse.Movies
                .GroupBy(m => m.Id)
                .Select(g => g.First())
                .ToList();

            foreach (var movie in apiResponse.Movies)
            {
                Console.WriteLine($"Title: {movie.OriginalTitle}");
                Console.WriteLine($"Overview: {movie.Overview}");
                Console.WriteLine();
            }

            return Ok(content);
        }

    }
}
