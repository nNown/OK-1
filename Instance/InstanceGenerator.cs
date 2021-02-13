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
        public List<Solution> GenerateRandomSolutions(int solutionNumber) {
            Graph firstGraph = _graphGenerator.GenerateGraph(50, 100);
            Graph secondGraph = _graphGenerator.GenerateGraph(50, 100);

            List<Solution> solutions = new List<Solution>();
            while(solutions.Count < solutionNumber) {
                solutions.Add(GenerateRandomSolution(firstGraph, secondGraph));
            }
            return solutions;
        }
        private Solution GenerateRandomSolution(Graph firstGraph, Graph secondGraph) {
            Solution solution = new Solution(firstGraph, secondGraph);
            
            int i = 0;
            while(GetUnvisitedVertices(firstGraph).Count + GetUnvisitedVertices(secondGraph).Count > 0) {
                if(i % 2 == 0 || GetUnvisitedVertices(secondGraph).Count < 1) {
                    solution.SolutionPath.Add(GeneratePath(firstGraph));
                } else if (i % 2 != 0 || GetUnvisitedVertices(firstGraph).Count < 1){
                    solution.SolutionPath.Add(GeneratePath(secondGraph));
                }
                i += 1;
            }

            return solution;
        }
        private List<Tuple<int, int>> GeneratePath(Graph graph) {
            List<Tuple<int, int>> generatedPath = new List<Tuple<int, int>>();
            Tuple<int, int> choosenVertex = PickRandomVertex(GetUnvisitedVertices(graph));
            while(generatedPath.Count < 10) {
                generatedPath.Add(choosenVertex);
                graph.VertexList[choosenVertex].Visited = true;

                // TODO: Find better way to access first element of dictionary
                Tuple<int, int> currentEdge = new List<Tuple<int, int>>(graph.VertexList[choosenVertex].EdgeList.Keys)[0];
                foreach(var edge in graph.VertexList[choosenVertex].EdgeList) {
                    if(!graph.VertexList[edge.Key].Visited) {
                        currentEdge = edge.Key;
                        continue;
                    }

                    if(edge.Value < graph.VertexList[choosenVertex].EdgeList[currentEdge]) {
                        currentEdge = edge.Key;
                    }
                }

                choosenVertex = currentEdge;
            }
            return generatedPath;
        }
        private Tuple<int, int> PickRandomVertex(List<Tuple<int, int>> vertices) {
            // TODO: Better solution here
            int choosenVertexIndex = _random.Next(0, vertices.Count);
            Tuple<int, int> choosenVertex = vertices[0];

            int iteration = 0;
            foreach(var vertex in vertices) {
                if(iteration == choosenVertexIndex) {
                    choosenVertex = vertex;
                }
                iteration += 1;
            }

            return choosenVertex;
        }
        private List<Tuple<int, int>> GetUnvisitedVertices(Graph graph) {
            List<Tuple<int, int>> unvisitedVertices = new List<Tuple<int, int>>();
            foreach(var vertex in graph.VertexList) {
                if(!vertex.Value.Visited) {
                    unvisitedVertices.Add(vertex.Key);
                }
            }
            return unvisitedVertices;
        }
        private InstanceGenerator() {
            _graphGenerator = GraphGenerator.Instance;
            _random = new Random();
        }
    }
}