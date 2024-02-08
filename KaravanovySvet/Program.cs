using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using KaravanovySvet.Data;
using Azure.Storage.Blobs;
namespace KaravanovySvet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<KaravanovySvetContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("KaravanovySvetContext") ?? throw new InvalidOperationException("Connection string 'KaravanovySvetContext' not found.")));

            // Configure BlobServiceClient
            builder.Services.AddSingleton(s => new BlobServiceClient(builder.Configuration.GetConnectionString("BlobStorageConnection")));

            // Add services to the container.
            builder.Services.AddRazorPages();

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

            app.MapRazorPages();

            app.Run();
        }
    }
}
