using System;
using System.Collections.Generic;

namespace ok_project {
    public class Solution {
        private readonly Graph _firstGraph;
        private readonly Graph _secondGraph;
        private List<List<Tuple<int, int>>> _solutionPath;
        private int? _solutionValue;
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
        public int? SolutionValue {
            get {
                _solutionValue ??= ComputeSolutionValue();
                return _solutionValue;
            }
        }
        private int ComputeSolutionValue() {
            int solutionValue = 0;
            Graph context = _firstGraph;
            for(int i = 0; i < _solutionPath.Count; i++) {
                if(i % 2 == 0) {
                    context = _firstGraph;    
                } else {
                    context = _secondGraph;
                }

                for(int j = 0; j < _solutionPath[i].Count - 1; j++) {
                    for(int k = 1; k < _solutionPath[i].Count; k++) {
                        solutionValue += context.VertexList[_solutionPath[i][j]].EdgeList[_solutionPath[i][k]];
                    }
                }
            }
            return solutionValue;
        }
        public Solution(Graph firstGraph, Graph secondGraph) {
            _firstGraph = firstGraph;
            _secondGraph = secondGraph;
            _solutionPath = new List<List<Tuple<int, int>>>();
        }
    }
}