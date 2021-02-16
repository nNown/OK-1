using System;
using System.Collections.Generic;

namespace ok_project {
    public class AntColony {
        private Graph _firstGraph;
        private Graph _secondGraph;
        private Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, double>> _firstGraphPheromonesTable;
        private Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, double>> _secondGraphPheromonesTable;
        private Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, double>> _jumpPheromonesTable;
        private List<Solution> _currentGeneratedSolutions;
        private static GraphGenerator _graphGenerator;
        private static InstanceGenerator _solutionGenerator;
        public List<Solution> ConstructedSolutions {
            get => _currentGeneratedSolutions;
        }
        private void AddPheromones(Tuple<int, int> source, Tuple<int, int> destination, int weight, ref Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, double>> pheromonesTable) {
            pheromonesTable[source][destination] += 1.0 / weight;
        }
        private void AddPheromonesToJumpTable(Tuple<int, int> source, Tuple<int, int> destination, ref Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, double>> pheromonesTable, int weightMultiplier) {
            if(_firstGraph.VertexList.ContainsKey(source)) _jumpPheromonesTable[source][destination] += 1.0 / (Graph.DistanceBetweenVertices(source, destination) * weightMultiplier);
            else _jumpPheromonesTable[destination][source] += 1.0 / (Graph.DistanceBetweenVertices(destination, source) * weightMultiplier);
        }
        private void VaporizePheromones() {
            
        }
        private void SmoothePheromones() {

        } 
        private static Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, double>> InitializePheromonesTable(Graph graph) {
            Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, double>> pheromonesTable = new Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, double>>();
            foreach(var vertex in graph.VertexList) {
                pheromonesTable.Add(vertex.Key, new Dictionary<Tuple<int, int>, double>());
                foreach(var edge in vertex.Value.EdgeList) {
                    pheromonesTable[vertex.Key].Add(edge.Key, 0);
                }
            }
            return pheromonesTable;
        }
        private static Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, double>> InitializePheromonesTable(Graph firstGraph, Graph secondGraph) {
            Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, double>> pheromonesTable = new Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, double>>();
            foreach(var firstGraphVertex in firstGraph.VertexList) {
                pheromonesTable.Add(firstGraphVertex.Key, new Dictionary<Tuple<int, int>, double>());
                foreach(var secondGraphVertex in secondGraph.VertexList) {
                    pheromonesTable[firstGraphVertex.Key].Add(secondGraphVertex.Key, 0);
                }
            }
            return pheromonesTable;
        }
        private void FillJumpPheromonesTable(Solution solution, ref Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, double>> pheromonesTable, int weightMultiplier) {
            Tuple<int, int> previousVertex = solution.SolutionPath[0][solution.SolutionPath[0].Count - 1];
            for(int i = 1; i < solution.SolutionPath.Count; i++) {
                AddPheromonesToJumpTable(previousVertex, solution.SolutionPath[i][0], ref pheromonesTable, weightMultiplier);
                previousVertex = solution.SolutionPath[i][solution.SolutionPath[i].Count - 1];
            }
        }
        private void FillPheromonesTable(Graph graph, List<List<Tuple<int, int>>> solutionPathsFromGraph, ref Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, double>> pheromonesTable, int weightMultiplier) {
            foreach(var path in solutionPathsFromGraph) {
                for(int i = 0, j = 1; i < path.Count - 1; i++, j++) {
                    AddPheromones(path[i], path[j], graph.VertexList[path[i]].EdgeList[path[j]] * weightMultiplier, ref pheromonesTable);
                }
            }
        }
        private List<List<Tuple<int, int>>> GetPathsFromOneContext(Solution solution, int iterationStartingPoint) {
            List<List<Tuple<int, int>>> paths = new List<List<Tuple<int, int>>>();
            for(int i = iterationStartingPoint; i < solution.SolutionPath.Count; i += 2) {
                paths.Add(solution.SolutionPath[i]);
            }
            return paths;
        }
        public AntColony(int graphSize, int maxWeight, int solutionsNumber, double solutionsThreshold) {
            _graphGenerator = GraphGenerator.Instance;
            _solutionGenerator = InstanceGenerator.Instance;

            _firstGraph = _graphGenerator.GenerateGraph(graphSize, maxWeight);
            _secondGraph = _graphGenerator.GenerateGraph(graphSize, maxWeight);

            _firstGraphPheromonesTable = InitializePheromonesTable(_firstGraph);
            _secondGraphPheromonesTable = InitializePheromonesTable(_secondGraph);
            _jumpPheromonesTable = InitializePheromonesTable(_firstGraph, _secondGraph);

            _currentGeneratedSolutions = _solutionGenerator.GenerateRandomSolutions(_firstGraph, _secondGraph, solutionsNumber);
            _currentGeneratedSolutions.Sort(delegate(Solution x, Solution y) {
                return x.SolutionValue.CompareTo(y.SolutionValue);
            });

            List<Solution> solutionsTakenIntoPheromonesTable = _currentGeneratedSolutions.GetRange(0, (int) Math.Ceiling(_currentGeneratedSolutions.Count * solutionsThreshold));
            int i = 1;
            foreach(var solution in solutionsTakenIntoPheromonesTable) {
                FillPheromonesTable(_firstGraph, GetPathsFromOneContext(solution, 0), ref _firstGraphPheromonesTable, i);
                FillPheromonesTable(_secondGraph, GetPathsFromOneContext(solution, 1), ref _secondGraphPheromonesTable, i);
                FillJumpPheromonesTable(solution, ref _jumpPheromonesTable, i);
                i += 1;
            }
        }
    }
}