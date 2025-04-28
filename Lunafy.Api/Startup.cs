using System;
using System.Text;
using Lunafy.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Lunafy.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }
    private readonly string _myAllowSpecificOrigins = "_myAllowSpecificOrigins";

    public void ConfigureServices(IServiceCollection services)
    {
        // var corsOrigins = Configuration.GetValue<string[]>("AllowedOrigins")
        //     ?? throw new InvalidOperationException("Cors origins cannot be empty");

        services.AddCors(options =>
        {
            options.AddPolicy(name: _myAllowSpecificOrigins,
                policy =>
                {
                    policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
        });

        services.AddHttpContextAccessor();

        services.AddMemoryCache();

        services.AddDbContext<LunafyDbContext>(options =>
        {
            var connectionString = Configuration.GetConnectionString("mysql");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException();

            var version = ServerVersion.AutoDetect(connectionString) ??
                throw new InvalidOperationException();

            options.UseMySql(connectionString, version);
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.RegisterServices();

        services.RegisterEventConsumers();

        services.AddAutoMapper(typeof(Startup));

        services.AddControllers();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Lunafy API",
                Version = "v1"
            });

            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
            });

            c.OperationFilter<SecurityRequirementsOperationFilter>();
        });

        services.AddAuthentication().AddJwtBearer(
            options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        Configuration.GetSection("Secret").Value!
                    ))
                };
            }
        );
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lunafy API v1");
            });
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseCors(_myAllowSpecificOrigins);

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}