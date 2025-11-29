using Azure.Identity;
using Microsoft.Data.SqlClient;
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
            // Build configuration from appsettings.json and User Secrets
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<BookifyDbContextFactory>() // Add User Secrets support
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            var optionsBuilder = new DbContextOptionsBuilder<BookifyDbContext>();
            
            // Check if this is a cloud SQL connection
            if (!string.IsNullOrEmpty(connectionString) && connectionString.Contains("database.windows.net", StringComparison.OrdinalIgnoreCase))
            {
                // For cloud connections, apply token-based authentication
                try
                {
                    var credential = new DefaultAzureCredential();
                    var token = credential.GetToken(
                        new Azure.Core.TokenRequestContext(new[] { "https://database.windows.net/.default" }),
                        CancellationToken.None);
                    
                    var sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.AccessToken = token.Token;
                    
                    optionsBuilder.UseSqlServer(
                        sqlConnection,
                        sqlOptions => sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null));
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Failed to authenticate database connection. Error: {ex.Message}", ex);
                }
            }
            else
            {
                // For local databases, use standard connection
                optionsBuilder.UseSqlServer(
                    connectionString,
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null));
            }

            return new BookifyDbContext(optionsBuilder.Options);
        }
    }
}

