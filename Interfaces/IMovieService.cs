using MovieDataFetcher.Entities;

namespace MovieDataFetcher.Interfaces
{
    public interface IMovieService
    {
        //not used ultimately, went with static SQL queries rather than entity framework due to a Postgresql compatibility issue
        public Task AddMoviesAsync(List<Movie> movies);
    }
}
