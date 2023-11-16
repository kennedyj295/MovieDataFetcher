using Microsoft.EntityFrameworkCore;
using MovieDataFetcher.Entities;

namespace MovieDataFetcher.Data
{
    public class DataContext : DbContext
    {
        //not used ultimately, went with static SQL queries rather than entity framework due to a Postgresql compatibility issue
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
    }
}
