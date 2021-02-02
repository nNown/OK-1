using System;
using System.Collections.Generic;

namespace ok_project {
    public sealed class GraphGenerator {
        private static readonly Lazy<GraphGenerator> _generator = new Lazy<GraphGenerator>(() => new GraphGenerator());
        public static GraphGenerator Instance {
            get => _generator.Value;
        }
        public Graph GenerateGraph() {
            List<Tuple<long, long>> vertices = new List<Tuple<long, long>>();
            vertices.Add(new Tuple<long, long>(15, 16));
            vertices.Add(new Tuple<long, long>(21, 12));
            vertices.Add(new Tuple<long, long>(24, 8));
            return new Graph(vertices);
        }
        private GraphGenerator() {
            Console.WriteLine("Graph generator created");
        }
    }
}