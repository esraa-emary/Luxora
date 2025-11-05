using System.Data.Common;
using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Bookify.DataAccessLayer
{
    /// <summary>
    /// Interceptor that adds Azure AD token to SQL connections.
    /// Uses Managed Identity in Azure, DefaultAzureCredential locally.
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
                try
                {
                    var token = await _credential.GetTokenAsync(
                        new TokenRequestContext(new[] { "https://database.windows.net/.default" }),
                        cancellationToken);
                    
                    sqlConnection.AccessToken = token.Token;
                }
                catch (Exception ex)
                {
                    // Log the error - token acquisition failed
                    // This will help diagnose if Managed Identity is not working
                    System.Diagnostics.Debug.WriteLine($"Azure AD token acquisition failed: {ex.Message}");
                    throw; // Re-throw to fail fast and show the real error
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
                try
                {
                    var token = _credential.GetToken(
                        new TokenRequestContext(new[] { "https://database.windows.net/.default" }),
                        CancellationToken.None);
                    
                    sqlConnection.AccessToken = token.Token;
                }
                catch (Exception ex)
                {
                    // Log the error - token acquisition failed
                    System.Diagnostics.Debug.WriteLine($"Azure AD token acquisition failed: {ex.Message}");
                    throw; // Re-throw to fail fast and show the real error
                }
            }

            return base.ConnectionOpening(connection, eventData, result);
        }
    }
}

