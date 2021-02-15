using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace ok_project {
    class Program {
        static void Main(string[] args) {
            var host = CreateHostBuilder(args).Build();
            host.Services.GetRequiredService<Program>().Run();
        }
        public void Run() {
            AntColony test = new AntColony(50, 100, 1, 0.5);
            // foreach(var solution in test.ConstructedSolutions) {
            //     foreach(var path in solution.SolutionPath) {
            //         Console.Write("[");
            //         foreach(var vertex in path) {
            //             Console.Write("({0}, {1}) ", vertex.Item1, vertex.Item2);
            //         }
            //         Console.Write("], ");
            //     }
            //     Console.WriteLine();
            // }
            // var instanceGenerator = InstanceGenerator.Instance;
            // var graphGenerator = GraphGenerator.Instance;
            // Graph test = graphGenerator.GenerateGraph(50, 100);
            // List<Tuple<int, int>> vertices = new List<Tuple<int, int>>(test.VertexList.Keys);
            // List<Tuple<int, int>> path = Graph.PathBetweenVertices(test, vertices[0], vertices[10]);
            // Console.WriteLine("({0}, {1}) -> ({2}, {3})", vertices[0].Item1, vertices[0].Item2, vertices[10].Item1, vertices[10].Item2);
            // foreach(var vertex in path) {
            //     Console.WriteLine("({0}, {1})", vertex.Item1, vertex.Item2);
            // }

            // Solution test = instanceGenerator.GenerateRandomSolution(graphGenerator.GenerateGraph(50, 100), graphGenerator.GenerateGraph(50, 100));
                // foreach(var path in test.SolutionPath) {
                //     foreach(var vertex in path) {
                //         Console.Write("({0}, {1}), ", vertex.Item1, vertex.Item2);
                //     }
                // }
                // Console.WriteLine("\n");
                // Console.WriteLine(test.SolutionValue);
                // Console.WriteLine("\n");

            // for(int i = 0; i < 100; i++) {
            //     var Generator = GraphGenerator.Instance;
            //     Graph test = Generator.GenerateGraph(100, 100);
            //     Console.WriteLine("Graph {0}", i);
            //     foreach(var vertex in test.VertexList) {
            //         Console.Write("Vertex: ({0}, {1}):\n", vertex.Key.Item1, vertex.Key.Item2);
            //         foreach(var edge in vertex.Value.EdgeList) {
            //             Console.Write("({0}, {1}), {2}\n", edge.Key.Item1, edge.Key.Item2, edge.Value);
            //         }
            //     }
            //     Console.WriteLine();
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