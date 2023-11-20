using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MovieDataFetcher.Entities
{
    public class Movie
    {
        // entity framework tags not ultimately used due to Postgresql compatibility issues
        [Key]
        [Column("Id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Column("OriginalLanguage")]
        [JsonPropertyName("original_language")]
        public string? OriginalLanguage { get; set; }

        [Column("OriginalTitle")]
        [JsonPropertyName("original_title")]
        public string? OriginalTitle { get; set; }

        [Column("Overview")]
        [JsonPropertyName("overview")]
        public string? Overview { get; set; }

        [Column("PosterPath")]
        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [Column("ReleaseDate")]
        [JsonPropertyName("release_date")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime ReleaseDate { get; set; }

        [Column("VoteAverage")]
        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }
    }
}
