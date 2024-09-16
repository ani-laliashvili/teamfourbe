using CoreLibrary;
using EVOptimizationAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace EVOptimizationAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Seed data after building the host
            SeedData(host);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        // Seeding EV data
        private static void SeedData(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // Get the EVService (or any other service you want to use to seed data)
                var evService = services.GetRequiredService<IEVService>();

                // Seed initial EV data
                var initialEVs = new List<EV>
                {
                    new EV(1, 2, 80, 70, 50, true, 2), // EV with 80% charge and 2% consumption rate per mile
                    new EV(1, 3, 70, 35, 50, true, 2), // EV with 50% charge and 1.5% consumption rate per mile
                    new EV(1, 2, 80, 80, 50, true, 2) // Fully charged EV with 2.5% consumption rate per mile
                };

                foreach (var ev in initialEVs)
                {
                    evService.AddEV(ev); // Assuming AddEV method exists in the IEVService
                }
            }
        }
    }
}
