using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Repository.Models;
using BusinessLogic;
using ForumApi.AuthenticationHelper;


namespace ForumApi
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
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                                builder =>
                                {
                                     builder.WithOrigins(
                                         "http://localhost:4200",
                                         "http://20.94.137.143/", //Frontend
                                         "http://20.189.29.112/", //Admintools
                                         "http://20.45.2.119/" //User
                                         )
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                                });
            });

            var myConnectionString = Configuration.GetConnectionString("Cinephiliacs_Forum");
            services.AddDbContext<Cinephiliacs_ForumContext>(
                options => options.UseSqlServer(myConnectionString)
            );

            services.AddScoped<BusinessLogic.Interfaces.IForumLogic, ForumLogic>();
            services.AddScoped<Repository.IRepoLogic, Repository.RepoLogic>();
            //services.AddScoped<BusinessLogic.ForumLogic>();

            // for authentication
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = "scheme";
            })
            .AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>(
                "scheme", o => { });

            var permissions = new[] {
                // "loggedin", // for signed in
                "manage:forums", // for moderator (is signed in)
                "manage:awebsite", // for admin (is moderator and signed in)
            };
            services.AddAuthorization(options =>
            {
                for (int i = 0; i < permissions.Length; i++)
                {
                    options.AddPolicy(permissions[i], policy => policy.RequireClaim(permissions[i], "true"));
                }
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ForumApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ForumApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Enables the CORS policty for all controller endpoints. Must come between UseRouting() and UseEndpoints()
            app.UseCors();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
