using System;
using System.Collections.Generic;

namespace ok_project {
    public sealed class GraphGenerator {
        private static readonly Lazy<GraphGenerator> _generator = new Lazy<GraphGenerator>(() => new GraphGenerator());
        public static GraphGenerator Instance {
            get => _generator.Value;
        }
        private static Random _random;
        public Graph GenerateGraph(int graphSize, int maxWeight) {
            Graph graph = new Graph();

            graph.AddVertex(new Tuple<int, int>(_random.Next(100, 10000), _random.Next(100, 10000)), 6);

            while(graph.VertexList.Count < graphSize) {
                List<Tuple<int, int>> verticesWithCapacity = GetVerticesWithCapacityForNewEdges(ref graph);

                if(verticesWithCapacity.Count < 1) {
                    int numberOfCandidatesToDeleteEdges = graphSize - graph.VertexList.Count > graph.VertexList.Count ? graph.VertexList.Count : graphSize - graph.VertexList.Count;
                    CreateSpaceForNewEdges(ref graph, numberOfCandidatesToDeleteEdges, 0.5);
                } else {
                    GenerateVertices(ref graph, verticesWithCapacity, maxWeight);
                    PopulateGraphWithEdges(ref graph, maxWeight);
                }
            }

            return graph;
        }
        private void PopulateGraphWithEdges(ref Graph graph, int maxWeightOfEdge) {
            foreach(var vertex in graph.VertexList) {
                foreach(var comparedVertex in graph.VertexList) {
                    if(vertex.Key == comparedVertex.Key) continue;

                    if(!graph.HasEdge(vertex.Key, comparedVertex.Key)) {
                        if(graph.DistanceBetweenVertices(vertex.Key, comparedVertex.Key) < maxWeightOfEdge) {
                            if(vertex.Value.EdgeList.Count < vertex.Value.Degree && comparedVertex.Value.EdgeList.Count < comparedVertex.Value.Degree) {
                                graph.AddEdge(vertex.Key, comparedVertex.Key);
                            }
                        }
                    }

                }
            }
        }
        private void GenerateVertices(ref Graph graph, List<Tuple<int, int>> verticesWithCapacity, int maxWeightOfEdge) {
            foreach(var vertex in verticesWithCapacity) {
                while(graph.VertexList[vertex].EdgeList.Count < graph.VertexList[vertex].Degree) {
                    Tuple<int, int> generatedVertex = GeneratePointWithinCircle(vertex, maxWeightOfEdge);
                    while(graph.VertexList.ContainsKey(generatedVertex)) {
                        generatedVertex = GeneratePointWithinCircle(vertex, maxWeightOfEdge);
                    }

                    graph.AddVertex(generatedVertex);
                    graph.AddEdge(vertex, generatedVertex);
                }
            }
        }
        private List<Tuple<int, int>> GetVerticesWithCapacityForNewEdges(ref Graph graph) {
            List<Tuple<int, int>> verticesWithCapacity = new List<Tuple<int, int>>();

            foreach(var vertex in graph.VertexList) {
                if(vertex.Value.EdgeList.Count < vertex.Value.Degree) {
                    verticesWithCapacity.Add(vertex.Key);
                }
            }

            return verticesWithCapacity;
        }
        private void CreateSpaceForNewEdges(ref Graph graph, int numberOfVertices, double capacityToLeave) {
            List<int> randomIndexes = GenerateListOfRandomValues(numberOfVertices, graph.VertexList.Count);

            int index = 0;
            foreach(var vertex in graph.VertexList) {
                if(randomIndexes.Contains(index)) {
                    if(vertex.Value.Degree < 2) continue;

                    int numberOfEdgesToDelete = vertex.Value.Degree - Math.Floor(vertex.Value.Degree * capacityToLeave) < 2 ? 1 : (int) (vertex.Value.Degree - Math.Floor(vertex.Value.Degree * capacityToLeave));

                    foreach(var edge in vertex.Value.EdgeList) {
                        if(numberOfEdgesToDelete < 1) break;

                        vertex.Value.EdgeList.Remove(edge.Key);
                        numberOfEdgesToDelete -= 1;
                    }
                }

                index += 1;
            }
        }
        private Tuple<int, int> GeneratePointWithinCircle(Tuple<int, int> centerPoint, int maxRadius) {
            double distanceFromCenterPoint = maxRadius * Math.Sqrt(_random.NextDouble());
            double theta = _random.NextDouble() * 2 * Math.PI;

            return new Tuple<int, int>((int) Math.Floor(centerPoint.Item1 + distanceFromCenterPoint * Math.Cos(theta)), (int) Math.Floor(centerPoint.Item2 + distanceFromCenterPoint * Math.Sin(theta)));
        }
        private List<int> GenerateListOfRandomValues(int listSize, int rangeUpperBound) {
            List<int> randomValues = new List<int>();

            while(randomValues.Count < listSize) {
                int value = _random.Next(0, rangeUpperBound);
                while(randomValues.Contains(value)) {
                    value = _random.Next(0, rangeUpperBound);
                }

                randomValues.Add(value);
            }

            return randomValues;
        }
        private GraphGenerator() {
            _random = new Random();
        }
    }
}