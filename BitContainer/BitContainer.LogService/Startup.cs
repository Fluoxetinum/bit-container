using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitContainer.DataAccess;
using BitContainer.DataAccess.DataProviders;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.DataAccess.Scripts;
using BitContainer.Shared.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BitContainer.LogService
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
            services.AddControllers().AddNewtonsoftJson();

            String sqlServerConnectionString = 
                Configuration.GetConnectionString("SqlServerDbConnectionString");

            T4LogsDbInitScript script = new T4LogsDbInitScript(DbNames.LogsDbName);
            services.AddSingleton<ISqlDbHelper, CSqlDbHelper>((serviceProvider) =>
                new CSqlDbHelper(sqlServerConnectionString, script));

            services.AddSingleton<ILogsProvider, CLogsProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
