using Codeuctivity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace PdfAValidatorWebApi
{
    /// <summary>
    /// Startup Things comes in here
    /// </summary>
    public class Startup
    {
        private const string GithubProjectAdress = "https://github.com/Codeuctivity/PdfAValidatorApi";

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        ///For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IPdfAValidator, PdfAValidator>(_ =>
            {
                var pdfaValidator = new PdfAValidator(new ApplicationInsightsJavaAgentOutputFilter());
                return pdfaValidator;
            });

            services.AddMvc();
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v2",
                    Title = $"PdfAValidator {typeof(Startup).Assembly.GetName().Version}",
                    Description = "A simple ASP.NET Core Web API wrapping access to VeraPdf",
                    TermsOfService = new Uri(GithubProjectAdress),
                    Contact = new OpenApiContact
                    {
                        Name = "Codeuctivity",
                        Email = "codeuctivity@gmail.com",
                        Url = new Uri(GithubProjectAdress),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under AGPL",
                        Url = new Uri($"{GithubProjectAdress}/blob/main/LICENSE"),
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            services.AddCors();

            var requestLimit = 500 * 1024 * 1024;

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = requestLimit;
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = requestLimit;
            });

            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = requestLimit;
                options.MultipartBodyLengthLimit = requestLimit;
                options.MultipartHeadersLengthLimit = requestLimit;
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            if (env.IsDevelopment())

            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PdfAValidator V1");
                c.RoutePrefix = string.Empty;
                c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
            });

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}