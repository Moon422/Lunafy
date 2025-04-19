using System;
using Lunafy.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lunafy.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
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
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Lunafy API",
                Version = "v1"
            });
        });
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

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}