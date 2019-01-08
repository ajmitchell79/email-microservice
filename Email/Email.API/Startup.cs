using System;
using System.IO;
using System.Text;
using Email.API.BRL.Common;
using Email.API.BRL.Interfaces;
using Email.API.BRL.Validation;
using Email.API.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace Email.API
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
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            services.Configure<SmtpSettings>(Configuration.GetSection("SmtpSettings"));

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser() //this will lock down all endpoints unless otherwise stated on each  eg allowanonymous
                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));

                config.Filters.Add(typeof(ValidatorActionFilter));
            })
            .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddCors(o => o.AddPolicy("CorsPolicy", builder => {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .AllowAnyOrigin();
            }));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                    {
                        Version = "v1.0",
                        Title = "Email MicroService",
                        Description = "Email MicroService",
                        TermsOfService = "",
                        Contact = new Swashbuckle.AspNetCore.Swagger.Contact
                        {
                            Name = "Andrew Mitchell",
                            Email = "yes@no.com.com",
                            Url = "yes.no.com"
                        }
                    }
                );
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Email.API.xml"));
            });

            services.AddTransient<IEmailService, EmailService>();

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret); //used to validate the token
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("EmailUser",
                    policy =>
                    {
                        //only care if the user is authenticated
                       policy.RequireAuthenticatedUser();
                      //  policy.RequireRole("DEPT_IT", "IT_USER_ADMIN");
                    });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var swaggerEndpoint = "/swagger/v1/swagger.json";

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                swaggerEndpoint = "/EmailAPI/swagger/v1/swagger.json";
            }


            app.UseAuthentication();
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(swaggerEndpoint, "Email MicroService API");
            });

            //logging
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Warning);

            app.UseCors("CorsPolicy");
            app.UseMvc();
        }
    }
}
