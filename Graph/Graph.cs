using System;
using System.Collections.Generic;

namespace ok_project {
    public class Vertex {
        private bool _visited;
        private readonly int _degree;
        private Dictionary<Tuple<int, int>, int> _edgeList;

        public bool Visited {
            get => _visited;
            set => _visited = value;
        }
        public int Degree {
            get => _degree;
        }
        public Dictionary<Tuple<int, int>, int> EdgeList {
            get => _edgeList;
            set => _edgeList = value;
        }
        public Vertex() {
            _visited = false;
            _degree = new Random().Next(1, 7);
            _edgeList = new Dictionary<Tuple<int, int>, int>();
        }

        public Vertex(int degree) {
            _visited = false;
            _degree = degree;
            _edgeList = new Dictionary<Tuple<int, int>, int>();
        }
    }
    public class Graph {
        private Dictionary<Tuple<int, int>, Vertex> _vertexList;
        public Dictionary<Tuple<int, int>, Vertex> VertexList {
            get => _vertexList;
            set => _vertexList = value;
        }
        public int DistanceBetweenVertices(Tuple<int, int> source, Tuple<int, int> destination) {
            return (int) Math.Floor(Math.Sqrt(Math.Pow(source.Item1 - destination.Item1, 2) + Math.Pow(source.Item2 - destination.Item2, 2)));
        }

        public void AddVertex(Tuple<int, int> vertex) {
            _vertexList.Add(vertex, new Vertex());
        }
        public void AddVertex(Tuple<int, int> vertex, int degree) {
            _vertexList.Add(vertex, new Vertex(degree));
        }
        public void RemoveVertex(Tuple<int, int> vertex) {
            _vertexList.Remove(vertex);
        }
        public bool HasVertex(Tuple<int, int> vertex) {
            return _vertexList.ContainsKey(vertex);
        }

        public void AddEdge(Tuple<int, int> source, Tuple<int, int> destination) {
            int weight = DistanceBetweenVertices(source, destination);
            
            _vertexList[source].EdgeList.Add(destination, weight);
            _vertexList[destination].EdgeList.Add(source, weight);
        }
        public void RemoveEdge(Tuple<int, int> source, Tuple<int, int> destination) {
            _vertexList[source].EdgeList.Remove(destination);
        }
        public bool HasEdge(Tuple<int, int> source, Tuple<int, int> destination) {
            return _vertexList[source].EdgeList.ContainsKey(destination);
        }
        
        public Graph() {
            _vertexList = new Dictionary<Tuple<int, int>, Vertex>();
        }
    }
}