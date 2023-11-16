using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MovieDataFetcher.Data;
using MovieDataFetcher.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System;

namespace MovieDataFetcher.Controllers
{
    public class DBController : BaseApiController
    {
        private readonly ILogger<DBController> _logger;
        private readonly IConfiguration _configuration;

        public DBController(ILogger<DBController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            var movies = new List<Movie>();

            using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("MyDbConnection")))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("SELECT * FROM Movies", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var movie = new Movie
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Title = reader.GetString(reader.GetOrdinal("OriginalTitle")),
                        };

                        movies.Add(movie);
                    }
                }
            }

            return Ok(movies);
        }

        [HttpGet("singlemovie/{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            var movie = new Movie();

            using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("MyDbConnection")))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("SELECT * FROM Movies WHERE Id = @Id", connection))
                {
                    command.Parameters.Add("@Id", NpgsqlTypes.NpgsqlDbType.Integer).Value = id;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            movie.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            movie.Title = reader.GetString(reader.GetOrdinal("OriginalTitle"));
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }

            return Ok(movie);
        }

        [HttpPost("updatemovie/{id}")]
        public async Task<IActionResult> UpdateMovie(int id, [FromForm] Movie movie)
        {

            using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("MyDbConnection")))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("UPDATE Movies SET OriginalTitle = @OriginalTitle WHERE Id = @Id", connection))
                {
                    command.Parameters.Add("@Id", NpgsqlTypes.NpgsqlDbType.Integer).Value = id;
                    command.Parameters.Add("@OriginalTitle", NpgsqlTypes.NpgsqlDbType.Text).Value = movie.Title;

                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                    {
                        return NotFound();
                    }
                }
            }

            return Ok();
        }

        [HttpPost("createmovie")]
        public async Task<ActionResult<Movie>> CreateMovie([FromBody] Movie movie)
        {
            using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("MyDbConnection")))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("INSERT INTO Movies (OriginalTitle) VALUES (@Title) RETURNING Id", connection))
                {
                    command.Parameters.Add("@OriginalTitle", NpgsqlTypes.NpgsqlDbType.Text).Value = movie.Title;

                    int newId = Convert.ToInt32(await command.ExecuteScalarAsync());

                    movie.Id = newId;
                }
            }

            return CreatedAtAction("GetMovie", new { id = movie.Id }, movie);
        }
    }
}
