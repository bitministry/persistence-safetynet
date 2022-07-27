using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NHibernate;
using System.IO;
using Turnit.GenericStore.Domain.Interfaces;
using Turnit.GenericStore.NHiber.Services;
using Turnit.GenericStore.NHibernateMaps;

namespace Turnit.GenericStore.WebApi.Common
{
    public class StartupBase
    {
        public StartupBase(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            UnitOfWork.ConnectionString = Configuration.GetConnectionString("Default");
            UnprocessedRequestsQueue.BaseLocation = Directory.GetCurrentDirectory() +"/UnprocessedRequests";

            services.AddControllers();

            services.AddScoped(UnitOfWork.CreateSessionFactory);
            services.AddScoped(sp => sp.GetRequiredService<ISessionFactory>().OpenSession());

            services.AddScoped<ICatalogService>(sp => new CatalogService(sp.GetRequiredService<ISession>()));
            services.AddScoped<IInventoryService>(sp => new InventoryService(sp.GetRequiredService<ISession>()));

            services.AddSwaggerGen(x => x.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Turnit Store"
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();
            app.UseSwagger()
                .UseSwaggerUI(x => x.SwaggerEndpoint("v1/swagger.json", "Turnit Store V1"));

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapSwagger();
            });
        }
    }
}