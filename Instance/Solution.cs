using System;
using System.Collections.Generic;

namespace ok_project {
    public class Solution {
        private readonly Graph _firstGraph;
        private readonly Graph _secondGraph;
        private List<List<Tuple<int, int>>> _solutionPath;
        private int _solutionValue;
        public Graph FirstGraph {
            get => _firstGraph;
        }
        public Graph SecondGraph {
            get => _secondGraph;
        }
        public List<List<Tuple<int, int>>> SolutionPath {
            get => _solutionPath;
            set => _solutionPath = value;
        }
        public int SolutionValue {
            get => _solutionValue;
            set => _solutionValue = value;
        }
        public Solution(Graph firstGraph, Graph secondGraph) {
            _firstGraph = firstGraph;
            _secondGraph = secondGraph;
            _solutionPath = new List<List<Tuple<int, int>>>();
        }
    }
}