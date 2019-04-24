using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace PollyTestApp
{
    public class Startup
    {
        private IWebHostEnvironment HostingEnvironment { get; }
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IConfiguration config = new ConfigurationBuilder()
                     .SetBasePath(HostingEnvironment.ContentRootPath)
           //.SetBasePath(Directory.GetCurrentDirectory())
           //.AddJsonFile("appsettings.json", true, true)
           .AddJsonFile($"appsettings.{HostingEnvironment.EnvironmentName}.json", true, true)
           .AddEnvironmentVariables()
           .Build();

            services.AddSingleton(config);
            services.AddSingleton(HostingEnvironment);
            services.AddOptions();
            services.AddMemoryCache();
            services.Configure<ClientRateLimitOptions>(Configuration.GetSection("ClientRateLimiting"));

            services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // configure the resolvers
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
            });


            services.AddControllersWithViews()
                .AddNewtonsoftJson();
            services.AddRazorPages();
            services.AddSwaggerGen(options =>
            {
                string defaultDocName = HostingEnvironment.ApplicationName;

                options.SwaggerDoc(defaultDocName, new OpenApiInfo
                {
                    Title = Configuration["Application:Name"] ?? $"{typeof(Startup).Namespace}.Docs",
                    Version = Configuration["Application:Version"] ?? "1.0",
                    Description = Configuration["Application:Description"] ?? $"Api of {typeof(Startup).Namespace}"
                });
                options.SwaggerDoc("1_0", new OpenApiInfo
                {
                    Title = Configuration["Application:Name"] ?? $"{typeof(Startup).Namespace}.Docs",
                    Version = "1.0",
                    Description = Configuration["Application:Description"] ?? $"Api of {typeof(Startup).Namespace}"
                });
                options.SwaggerDoc("2_0", new OpenApiInfo
                {
                    Title = Configuration["Application:Name"] ?? $"{typeof(Startup).Namespace}.Docs",
                    Version = "2.0",
                    Description = Configuration["Application:Description"] ?? $"Api of {typeof(Startup).Namespace}"
                });
                options.CustomSchemaIds(t => t.FullName);
                options.DescribeAllEnumsAsStrings();
                options.DescribeStringEnumsInCamelCase();
                options.DescribeAllParametersInCamelCase();
              
                options.IgnoreObsoleteActions();
                options.IgnoreObsoleteProperties();
                options.IncludeXmlComments(Path.Combine(HostingEnvironment.WebRootPath, HostingEnvironment.ApplicationName + ".xml"));
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseClientRateLimiting();
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthorization();
            app.Use(async (context, next) =>
            {
                if (context.Request.Query.TryGetValue("echo", out StringValues value))
                {
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "application/json; charset=utf-8";
                    await context.Response.WriteAsync(value.ToString());
                    return;
                }

                await next.Invoke();
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.DocumentTitle = "Api Documents of " + GetType().Namespace;
                options.RoutePrefix = "docs";
                options.DefaultModelRendering(ModelRendering.Example);
                options.DisplayRequestDuration();
                options.DocExpansion(DocExpansion.None);
                options.EnableDeepLinking();
                options.EnableFilter();
                options.EnableValidator();
                options.SwaggerEndpoint($"/swagger/{HostingEnvironment.ApplicationName}/swagger.json", HostingEnvironment.ApplicationName);
                options.SwaggerEndpoint("/swagger/1_0/swagger.json", HostingEnvironment.ApplicationName + " - 1.0");
                options.SwaggerEndpoint("/swagger/2_0/swagger.json", HostingEnvironment.ApplicationName + " - 2.0");
            });
        }
    }
}
