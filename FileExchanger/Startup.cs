using Core;
using Core.Enums;
using FileExchanger.Attributes;
using FileExchanger.Interfaces;
using FileExchanger.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;

namespace FileExchanger
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
            services.AddSwaggerGen();
            services.AddDbContext<DbApp>(options =>
                options.UseSqlServer(Config.Instance.DbConnect));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateIssuerSigningKey = true,
                            ValidateLifetime = true,
                            ValidIssuer = Config.Instance.Auth.Issuer,
                            ValidAudience = Config.Instance.Auth.Audience,
                            IssuerSigningKey = Config.Instance.Auth.GetSymmetricSecurityKey
                        };
                    });
            services.AddAuthorization();
            services.AddControllers();
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = 5905580032;//5.5Gb // In case of multipart
            });
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
            });

            services.AddTransient<IAuthorizationHandler, AuthHandler>();
            services.AddTransient<IAuthorizationHandler, AdminHandler>();
            services.AddTransient<IShareService, ShareService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IDirectoryService, DirectoryService>();
            services.AddTransient<IStorageFileService, StorageFileService>();

            services.AddAuthorization(opts => {
                opts.AddPolicy("AuthStorage",
                    policy => policy.Requirements.Add(new AuthRequirement(DefaultService.FileStorage)));
                opts.AddPolicy("AuthExchanger",
                    policy => policy.Requirements.Add(new AuthRequirement(DefaultService.FileExchanger)));
                opts.AddPolicy("Admin",
                    policy => policy.Requirements.Add(new AdminRequirement()));
            });    
            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseWebSockets();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "wwwroot";
            });
        }
    }
}
