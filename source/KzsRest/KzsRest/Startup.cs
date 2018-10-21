using KzsRest.Engine.MetricsExtensions;
using KzsRest.Engine.Services.Abstract;
using KzsRest.Engine.Services.Implementation;
using KzsRest.Filters;
using KzsRest.Services.Abstract;
using KzsRest.Services.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using System;

namespace KzsRest
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
            services.AddMvc(setup =>
            {
                setup.Filters.Add(new ExceptionFilter());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMemoryCache();
            services.AddSingleton<IKzsParser, KzsParser>();
            services.AddSingleton<IDomRetriever, DomRetriever>();
            services.AddSingleton<IConvert, KzsConvert>();
            services.AddSingleton<ISystem, KzsSystem>();
            services.AddSingleton<ICacheService, CacheService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment() || true)
            {
                app.UseDeveloperExceptionPage();
            }
            Console.WriteLine($"Environment is {env.EnvironmentName}");
            app.UseMetricServer();
            app.UseMiddleware<RequestMetricsMiddleware>();

            app.UseMvc();
        }
    }
}
