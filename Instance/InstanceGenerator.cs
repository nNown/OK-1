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
            List<Tuple<int, int>> firstGraphUnvisitedVertices = GetUnvisitedVertices(firstGraph);
            List<Tuple<int, int>> secondGraphUnvisitedVertices = GetUnvisitedVertices(secondGraph);

            Graph context = firstGraph;
            while(firstGraphUnvisitedVertices.Count > 0 && secondGraphUnvisitedVertices.Count > 0) {
                List<Tuple<int, int>> subpath = GeneratePath(ref context);
                solution.SolutionPath.Add(subpath);
                solution.SolutionValue += ComputePathValue(context, subpath) + solution.SolutionPath.Count > 1 ? Graph.DistanceBetweenVertices(solution.SolutionPath[solution.SolutionPath.Count - 1][solution.SolutionPath[solution.SolutionPath.Count - 1].Count - 1], subpath[0]) : 0;

                firstGraphUnvisitedVertices = GetUnvisitedVertices(firstGraph);
                secondGraphUnvisitedVertices = GetUnvisitedVertices(secondGraph);
                i += 1;
                context = GetGraphContext(firstGraphUnvisitedVertices, secondGraphUnvisitedVertices, i) ? firstGraph : secondGraph;
            }
            List<Tuple<int, int>> contextUnvisitedVertices = GetUnvisitedVertices(context);
            List<Tuple<int, int>> finalPath = GeneratePath(ref context, contextUnvisitedVertices);

            solution.SolutionPath.Add(finalPath);
            solution.SolutionValue += ComputePathValue(context, finalPath) + Graph.DistanceBetweenVertices(solution.SolutionPath[solution.SolutionPath.Count - 1][solution.SolutionPath[solution.SolutionPath.Count - 1].Count - 1], finalPath[0]);

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

        public Tuple<int, int> PickRandomVertex(List<Tuple<int, int>> vertices) {
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
        public List<Tuple<int, int>> GetUnvisitedVertices(Graph graph) {
            List<Tuple<int, int>> unvisitedVertices = new List<Tuple<int, int>>();
            foreach(var vertex in graph.VertexList) {
                if(!vertex.Value.Visited) {
                    unvisitedVertices.Add(vertex.Key);
                }
            }
            return unvisitedVertices;
        }
        public bool GetGraphContext(List<Tuple<int, int>> firstGraphVerticesPool, List<Tuple<int, int>> secondGraphVerticesPool, int iteration) {
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
        public int ComputePathValue(Graph graph, List<Tuple<int, int>> solutionSubpath) {
            int solutionValue = 0;
            for(int i = 0, j = 1; i < solutionSubpath.Count - 1; i++, j++) {
                solutionValue += graph.VertexList[solutionSubpath[i]].EdgeList[solutionSubpath[j]];
            }
            return solutionValue;
        }
        public void MarkVerticesAsUnvisited(ref Graph graph) {
            foreach(var vertex in graph.VertexList) {
                vertex.Value.Visited = false;
            }
        }
        private InstanceGenerator() {
            _random = new Random();
        }
    }
}