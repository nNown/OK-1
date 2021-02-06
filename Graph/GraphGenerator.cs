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
                            if(graph.VertexList[vertex].EdgeList.Count >= graph.VertexList[vertex].Degree - 2) break;
                            if(vertex == comparedVertex) continue;

                            if(graph.DistanceBetweenVertices(vertex, comparedVertex) <= maxWeight && !graph.HasEdge(vertex, comparedVertex) && graph.VertexList[comparedVertex].EdgeList.Count < graph.VertexList[comparedVertex].Degree) {
                                // Console.WriteLine("Added edge: ({0}, {1}) - ({2}, {3})", vertex.Item1, vertex.Item2, comparedVertex.Item1, comparedVertex.Item2);
                                graph.AddEdge(vertex, comparedVertex);
                            }
                        }
                    }

                    if(graph.VertexList[vertex].EdgeList.Count < graph.VertexList[vertex].Degree) {
                        for(int i = 0; i < graph.VertexList[vertex].Degree - graph.VertexList[vertex].EdgeList.Count; i++) {
                            if(graph.VertexList.Count >= graphSize) break;

                            double distance, theta;
                            Tuple<int, int> newVertex = vertex;
                            while(graph.VertexList.ContainsKey(newVertex)) {
                                distance = maxWeight * Math.Sqrt(_random.NextDouble());
                                theta = _random.NextDouble() * 2 * Math.PI;

                                newVertex = new Tuple<int, int>((int) (vertex.Item1 + distance * Math.Cos(theta)), (int) (vertex.Item2 + distance * Math.Sin(theta)));
                            }
                            // Console.WriteLine("Added vertex: ({0}, {1})", newVertex.Item1, newVertex.Item2);
                            graph.AddVertex(newVertex);
                            graph.AddEdge(vertex, newVertex);
                        }
                    }
                }
            }
            // Console.WriteLine("Graph Generated");
            return graph;
        }
        private GraphGenerator() {
            _random = new Random();
        }
    }
}