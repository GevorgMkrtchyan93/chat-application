using Abp.AspNetCore.SignalR.Hubs;
using Abp.Runtime.Caching;
using ChatApplication.Data;
using ChatApplication.Hubs;
using ChatApplication.Models;
using ChatApplication.Services;
using ChatApplication.Services.Interfaces;
using ChatApplication.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();

            services.AddSignalR();

            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = Configuration["Redis:ConnectionString"];
            });

            services.AddSingleton<ICacheExtensionsService, CacheExtensionsService>();
            services.AddTransient<IMessageService, MessageService>();

            Task.Run(async () => await AddDbDateCache(services));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors(builder =>
            {
                builder.WithOrigins("https://localhost:4000")
                    .AllowAnyHeader()
                    .WithMethods("GET", "POST")
                    .AllowCredentials();
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<AbpCommonHub>("/signalr");
                endpoints.MapHub<ChatHub>("/chathub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }

        public async Task AddDbDateCache(IServiceCollection services)
        {
            var _context = services.BuildServiceProvider()
                      .GetService<ApplicationDbContext>();

            var _cacheExtensionsService = services.BuildServiceProvider()
                     .GetService<ICacheExtensionsService>();

            var redisMessages = await _context.Messages.Select(m =>
            new RedisCacheDataModel
            {
                Text = m.Text,
                UserName = m.Sender.UserName,
                When = m.When
            }).ToListAsync();

            await _cacheExtensionsService.SetInitialCacheValueAsync(redisMessages);
        }
    }
}
