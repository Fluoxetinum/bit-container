using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitContainer.DataAccess;
using BitContainer.DataAccess.DataProviders;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.Presentation.Controllers.Proxies;
using BitContainer.Shared.Auth;
using BitContainer.Shared.Http;
using BitContainer.Shared.Middleware;
using BitContainer.StorageService.Managers;
using BitContainer.StorageService.Managers.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace BitContainer.StorageService
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
            services.AddControllers().AddNewtonsoftJson( options =>
            {
                options.SerializerSettings.TypeNameHandling = TypeNameHandling.All;
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = AuthOptions.GetTokenValidationParameters();
                });
            
            services.AddSingleton<IStorageProvider, CStorageProvider>();
            services.AddSingleton<IParallelAccessManager, CReadWriteFileLock>();

            services.AddSignalR();
            
            String sqlServerConnectionString = 
                Configuration.GetConnectionString("SqlServerDbConnectionString");
            CDbHelper.Init(sqlServerConnectionString);

            String authServiceUrl = 
                Configuration.GetConnectionString("AuthServiceUrl");
            AuthServiceProxy.SetServiceUrl(authServiceUrl);

            String logServiceUrl =
                Configuration.GetConnectionString("LogServiceUrl");
            CLogServiceProxy.SetServiceUrl(logServiceUrl);
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
            });
        }
    }
}
