using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Repository.Models;
using BusinessLogic;

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
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                                builder =>
                                {
                                     builder.WithOrigins("http://localhost:4200", "*")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                                });
            });

            services.AddControllers();

            var myConnectionString = Configuration.GetConnectionString("Cinephiliacs_Forum");
            services.AddDbContext<Cinephiliacs_ForumContext>(
                options => options.UseSqlServer(myConnectionString)
            );

            services.AddScoped<BusinessLogic.Interfaces.IForumLogic, ForumLogic>();
            services.AddScoped<Repository.RepoLogic>();
            //services.AddScoped<BusinessLogic.ForumLogic>();

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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
