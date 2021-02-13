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
            // var Generator = InstanceGenerator.Instance;
            // Instance test = Generator.RandomSolution();
            // foreach(var vertex in test.Solution) {
            //     Console.WriteLine("({0}, {1})", vertex.Item1, vertex.Item2);
            // }
            for(int i = 0; i < 100; i++) {
                var Generator = GraphGenerator.Instance;
                Graph test = Generator.GenerateGraph(100, 100);
                Console.WriteLine("Graph {0}", i);
                foreach(var vertex in test.VertexList) {
                    Console.Write("Vertex: ({0}, {1}):\n", vertex.Key.Item1, vertex.Key.Item2);
                    foreach(var edge in vertex.Value.EdgeList) {
                        Console.Write("({0}, {1}), {2}\n", edge.Key.Item1, edge.Key.Item2, edge.Value);
                    }
                }
                Console.WriteLine();
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