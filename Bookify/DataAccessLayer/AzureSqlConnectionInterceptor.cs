using System.Data.Common;
using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Bookify.DataAccessLayer
{
    /// <summary>
    /// Interceptor that handles SQL connection authentication.
    /// Automatically detects connection type and applies appropriate authentication.
    /// </summary>
    public class AzureSqlConnectionInterceptor : DbConnectionInterceptor
    {
        private static readonly TokenCredential _credential = new DefaultAzureCredential();

        public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
            DbConnection connection,
            ConnectionEventData eventData,
            InterceptionResult result,
            CancellationToken cancellationToken = default)
        {
            if (connection is SqlConnection sqlConnection && sqlConnection.AccessToken == null)
            {
                // Only apply special authentication for cloud SQL connections
                // Skip for local databases (localdb, localhost, etc.)
                var connectionString = sqlConnection.ConnectionString ?? "";
                if (connectionString.Contains("database.windows.net", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var token = await _credential.GetTokenAsync(
                            new TokenRequestContext(new[] { "https://database.windows.net/.default" }),
                            cancellationToken);
                        
                        sqlConnection.AccessToken = token.Token;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Token acquisition failed: {ex.Message}");
                        throw;
                    }
                }
            }

            return await base.ConnectionOpeningAsync(connection, eventData, result, cancellationToken);
        }

        public override InterceptionResult ConnectionOpening(
            DbConnection connection,
            ConnectionEventData eventData,
            InterceptionResult result)
        {
            if (connection is SqlConnection sqlConnection && sqlConnection.AccessToken == null)
            {
                // Only apply special authentication for cloud SQL connections
                // Skip for local databases (localdb, localhost, etc.)
                var connectionString = sqlConnection.ConnectionString ?? "";
                if (connectionString.Contains("database.windows.net", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var token = _credential.GetToken(
                            new TokenRequestContext(new[] { "https://database.windows.net/.default" }),
                            CancellationToken.None);
                        
                        sqlConnection.AccessToken = token.Token;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Token acquisition failed: {ex.Message}");
                        throw;
                    }
                }
            }

            return base.ConnectionOpening(connection, eventData, result);
        }
    }
}

