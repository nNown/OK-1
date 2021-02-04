using System;
using System.Collections.Generic;

namespace ok_project {
    public sealed class GraphGenerator {
        private static readonly Lazy<GraphGenerator> _generator = new Lazy<GraphGenerator>(() => new GraphGenerator());
        public static GraphGenerator Instance {
            get => _generator.Value;
        }
        private static Random _random;
        public Graph GenerateGraph(uint graphSize, uint maxWeight) {
            Graph graph = new Graph();

            graph.AddVertex(new Tuple<int, int>(_random.Next(1, 10000), _random.Next(1, 10000)));
            while(graph.VertexList.Count < graphSize) {
                List<Tuple<int, int>> vertices = new List<Tuple<int, int>>(graph.VertexList.Keys);

                foreach(var vertex in vertices) {
                    if(graph.VertexList[vertex].EdgeList.Count < graph.VertexList[vertex].Degree) {
                        foreach(var comparedVertex in vertices) {
                            if(graph.VertexList[vertex].EdgeList.Count >= graph.VertexList[vertex].Degree) break;
                            if(vertex == comparedVertex) continue;

                            if(graph.DistanceBetweenVertices(vertex, comparedVertex) <= maxWeight && !graph.HasEdge(vertex, comparedVertex)) {
                                graph.AddEdge(vertex, comparedVertex);
                            }
                        }
                    }

                    if(graph.VertexList[vertex].EdgeList.Count < graph.VertexList[vertex].Degree) {
                        Random random = new Random();
                        for(int i = 0; i < graph.VertexList[vertex].Degree - graph.VertexList[vertex].EdgeList.Count; i++) {
                            double distance = 100 * Math.Sqrt(random.NextDouble());
                            double theta = random.NextDouble() * 2 * Math.PI;

                            Tuple<int, int> newVertex = new Tuple<int, int>((int) (vertex.Item1 + distance * Math.Cos(theta)), (int) (vertex.Item2 + distance * Math.Sin(theta)));
                            graph.AddVertex(newVertex);
                            graph.AddEdge(vertex, newVertex);
                        }
                    }
                }
            }

            return graph;
        }
        private GraphGenerator() {
            _random = new Random();
        }
    }
}