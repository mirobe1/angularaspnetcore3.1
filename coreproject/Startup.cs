using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coreproject.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;
using coreproject.Model;
using coreproject.Middleware;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Text.Json.Serialization;

namespace coreproject
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

            services.AddDbContext<ApplicationContext>();
            services.AddCors();

            services.AddScoped<IAccountService,AccountService>();
            services.AddSingleton<IEncryptonService,EncryptonService>();
            services.AddScoped<IRecipeService,RecipeService>();
            
            services.Configure<FormOptions>(o => {  // for files
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });

            services.AddControllers().AddJsonOptions(jsonOptions =>
                {
                    jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            //app.UseCookieAuthentication();    
            app.UseHttpsRedirection();


            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
                RequestPath = new PathString("/Resources")
            });

            app.UseRouting();
            

            var cookiePolicyOptions = new CookiePolicyOptions
        {
            Secure = CookieSecurePolicy.SameAsRequest,
            MinimumSameSitePolicy = SameSiteMode.None
        };

         app.UseCookiePolicy(cookiePolicyOptions);


            // global cors policy
            app.UseCors(x => x
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()); // allow credentials

            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseMiddleware<AuthorizationMiddleware>();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
