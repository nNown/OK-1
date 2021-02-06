using System;
using System.Collections.Generic;

namespace ok_project {
    public sealed class InstanceGenerator {
        private static readonly Lazy<InstanceGenerator> _generator = new Lazy<InstanceGenerator>(() => new InstanceGenerator());
        public static InstanceGenerator Instance {
            get => _generator.Value;
        }
        private static GraphGenerator _graphGenerator;
        private static Random _random;
        public Instance RandomSolution() {
            Instance instance = new Instance(_graphGenerator.GenerateGraph(50, 100), _graphGenerator.GenerateGraph(50, 100));
            instance.Solution = new List<Tuple<int, int>>();
            
            instance.Solution.Add(new List<Tuple<int, int>>(instance.FirstGraph.VertexList.Keys)[_random.Next(0, instance.FirstGraph.VertexList.Keys.Count)]);
            int currentVertexNumber = 1, visitedUniqueVertices = 1;
            Graph context = instance.FirstGraph;
            Tuple<int, int> currentVertex = instance.Solution[0];
            while(visitedUniqueVertices < instance.FirstGraph.VertexList.Count + instance.SecondGraph.VertexList.Count) {
                if(currentVertexNumber >= 10) {
                    if(context == instance.FirstGraph) {
                        foreach(var vertex in instance.SecondGraph.VertexList) {
                            if(!vertex.Value.Visited) {
                                currentVertex = vertex.Key;
                                currentVertexNumber = 1;
                                context = instance.SecondGraph;
                                break;
                            }
                        }
                    } else {
                        foreach(var vertex in instance.FirstGraph.VertexList) {
                            if(!vertex.Value.Visited) {
                                currentVertex = vertex.Key;
                                currentVertexNumber = 1;
                                context = instance.FirstGraph;
                                break;
                            }
                        }
                    }
                }

                List<Tuple<int, int>> edges = new List<Tuple<int, int>>(context.VertexList[currentVertex].EdgeList.Keys);
                Tuple<int, int> localOptimalVertex = edges[0];
                bool foundUnique = false;
                foreach(var edge in edges) {
                    if(!context.VertexList[edge].Visited) {
                        instance.Solution.Add(edge);
                        currentVertexNumber += 1;
                        visitedUniqueVertices += 1;
                        context.VertexList[edge].Visited = true;
                        currentVertex = edge;
                        foundUnique = true;
                        break;
                    }

                    if(context.VertexList[currentVertex].EdgeList[edge] < context.VertexList[currentVertex].EdgeList[localOptimalVertex]) {
                        localOptimalVertex = edge;
                    }
                }

                if(!foundUnique) {
                    instance.Solution.Add(localOptimalVertex);
                    currentVertexNumber += 1;
                    currentVertex = localOptimalVertex;
                    foundUnique = false;
                }
            }

            return instance;
        }

        private InstanceGenerator() {
            _graphGenerator = GraphGenerator.Instance;
            _random = new Random();
        }
    }
}