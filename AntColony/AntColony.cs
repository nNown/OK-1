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
        private static Random _random;
        private Solution _mostOptimalSolution;
        public void Optimize(int iterationsUpperBound, int numberOfAnts, double pheromoneUsageChance, double maxPheromoneUsageChance, double maxChanceToPickVertex, double vaporationChance, double vaporationRate, int pheromonesUsageGrowthNumber, double solutionsTakenIntoPheromonesTable) {
            Console.WriteLine("\tIterations:{0}\n\tAnts:{1}\n\tPheromones:{2}:{3}:{4}:{5}\n\tVaporation:{6}:{7}\n\tSolutions:{8}", iterationsUpperBound, numberOfAnts, pheromoneUsageChance, maxPheromoneUsageChance, maxChanceToPickVertex, pheromonesUsageGrowthNumber, vaporationChance, vaporationRate, solutionsTakenIntoPheromonesTable);
            List<int> cutoffs = pheromonesGrowthCutoffs(iterationsUpperBound, pheromonesUsageGrowthNumber);
            double pheromoneUsage = pheromoneUsageChance;
            for(int i = 0; i < iterationsUpperBound; i++) {
                if(cutoffs.Contains(i)) {
                    if(pheromoneUsage < maxPheromoneUsageChance) {
                        pheromoneUsage += pheromoneUsageChance;
                    }
                }

                UpdatePheromones(solutionsTakenIntoPheromonesTable, vaporationChance, vaporationRate);
                GenerateSolutions(numberOfAnts, pheromoneUsage, maxChanceToPickVertex);
                SortSolutions();

                if(_currentGeneratedSolutions[0].SolutionValue < _mostOptimalSolution.SolutionValue) {
                    _mostOptimalSolution = _currentGeneratedSolutions[0];
                    Console.WriteLine("\n\tOptimized solution:{0}\n\tIteration:{1}", _mostOptimalSolution.SolutionValue, i);
                }
            }
        }
        private List<int> pheromonesGrowthCutoffs(int iterations, int numberOfPheromonesUsageGrowths) {
            List<int> cutoffs = new List<int>();
            int lowestGrowth = iterations / numberOfPheromonesUsageGrowths;
            int currentGrowth = lowestGrowth;
            while(currentGrowth < iterations) {
                cutoffs.Add(currentGrowth);
                currentGrowth += lowestGrowth;
            }
            return cutoffs;
        }
        private void GenerateSolutions(int numberOfAnts, double pheromoneUsageChance, double maxChanceToPickVertex) {
            _currentGeneratedSolutions = new List<Solution>();
            for(int i = 0; i < numberOfAnts; i++) {
                _currentGeneratedSolutions.Add(GenerateSolution(pheromoneUsageChance, maxChanceToPickVertex));
                MarkVerticesAsUnvisited(ref _firstGraph);
                MarkVerticesAsUnvisited(ref _secondGraph);
            }
        }
        private Solution GenerateSolution(double pheromoneUsageChance, double maxChanceToPickVertex) {
            Solution solution = new Solution(_firstGraph, _secondGraph);
            
            int i = 0;
            List<Tuple<int, int>> firstGraphUnvisitedVertices = GetUnvisitedVertices(_firstGraph);
            List<Tuple<int, int>> secondGraphUnvisitedVertices = GetUnvisitedVertices(_secondGraph);
            Tuple<int, int> subpathStartingPoint = _mostOptimalSolution.SolutionPath[0][0];
            Graph context = _firstGraph;
            Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, double>> pheromonesContext = _firstGraphPheromonesTable;
            while(firstGraphUnvisitedVertices.Count > 0 && secondGraphUnvisitedVertices.Count > 0) {
                List<Tuple<int, int>> subpath = GeneratePath(ref context, subpathStartingPoint, pheromoneUsageChance, pheromonesContext, maxChanceToPickVertex);
                solution.SolutionPath.Add(subpath);
                solution.SolutionValue += ComputePathValue(context, subpath) + solution.SolutionPath.Count > 1 ? Graph.DistanceBetweenVertices(solution.SolutionPath[solution.SolutionPath.Count - 1][solution.SolutionPath[solution.SolutionPath.Count - 1].Count - 1], subpath[0]) : 0;

                firstGraphUnvisitedVertices = GetUnvisitedVertices(_firstGraph);
                secondGraphUnvisitedVertices = GetUnvisitedVertices(_secondGraph);
                subpathStartingPoint = SubpathStartingPoint(context == _firstGraph ? _secondGraph : _firstGraph, subpath[subpath.Count - 1], pheromoneUsageChance);
                i += 1;
                context = i % 2 == 0 ? _firstGraph : _secondGraph;
                pheromonesContext = i % 2 == 0 ? _firstGraphPheromonesTable : _secondGraphPheromonesTable;
            }
            List<Tuple<int, int>> contextUnvisitedVertices = GetUnvisitedVertices(context);
            List<Tuple<int, int>> finalPath = GeneratePath(ref context, contextUnvisitedVertices);
            
            solution.SolutionPath.Add(finalPath);
            solution.SolutionValue += ComputePathValue(context, finalPath) + Graph.DistanceBetweenVertices(solution.SolutionPath[solution.SolutionPath.Count - 1][solution.SolutionPath[solution.SolutionPath.Count - 1].Count - 1], finalPath[0]);

            return solution;
        }

        private List<Tuple<int, int>> GeneratePath(ref Graph graph, Tuple<int, int> startingPoint, double chanceToUsePheromonesTable, Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, double>> pheromonesTable, double maxChanceToPickVertex) {
            List<Tuple<int, int>> generatedPath = new List<Tuple<int, int>>();
            
            Tuple<int, int> currentVertex = startingPoint;
            while(generatedPath.Count < 10) {
                generatedPath.Add(currentVertex);
                graph.VertexList[currentVertex].Visited = true;

                
                Tuple<int, int> currentEdge = PickRandomVertex(new List<Tuple<int, int>>(graph.VertexList[currentVertex].EdgeList.Keys));
                if(_random.NextDouble() <= chanceToUsePheromonesTable) {
                    currentEdge = GetVertexFromPheromonesTable(currentVertex, pheromonesTable, maxChanceToPickVertex);
                } else {
                    foreach(var edge in graph.VertexList[currentVertex].EdgeList) {
                        if(!graph.VertexList[edge.Key].Visited) {
                            currentVertex = edge.Key;
                            break;
                        }

                        if(generatedPath.Contains(edge.Key)) {
                            continue;
                        }

                        if(edge.Value < graph.VertexList[currentVertex].EdgeList[currentEdge]) {
                            currentEdge = edge.Key;
                        }
                    }
                }

                currentVertex = currentEdge;
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

        public int ComputePathValue(Graph graph, List<Tuple<int, int>> solutionSubpath) {
            int solutionValue = 0;
            for(int i = 0, j = 1; i < solutionSubpath.Count - 1; i++, j++) {
                solutionValue += graph.VertexList[solutionSubpath[i]].EdgeList[solutionSubpath[j]];
            }
            return solutionValue;
        }

        private Tuple<int, int> SubpathStartingPoint(Graph graph, Tuple<int, int> pheromoneVertex, double chanceToUsePheromonesTable) {
            Tuple<int, int> choosenVertex = PickRandomVertex(GetUnvisitedVertices(graph));
            if(_random.NextDouble() <= chanceToUsePheromonesTable) {
                List<Tuple<int, int>> unvisitedVertices = GetUnvisitedVertices(graph, pheromoneVertex, _jumpPheromonesTable);
                choosenVertex = PickRandomVertex(unvisitedVertices);
            }
            return choosenVertex;
        }


        private Tuple<int, int> PickRandomVertex(List<Tuple<int, int>> vertices) {
            return vertices[_random.Next(0, vertices.Count)];
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

        private List<Tuple<int, int>> GetUnvisitedVertices(Graph graph, Tuple<int, int> pheromoneVertex, Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, double>> pheromonesTable) {
            List<Tuple<int, int>> unvisitedVertices = new List<Tuple<int, int>>();

            foreach(var vertex in pheromonesTable[pheromoneVertex]) {
                if(!graph.VertexList[vertex.Key].Visited) {
                    unvisitedVertices.Add(vertex.Key);
                }
            }

            return unvisitedVertices;
        }

        private Tuple<int, int> GetVertexFromPheromonesTable(Tuple<int, int> vertex, Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, double>> pheromonesTable, double maxChanceToPickVertex) {
            SmoothePheromones(ref pheromonesTable, vertex, maxChanceToPickVertex);
            
            List<Tuple<Tuple<int, int>, double>> chancesToPickVertex = CreateListOfChances(pheromonesTable[vertex]);
            List<Tuple<Tuple<int, int>, double>> chancesCDF = CDF(chancesToPickVertex);

            double generatedChance = _random.NextDouble();
            return ChoosePheromone(chancesCDF, generatedChance);
        }
        private double SumPheromonesValues(List<double> pheromonesValues) {
            double pheromonesSum = 0;

            foreach(var pheromoneValue in pheromonesValues) {
                pheromonesSum += pheromoneValue;
            }

            return pheromonesSum;
        }

        private void SmoothePheromones(ref Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, double>> pheromones, Tuple<int, int> vertex, double maxChanceToUsePheromone) {
            List<Tuple<Tuple<int, int>, double>> chancesToPickVertex = CreateListOfChances(pheromones[vertex]);
            foreach(var chance in chancesToPickVertex) {
                if(chance.Item2 > maxChanceToUsePheromone) {
                    double excessPheromone = chance.Item2 * (1 - maxChanceToUsePheromone);
                    pheromones[vertex][chance.Item1] = chance.Item2 * maxChanceToUsePheromone;
                    foreach(var pheromone in chancesToPickVertex) {
                        if(chance.Item1 == pheromone.Item1) continue;
                        pheromones[vertex][pheromone.Item1] += GetChance(chancesToPickVertex, pheromone.Item1) * excessPheromone;
                    }
                    break;
                }
            }
        }

        private double GetChance(List<Tuple<Tuple<int, int>, double>> chances, Tuple<int, int> vertex) {
            double chanceToPickVertex = 0;
            foreach(var chance in chances) {
                if(chance.Item1 == vertex) {
                    chanceToPickVertex = chance.Item2;
                }
            }
            return chanceToPickVertex;
        }

        private List<Tuple<Tuple<int, int>, double>> CreateListOfChances(Dictionary<Tuple<int, int>, double> pheromones) {
            double pheromonesSum = SumPheromonesValues(new List<double>(pheromones.Values));

            List<Tuple<Tuple<int, int>, double>> chancesToPickVertex = new List<Tuple<Tuple<int, int>, double>>();
            foreach(var pheromone in pheromones) {
                chancesToPickVertex.Add(new Tuple<Tuple<int, int>, double>(pheromone.Key, pheromone.Value / pheromonesSum));
            }

            return chancesToPickVertex;
        }

        private List<Tuple<Tuple<int, int>, double>> CDF(List<Tuple<Tuple<int, int>, double>> pheromonesProbabilities) {
            pheromonesProbabilities.Sort(delegate(Tuple<Tuple<int, int>, double> x, Tuple<Tuple<int, int>, double> y) {
                return x.Item2.CompareTo(y.Item2);
            });

            List<Tuple<Tuple<int, int>, double>> pheromonesCDF = new List<Tuple<Tuple<int, int>, double>>();
            pheromonesCDF.Add(pheromonesProbabilities[0]);
            for(int i = 1; i < pheromonesProbabilities.Count; i++) {
                pheromonesCDF.Add(new Tuple<Tuple<int, int>, double>(pheromonesProbabilities[i].Item1, pheromonesProbabilities[i].Item2 + pheromonesProbabilities[i - 1].Item2));
            }

            return pheromonesCDF;
        }

        private Tuple<int, int> ChoosePheromone(List<Tuple<Tuple<int, int>, double>> pheromonesCDF, double generatedChance) {
            Tuple<int, int> choosenPheromone = pheromonesCDF[pheromonesCDF.Count - 1].Item1;
            foreach(var pheromone in pheromonesCDF) {
                if(generatedChance < pheromone.Item2) {
                    choosenPheromone = pheromone.Item1;
                    break;
                }
            }
            return choosenPheromone;
        }
        private void UpdatePheromones(double solutionsThreshold, double vaporationChance, double vaporationRate) {
            if(_random.NextDouble() <= vaporationChance) {
                VaporizePheromones(ref _firstGraphPheromonesTable, 0.05);
                VaporizePheromones(ref _secondGraphPheromonesTable, 0.05);
                VaporizePheromones(ref _jumpPheromonesTable, 0.05);
            }
            AddPheromones(solutionsThreshold);
        }
        private List<Solution> GetSolutionsForPheromonesUpdate(double solutionsThreshold) {
            SortSolutions();

            List<Solution> solutionsTakenIntoPheromonesTable = _currentGeneratedSolutions.GetRange(0, (int) Math.Ceiling(_currentGeneratedSolutions.Count * solutionsThreshold));
            return solutionsTakenIntoPheromonesTable;
        }
        private void SortSolutions() {
            _currentGeneratedSolutions.Sort(delegate(Solution x, Solution y) {
                return x.SolutionValue.CompareTo(y.SolutionValue);
            });
        }
        private void AddPheromones(double solutionsThreshold) {
            int i = 1;
            foreach(var solution in GetSolutionsForPheromonesUpdate(solutionsThreshold)) {
                FillPheromonesTable(_firstGraph, GetPathsFromOneContext(solution, 0), ref _firstGraphPheromonesTable, i);
                FillPheromonesTable(_secondGraph, GetPathsFromOneContext(solution, 1), ref _secondGraphPheromonesTable, i);
                FillJumpPheromonesTable(solution, ref _jumpPheromonesTable, i);
                i += 1;
            }
        }
        private void VaporizePheromones(ref Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, double>> pheromonesTable, double vaporationRate) {
            foreach(var vertex in pheromonesTable) {
                List<Tuple<int, int>> pheromoneVertices = new List<Tuple<int, int>>(pheromonesTable[vertex.Key].Keys);
                foreach(var pheromone in pheromoneVertices) {
                    pheromonesTable[vertex.Key][pheromone] *= (1.0 - vaporationRate);
                }
            }
        }
        private void AddPheromonesToGraphTables(Tuple<int, int> source, Tuple<int, int> destination, int weight, ref Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, double>> pheromonesTable) {
            pheromonesTable[source][destination] += 1.0 / weight;
        }
        private void AddPheromonesToJumpTable(Tuple<int, int> source, Tuple<int, int> destination, ref Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, double>> pheromonesTable, int weightMultiplier) {
            _jumpPheromonesTable[source][destination] += 1.0 / (Graph.DistanceBetweenVertices(source, destination) * weightMultiplier);
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

            foreach(var secondGraphVertex in secondGraph.VertexList) {
                pheromonesTable.Add(secondGraphVertex.Key, new Dictionary<Tuple<int, int>, double>());
                foreach(var firstGraphVertex in firstGraph.VertexList) {
                    pheromonesTable[secondGraphVertex.Key].Add(firstGraphVertex.Key, 0);
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
                    AddPheromonesToGraphTables(path[i], path[j], graph.VertexList[path[i]].EdgeList[path[j]] * weightMultiplier, ref pheromonesTable);
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
        private void MarkVerticesAsUnvisited(ref Graph graph) {
            foreach(var vertex in graph.VertexList) {
                vertex.Value.Visited = false;
            }
        }
        public AntColony(int graphSize, int maxWeight, int solutionsNumber, double solutionsThreshold) {
            _graphGenerator = GraphGenerator.Instance;
            _solutionGenerator = InstanceGenerator.Instance;
            _random = new Random();

            _firstGraph = _graphGenerator.GenerateGraph(graphSize, maxWeight);
            _secondGraph = _graphGenerator.GenerateGraph(graphSize, maxWeight);

            _firstGraphPheromonesTable = InitializePheromonesTable(_firstGraph);
            _secondGraphPheromonesTable = InitializePheromonesTable(_secondGraph);
            _jumpPheromonesTable = InitializePheromonesTable(_firstGraph, _secondGraph);

            _currentGeneratedSolutions = _solutionGenerator.GenerateRandomSolutions(_firstGraph, _secondGraph, solutionsNumber);
            SortSolutions();
            _mostOptimalSolution = _currentGeneratedSolutions[0];
            AddPheromones(solutionsThreshold);

            Console.WriteLine("Generated Graph:\n\tSize:{0}:{1}\n\tSolutions:{2}\n\tThreshold:{3}\n\tMost optimal from random solutions generator:{4}\n", _firstGraph.VertexList.Count, _secondGraph.VertexList.Count, solutionsNumber, solutionsThreshold, _mostOptimalSolution.SolutionValue);
        }
    }
}