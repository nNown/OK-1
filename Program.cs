using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ok_project {
    class Program {
        static void Main(string[] args) {
            var host = CreateHostBuilder(args).Build();
            host.Services.GetRequiredService<Program>().Run();
        }
        public void Run() {
            GraphGenerator generator = GraphGenerator.Instance;

            try {
                AntColony colony = new AntColony(25, 100, 150, 0.5);
                colony.Optimize(2000, 100, 0.1, 0.8, 0.6, 0.2, 0.1, 7, 0.9);
            } catch {
                Console.WriteLine("\nError in generating");
            }
        }
        private static IHostBuilder CreateHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => {
                    services.AddTransient<Program>();
                    services.AddTransient<Graph>();
                    services.AddTransient<GraphGenerator>();
                    services.AddTransient<Solution>();
                    services.AddTransient<InstanceGenerator>();
                    services.AddTransient<AntColony>();
                });
        }
    }
}