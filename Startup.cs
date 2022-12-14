using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApi.BLL;
using TestApi.Interfaces;
using TestApi.Models;
using TestApi.Repository;

namespace TestApi
{
    public class Startup
    {
        public static Dictionary<int, int> FilesProgress { get; set; }
        public Startup(IConfiguration configuration)
        {
            FilesProgress = new Dictionary<int, int>();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string con = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<Context>(options => options.UseNpgsql(con));

            services.AddTransient<IFilesRepository, FilesRepository>();
            services.AddTransient<IFilesService, FilesService>();

            services.AddTransient<ILinksRepository, LinksRepository>();
            services.AddTransient<ILinksService, LinksService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TestApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TestApi v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
