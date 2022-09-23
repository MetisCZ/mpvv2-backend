using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using mpvv2.DbModels;

namespace mpvv2
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
            services.AddCors(options =>
            {
                /*options.AddPolicy("AllowMyOrigin",
                    builder => builder.AllowAnyHeader().WithOrigins(
                            "http://localhost:3000").AllowAnyMethod()
                );*/
                options.AddPolicy("AllowMyOrigin",
                    builder => builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod()
                );
            });
            
            //var connectionString = "Server=localhost;User=mpv-admin;Password=Jakub321654;Database=mpv;Connection Timeout=300";
            var connectionString = "Server=localhost;User=root;Password=;Database=mpv";
            var serverVersion = new MariaDbServerVersion(new Version(10,4,22));

            services.AddDbContext<mpvContext>(
                dbContextOptions => dbContextOptions.UseMySql(connectionString, serverVersion));
            
            /*services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("10.0.0.100"));
            });*/

            services.AddHostedService<TimedHostedService>();
            
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthorization();
            
            app.UseCors("AllowMyOrigin");
            //app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
