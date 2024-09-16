using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using EVOptimizationAPI.Services;

namespace EVOptimizationAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        // Constructor to access configuration settings
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register services needed by the app
            services.AddControllers(); // Add support for MVC controllers

            // Example of adding a singleton service, e.g., for managing EV data
            services.AddSingleton<IEVService, EVService>();
            services.AddTransient<IEVOptimizationService, EVOptimizationService>();

            // Optional: Add API versioning, Swagger, etc.
            services.AddSwaggerGen(); // Enable Swagger for API documentation
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            // Enable Swagger if running in development
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EVOptimizationAPI v1"));
            }

            app.UseRouting();

            // Example of adding global error handling middleware
            app.Use(async (context, next) =>
            {
                try
                {
                    await next.Invoke();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while processing the request.");
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync("An unexpected error occurred.");
                }
            });

            // Example of using authorization if needed (can be removed if not necessary)
            app.UseAuthorization();

            // Register the endpoints for API controllers
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
