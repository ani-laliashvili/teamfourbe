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

            // CORS configuration to allow requests from the React app (localhost:3000)
            services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", builder =>
                {
                    builder
                        .WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            // Register services needed by the app
            services.AddSingleton<IEVService, EVService>();
            services.AddTransient<IEVOptimizationService, EVOptimizationService>();

            // Add Swagger for API documentation
            services.AddSwaggerGen();

            // Optional: Add other services like database, authentication, etc.
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
            else
            {
                // Redirect HTTP to HTTPS in production
                app.UseHttpsRedirection();
            }

            // Enable CORS for the React app
            app.UseCors("AllowReactApp");

            app.UseRouting();

            // Global error handling middleware
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

            app.UseAuthorization(); // Use if you have authentication/authorization

            // Register the endpoints for API controllers
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
