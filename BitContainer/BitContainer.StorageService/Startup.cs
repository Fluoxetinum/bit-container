using System;
using BitContainer.DataAccess;
using BitContainer.DataAccess.DataProviders;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Scripts;
using BitContainer.Http;
using BitContainer.Http.Proxies;
using BitContainer.Service.Storage.Managers;
using BitContainer.Service.Storage.Managers.Interfaces;
using BitContainer.Services.Shared;
using BitContainer.Services.Shared.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;

namespace BitContainer.Service.Storage
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            IdentityModelEventSource.ShowPII = true;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = AuthOptions.GetTokenValidationParameters();
                });

            String sqlServerConnectionString = 
                Configuration.GetConnectionString("SqlServerDbConnectionString");

            T4StorageDbInitScript script = new T4StorageDbInitScript(DbNames.StorageDbName);
            services.AddSingleton<ISqlDbHelper, CSqlDbHelper>((serviceProvider) =>
                new CSqlDbHelper(sqlServerConnectionString, script, serviceProvider.GetService<ILogger<CSqlDbHelper>>()));
            services.AddSingleton<IStorageProvider, CStorageProvider>();
            services.AddSingleton<IStatsProvider, CStatsProvider>();
            services.AddSingleton<ILoadsManager, CLoadsManager>();
            services.AddSingleton<ISignalsManager, CSignalsManager>();
            services.AddSignalR();
            
            String authServiceUrl = 
                Configuration.GetConnectionString("AuthServiceUrl");

            services.AddSingleton<IAuthServiceProxy, ÑAuthServiceProxy>((serviceProvider) =>
                new ÑAuthServiceProxy(new CHttpHelper(), authServiceUrl));
            
            String logServiceUrl =
                Configuration.GetConnectionString("LogServiceUrl");
           
            services.AddSingleton<ILogServiceProxy, CLogServiceProxy>((serviceProvider) 
                => new CLogServiceProxy(new CHttpHelper(), logServiceUrl));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<CEventsHub>("/eventshub");
            });
        }
    }
}
