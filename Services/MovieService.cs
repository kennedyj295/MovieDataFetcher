using Microsoft.EntityFrameworkCore;
using MovieDataFetcher.Data;
using MovieDataFetcher.Entities;
using MovieDataFetcher.Interfaces;

namespace MovieDataFetcher.Services
{
    public class MovieService : IMovieService
    {
        //this class is for writing movies to the database, I didn't end up needing this since I manually inserted the records into the DB through static DML
        private readonly DataContext _dbContext;

        public MovieService(DataContext context)
        {
            _dbContext = context;
        }
        public async Task AddMoviesAsync(List<Movie> movies)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var movie in movies)
                    {
                        var existingMovie = await _dbContext.Movies
                            .FirstOrDefaultAsync(m => m.Id == movie.Id);

                        if (existingMovie == null)
                        {
                            movie.ReleaseDate = DateTime.SpecifyKind(movie.ReleaseDate, DateTimeKind.Utc);
                            _dbContext.Movies.Add(movie);
                        }
                    }

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
}
