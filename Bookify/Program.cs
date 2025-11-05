using Bookify.DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace Bookify
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Configure Entity Framework with Azure AD authentication
            // Uses Managed Identity when deployed, DefaultAzureCredential locally
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            
            // Remove Authentication from connection string - we'll set token manually
            var sqlBuilder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
            if (sqlBuilder.ContainsKey("Authentication"))
            {
                sqlBuilder.Remove("Authentication");
            }
            
            builder.Services.AddSingleton<AzureSqlConnectionInterceptor>();
            
            builder.Services.AddDbContext<BookifyDbContext>((serviceProvider, options) =>
            {
                var interceptor = serviceProvider.GetRequiredService<AzureSqlConnectionInterceptor>();
                
                options.UseSqlServer(
                    sqlBuilder.ConnectionString,
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null));
                
                options.AddInterceptors(interceptor);
            });

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
