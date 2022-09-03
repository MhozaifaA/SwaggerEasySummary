using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swagger.Configure;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SWSwaggerGenServiceCollectionExtensions
    {
        public static IServiceCollection AddSWSwagger(
            this IServiceCollection services)
        {

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = Assembly.GetEntryAssembly().GetName().Name,
                    Description = "API for "+ Assembly.GetEntryAssembly().GetName().Name,
                    TermsOfService = new Uri("http://sworksgroup.com/"),
                    Contact = new OpenApiContact
                    {
                        Name = "Contact",
                        Url = new Uri("http://sworksgroup.com/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "License",
                        Url = new Uri("http://sworksgroup.com/")
                    }
                });


                options.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    In = ParameterLocation.Header,
                    Description = "Basic Authorization header using the Bearer scheme."
                });


                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "basic"
                            }
                        },
                        new string[] {}
                    }
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {{
                       new OpenApiSecurityScheme
                         {
                             Reference = new OpenApiReference
                             {
                                 Type = ReferenceType.SecurityScheme,
                                 Id = "Bearer"
                             }
                         },
                         new string[] {}
                    }});




                var XmlDocPaths = System.IO.Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)), "*.xml");
                try
                {
                    var XmlDocs = (
                     from DocPath in XmlDocPaths select XDocument.Load(DocPath)
                   ).ToList();

                    foreach (var doc in XmlDocs)
                    {
                        try
                        {
                            options.IncludeXmlComments(() => new XPathDocument(doc.CreateReader()), true);
                            options.SchemaFilter<DescribeEnumMembers>(doc);
                        }
                        catch
                        {

                        }
                    }

                    options.DocumentFilter<EnumTypesDocumentFilter>();

                }
                catch
                {

                }
               
               

            });

            return services;
        }
    }
}


namespace Microsoft.AspNetCore.Builder
{
    public static class SWSwaggerUIBuilderExtensions
    {
        /// <summary>
        /// <para></para>
        /// c.DocExpansion(DocExpansion.None);
        /// <para></para>
        /// 
        /// c.InjectStylesheet(configuration["SubBasePath"] + "/app-swagger-ui.css");
        /// <para></para>
        /// 
        /// c.InjectJavascript(configuration["SubBasePath"] + "/jquery-3.6.0.min.js");
        /// <para></para>
        ///
        /// c.InjectJavascript(configuration["SubBasePath"] + "/app-swagger-ui.js");
        /// </summary>
        /// <param name="app"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSWSwagger(this IApplicationBuilder app, Action<SwaggerUIOptions> setupAction = null)
        {

            app.UseSwagger();

            IConfiguration configuration = app.ApplicationServices.GetService<IConfiguration>();
            Action<SwaggerUIOptions> action = c =>
            {
                c.DocExpansion(DocExpansion.None);
                c.InjectStylesheet(configuration["SubBasePath"] + "/app-swagger-ui.css");
                c.InjectJavascript(configuration["SubBasePath"] + "/jquery-3.6.0.min.js");
                c.InjectJavascript(configuration["SubBasePath"] + "/app-swagger-ui.js");
            };
            setupAction ??= action;



            return app.UseSwaggerUI(setupAction);
        }
    }
}