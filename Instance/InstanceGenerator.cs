using System;
using System.Collections.Generic;

namespace ok_project {
    public sealed class InstanceGenerator {
        private static readonly Lazy<InstanceGenerator> _generator = new Lazy<InstanceGenerator>(() => new InstanceGenerator());
        public static InstanceGenerator Instance {
            get => _generator.Value;
        }
        private static Random _random;
        public List<Solution> GenerateRandomSolutions(Graph firstGraph, Graph secondGraph, int solutionNumber) {
            List<Solution> solutions = new List<Solution>();
            while(solutions.Count < solutionNumber) {
                solutions.Add(GenerateRandomSolution(firstGraph, secondGraph));
                
                MarkVerticesAsUnvisited(ref firstGraph);
                MarkVerticesAsUnvisited(ref secondGraph);
            }
            return solutions;
        }
        public Solution GenerateRandomSolution(Graph firstGraph, Graph secondGraph) {
            Solution solution = new Solution(firstGraph, secondGraph);
            
            int i = 0;
            // TODO: Reduce calls to GetUnvisitedVertices here
            List<Tuple<int, int>> firstGraphUnvisitedVertices = GetUnvisitedVertices(firstGraph);
            List<Tuple<int, int>> secondGraphUnvisitedVertices = GetUnvisitedVertices(secondGraph);

            while(firstGraphUnvisitedVertices.Count + secondGraphUnvisitedVertices.Count > 0) {
                Graph context = GetGraphContext(firstGraphUnvisitedVertices, secondGraphUnvisitedVertices, i) ? firstGraph : secondGraph;

                List<Tuple<int, int>> subpath = new List<Tuple<int, int>>();
                if(firstGraphUnvisitedVertices.Count == 0) {
                    subpath = GeneratePath(ref secondGraph, secondGraphUnvisitedVertices);
                    context = secondGraph;

                } else if (secondGraphUnvisitedVertices.Count == 0) {
                    subpath = GeneratePath(ref firstGraph, firstGraphUnvisitedVertices);
                    context = firstGraph;

                } else {
                    subpath = GeneratePath(ref context);
                }
                    solution.SolutionPath.Add(subpath);
                    solution.SolutionValue += ComputePathValue(context, subpath) + Graph.DistanceBetweenVertices(solution.SolutionPath[solution.SolutionPath.Count - 1][solution.SolutionPath[solution.SolutionPath.Count - 1].Count - 1], subpath[0]);
                Console.WriteLine("Context: {0}, Path length: {1}", GetGraphContext(firstGraphUnvisitedVertices, secondGraphUnvisitedVertices, i) ? "First Graph" : "Second Graph", subpath.Count);
                i += 1;
                firstGraphUnvisitedVertices = GetUnvisitedVertices(firstGraph);
                secondGraphUnvisitedVertices = GetUnvisitedVertices(secondGraph);
            }
            return solution;
        }
        private List<Tuple<int, int>> GeneratePath(ref Graph graph) {
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

                    if(generatedPath.Contains(edge.Key)) {
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
        private List<Tuple<int, int>> GeneratePath(ref Graph graph, List<Tuple<int, int>> univisitedVertices) {
            List<Tuple<int, int>> generatedPath = new List<Tuple<int, int>>();
            Tuple<int, int> choosenVertex = PickRandomVertex(univisitedVertices);
            univisitedVertices.Remove(choosenVertex);

            while(univisitedVertices.Count > 0) {
                graph.VertexList[choosenVertex].Visited = true;

                Tuple<int, int> randomDestination = PickRandomVertex(univisitedVertices);
                List<Tuple<int, int>> pathBetweenVertices = Graph.PathBetweenVertices(graph, choosenVertex, randomDestination);
                foreach(var vertex in pathBetweenVertices) {
                    generatedPath.Add(vertex);
                }

                choosenVertex = randomDestination;
                univisitedVertices.Remove(choosenVertex);
            }
            generatedPath.Add(choosenVertex);
            graph.VertexList[choosenVertex].Visited = true;

            return generatedPath;
        }

        private void ConcatenatePaths(ref List<Tuple<int, int>> firstPath, List<Tuple<int, int>> secondPath) {
            foreach(var vertex in secondPath) {
                firstPath.Add(vertex);
            }
        }

        private Tuple<int, int> PickRandomVertex(List<Tuple<int, int>> vertices) {
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
        private bool GetGraphContext(List<Tuple<int, int>> firstGraphVerticesPool, List<Tuple<int, int>> secondGraphVerticesPool, int iteration) {
            if(firstGraphVerticesPool.Count > 0) {
                if(iteration % 2 != 0 && secondGraphVerticesPool.Count > 0) {
                    return false;
                } else {
                    return true;
                }
            } else {
                return false;
            }
        }
        private int ComputePathValue(Graph graph, List<Tuple<int, int>> solutionSubpath) {
            int solutionValue = 0;
            for(int i = 0, j = 1; i < solutionSubpath.Count - 1; i++, j++) {
                solutionValue += graph.VertexList[solutionSubpath[i]].EdgeList[solutionSubpath[j]];
            }
            return solutionValue;
        }
        private void MarkVerticesAsUnvisited(ref Graph graph) {
            foreach(var vertex in graph.VertexList) {
                vertex.Value.Visited = false;
            }
        }
        private InstanceGenerator() {
            _random = new Random();
        }
    }
}