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

            Graph test = Generator.GenerateGraph();
            foreach(Tuple<long, long> vertex in test.EdgeList.Keys) {
                foreach(var edge in test.EdgeList[vertex]) {
                    Console.WriteLine("Vertex ({0}, {1}): ({2}, {3}), {4}, {5}", vertex.Item1, vertex.Item2, edge.Key.Item1, edge.Key.Item2, edge.Value.Item1, edge.Value.Item2);
                }
            }
        }
        private static IHostBuilder CreateHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => {
                    services.AddTransient<Program>();
                    services.AddTransient<Graph>();
                });
        }
    }
}
