using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using KaravanovySvet.Data;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Identity;
using Azure.Identity;
using JenikuvWeb.CloudStorage;
using Serilog;
namespace KaravanovySvet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<KaravanovySvetContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("KaravanovySvetContext") ?? throw new InvalidOperationException("Connection string 'KaravanovySvetContext' not found.")));

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<KaravanovySvetContext>();

            builder.Services.AddSingleton<ICloudStorage, GoogleCloudStorage>();

            builder.Services.AddMemoryCache();

            // Add services to the container.
            builder.Services.AddMvc();
            builder.Services.AddRazorPages();

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog(); // Use Serilog for logging

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "Admin",
                pattern: "{area:exists}/{controller=Home}/{action=Index}");

            app.MapRazorPages();

            app.Run();
        }
    }
}
