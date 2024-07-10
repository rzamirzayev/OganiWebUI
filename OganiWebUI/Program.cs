using Microsoft.EntityFrameworkCore;
using OganiWebUI.AppCode.Services;
using OganiWebUI.AppCode.Services.Implementation;
using OganiWebUI.Models.Configurations;
using OganiWebUI.Models.Contexts;

namespace OganiWebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddRouting(cfg => cfg.LowercaseUrls = true);
            builder.Services.AddDbContext<DataContext>(cfg =>
            {
                cfg.UseSqlServer(builder.Configuration.GetConnectionString("cString"), opt =>
                {
                    opt.MigrationsHistoryTable("MigrationHistory");
                });
            });


            builder.Services.Configure<EmailConfiguration>(cfg => builder.Configuration.GetSection(cfg.GetType().Name).Bind(cfg));

            builder.Services.Configure<CryptoServiceConfiguration>(cfg =>builder.Configuration.GetSection(cfg.GetType().Name).Bind(cfg));

            builder.Services.AddSingleton<IEmailService, EmailService>();
            builder.Services.AddSingleton<ICryptoService, CryptoService>();

            var app = builder.Build();
            app.UseStaticFiles();
            app.MapControllerRoute(name:"default",pattern:"{controller=home}/{action=index}/{id?}");

            //app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}