using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Movies.APP.Domain
{
    public class MoviesDbFactory : IDesignTimeDbContextFactory<MoviesDb>
    {

        const string CONNECTIONSTRING = "data source=MoviesDB";

        public MoviesDb CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MoviesDb>();
            optionsBuilder.UseSqlite(CONNECTIONSTRING);
            return new MoviesDb(optionsBuilder.Options);
        }
    }
}