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
            var Generator = GraphGenerator.Instance;
            var test = Generator.GenerateGraph(10, 100);
            foreach(var vertex in test.VertexList) {
                Console.Write("Vertex ({0}, {1}): \n", vertex.Key.Item1, vertex.Key.Item2);
                foreach(var edge in vertex.Value.EdgeList) {
                    Console.Write("\t({0}, {1}) : {2}, {3}\n", edge.Key.Item1, edge.Key.Item2, edge.Value, test.VertexList[edge.Key].Visited);
                }
            }
        }
        private static IHostBuilder CreateHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => {
                    services.AddTransient<Program>();
                    services.AddTransient<Graph>();
                    services.AddTransient<GraphGenerator>();
                });
        }
    }
}