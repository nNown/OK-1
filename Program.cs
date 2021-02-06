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
            var Generator = InstanceGenerator.Instance;
            Instance test = Generator.RandomSolution();
            foreach(var vertex in test.Solution) {
                Console.WriteLine("({0}, {1})", vertex.Item1, vertex.Item2);
            }
        }
        private static IHostBuilder CreateHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => {
                    services.AddTransient<Program>();
                    services.AddTransient<Graph>();
                    services.AddTransient<GraphGenerator>();
                    services.AddTransient<Instance>();
                    services.AddTransient<InstanceGenerator>();
                });
        }
    }
}