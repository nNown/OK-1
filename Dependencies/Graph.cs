using System;
using System.Collections.Generic;

namespace ok_project {
    public class Graph {
        Dictionary<Tuple<long, long>, Dictionary<Tuple<long, long>, Tuple<long, bool>>> _edgeList;
        public Dictionary<Tuple<long, long>, Dictionary<Tuple<long, long>, Tuple<long, bool>>> EdgeList {
            get => _edgeList;
            private set => _edgeList = value;
        }

        public Graph(List<Tuple<long, long>> vertices) {
            this._edgeList = new Dictionary<Tuple<long, long>, Dictionary<Tuple<long, long>, Tuple<long, bool>>>();
            foreach(Tuple<long, long> coords in vertices) {
                this._edgeList.Add(coords, new Dictionary<Tuple<long, long>, Tuple<long, bool>>());
                foreach(Tuple<long, long> edge in vertices) {
                    if(coords != edge) {
                        this._edgeList[coords].Add(edge, new Tuple<long, bool>(0, false));
                    } 
                }
            }
        }
    }
}