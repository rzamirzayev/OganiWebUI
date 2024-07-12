using Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Services;
using Services.Common;
using Services.Implementation;
using Services.Implementation.Common;

namespace WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddRouting(cfg => cfg.LowercaseUrls = true);
            builder.Services.AddDbContext<DbContext,DataContext>(cfg =>
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
            builder.Services.AddHttpContextAccessor();


            builder.Services.AddScoped<IContactPostService, ContactPostService>();
            builder.Services.AddScoped<ISubscribeService, SubscribeService>();


            var app = builder.Build();
            app.UseStaticFiles();
            app.MapControllerRoute(name:"default",pattern:"{controller=home}/{action=index}/{id?}");

            //app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}