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
            int[] graphSizes = { 30, 35, 40, 45, 50, 55, 60 };
            int[] initialGeneratedSolutions = { 100, 125, 150, 175, 200, 225, 250, 275, 300 };
            double[] initialSolutionsThreshold = { 0.15, 0.5, 0.85 };

            int[] iterations = { 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000, 11000, 12000, 13000, 14000, 15000 };
            int[] antsNumber = { 10, 20, 30, 40, 50 };
            Tuple<double, double, int>[] pheromoneUsages = { new Tuple<double, double, int>(0.1, 0.8, 8), new Tuple<double, double, int>(0.05, 0.9, 18), new Tuple<double, double, int>(0.2, 0.6, 3), new Tuple<double, double, int>(0.25, 1, 4), new Tuple<double, double, int>(0.1, 0.5, 5) };
            double[] chancesToPickVertex = { 0.5, 0.6, 0.7, 0.8 };
            double[] vaporationRates = { 0.05, 0.1, 0.15, 0.2, 0.25, 0.3, 0.35, 0.4, 0.45, 0.5 };
            double[] vaporationChances = { 0.2, 0.4, 0.6, 0.8, 1 };
            double[] solutionsTakenIntoPheromonesTables = { 0.25, 0.5, 0.75, 1 };

            // foreach(var size in graphSizes) {
            //     foreach(var initialSolutions in initialGeneratedSolutions) {
            //         foreach(var initialThreshold in initialSolutionsThreshold) {
                        foreach(var iterationUpperBound in iterations) {
                            foreach(var ants in antsNumber) {
                                foreach(var pheromoneUsage in pheromoneUsages) {
                                    foreach(var chanceToPickVertex in chancesToPickVertex) {
                                        foreach(var vaporationRate in vaporationRates) {
                                            foreach(var vaporationChance in vaporationChances) {
                                                foreach(var solutionTakenIntoTable in solutionsTakenIntoPheromonesTables) {
                                                    try {
                                                        AntColony colony = new AntColony(50, 100, 150, 0.5);
                                                        colony.Optimize(iterationUpperBound, ants, pheromoneUsage.Item1, pheromoneUsage.Item2, chanceToPickVertex, vaporationChance, vaporationRate, pheromoneUsage.Item3, solutionTakenIntoTable);
                                                    } catch {
                                                        Console.WriteLine("\nError in generating");
                                                        continue;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
            //         } 
            //     }
            // }
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