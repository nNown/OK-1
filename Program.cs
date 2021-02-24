using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace ok_project {
    class Program {
        static void Main(string[] args) {
            var host = CreateHostBuilder(args).Build();
            host.Services.GetRequiredService<Program>().Run(args);
        }
        public void Run(string[] args) {
            GraphGenerator generator = GraphGenerator.Instance;
            try {
                int graphSize = Int32.Parse(args[0]);
                int maxWeight = Int32.Parse(args[1]);
                int startingSolutionsNumber = Int32.Parse(args[2]);
                double startingSolutionsThreshold = Double.Parse(args[3]);

                int iterations = Int32.Parse(args[4]);
                int antsNumber = Int32.Parse(args[5]);
                double startingPheromoneUsageChance = Double.Parse(args[6]);
                double maxPheromoneUsageChance = Double.Parse(args[7]);
                double maxChanceToPickVertex = Double.Parse(args[8]); // Smoothing pheromones
                double evaporationChance = Double.Parse(args[9]);
                double evaporationRate = Double.Parse(args[10]);
                int pheromonesUsageGrowthRate = Int32.Parse(args[11]);
                double solutionsThreshold = Double.Parse(args[12]);

                AntColony colony = new AntColony(graphSize, maxWeight, startingSolutionsNumber, startingSolutionsThreshold, args[13]);
                colony.Optimize(iterations, antsNumber, startingPheromoneUsageChance, maxPheromoneUsageChance, maxChanceToPickVertex, evaporationChance, evaporationRate, pheromonesUsageGrowthRate, solutionsThreshold, args[13]);
            } catch {
                Console.WriteLine("\nError in generating graph, try again");
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