using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;

namespace PdfAValidatorWebApi
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                  c.OperationFilter<FormFileSwaggerFilter> ();
                c.SwaggerDoc("v1", new Info
                {
                    Title = "PdfAValidator",
                    Version = "v1",
                    Description = "A simple ASP.NET Core Web API wrapping access to VeraPdf",
                    TermsOfService = "This is just a showcase, this service comes without any warranty.",
                    Contact = new Contact
                    {
                        Name = "Codeuctivity",
                        Email = string.Empty,
                        Url = "https://github.com/Codeuctivity/PdfAValidatorApi"
                    },
                    License = new License
                    {
                        Name = "Use under AGPL",
                        Url = "https://github.com/Codeuctivity/PdfAValidatorApi/blob/master/LICENSE"
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PdfAValidator V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseMvc();
        }
    }
}