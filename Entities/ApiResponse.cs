using System.Text.Json.Serialization;

namespace MovieDataFetcher.Entities
{
    public class ApiResponse
    {
        [JsonPropertyName("results")]
        public List<Movie> Movies { get; set; }
    }
}
