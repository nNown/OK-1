using System;
using System.Collections.Generic;

namespace ok_project {
    public class Instance {
        private readonly Graph _fistGraph;
        private readonly Graph _secondGraph;
        private List<Tuple<int, int>> _solution;

        public Graph FirstGraph {
            get => _fistGraph;
        }
        public Graph SecondGraph {
            get => _secondGraph;
        }
        public List<Tuple<int, int>> Solution {
            get => _solution;
            set => _solution = value;
        }

        public Instance(Graph firstGraph, Graph secondGraph) {
            _fistGraph = firstGraph;
            _secondGraph = secondGraph;
            _solution = new List<Tuple<int, int>>();
        }
    }
}