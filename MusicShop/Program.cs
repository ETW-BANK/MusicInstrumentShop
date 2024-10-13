using Microsoft.EntityFrameworkCore;
using MusicShop.DataAccess.Data;
using MusicShop.DataAccess.Repository.IRepository;
using MusicShop.DataAccess.Repository;
using Microsoft.AspNetCore.Identity;
using Stripe;

namespace MusicShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            var connectionstring = builder.Configuration.GetConnectionString("MusicShopDb");
            builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(connectionstring));

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();


            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set timeout as needed
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
              pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
