using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Bookify.DataAccessLayer
{
    /// <summary>
    /// Design-time factory for Entity Framework migrations.
    /// This allows EF Core tools to create the DbContext during migrations.
    /// </summary>
    public class BookifyDbContextFactory : IDesignTimeDbContextFactory<BookifyDbContext>
    {
        public BookifyDbContext CreateDbContext(string[] args)
        {
            // Build configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<BookifyDbContext>();
            optionsBuilder.UseSqlServer(
                connectionString,
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null));

            return new BookifyDbContext(optionsBuilder.Options);
        }
    }
}

