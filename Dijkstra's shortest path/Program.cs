using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Dijkstra_s_shortest_path
{
    class Program
    {   /// <summary>
        /// Map accessible by all methods in Program class
        /// </summary>
        static int[,] map;
        /// <summary>
        /// Main method initializing sequence of methods to resolve problem
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            List<string[]> records = new List<string[]>();
            List<string[]> question = new List<string[]>();
            string file = "map.txt";
            records = ReadFile(file, out question);
            map = GenerateMap(records);
            PrintMap(map);
            FindAllPaths(ref map, question);


            Console.ReadLine();
        }
        /// <summary>
        /// Method for running path calculation for each pair in input file
        /// </summary>
        /// <param name="map">Map of graph</param>
        /// <param name="question">List of pairs of nodes to find distance between</param>
        private static void FindAllPaths(ref int[,] map, List<string[]> question)
        {
            GraphPath path;
            foreach (string[] startEnd in question)
            {
                path = new GraphPath();
                FindPath(startEnd);
            }
        }
        /// <summary>
        /// Main part of Dijkstra's algorythm
        /// </summary>
        /// <param name="startEnd">Star and end nodes</param>
        private static void FindPath(string[] startEnd)
        {
            char startNode = startEnd[0][0];
            char endNode = startEnd[1][0];
            int row;
            int size = map.GetLength(0);
            PriorityQueue queue = new PriorityQueue();
            queue.Enqueue(new Tuple<char, int, char>(startNode, 0, startNode));
            Dictionary<char, int[]> visited = new Dictionary<char, int[]>();

            while (queue.Count > 0)
            {
                Tuple<char, int, char> currentNode = queue.Dequeue();
                char previousNode = currentNode.Item3;
                row = currentNode.Item1 - 65;
                int currentDistance = currentNode.Item2;
                if (!visited.ContainsKey(currentNode.Item1))
                {
                    visited.Add(currentNode.Item1, new int[] { currentDistance, previousNode }); 
                }
                else if (visited.ContainsKey(currentNode.Item1) && visited[currentNode.Item1][0] > currentNode.Item2)
                {
                    visited[currentNode.Item1][0] = currentNode.Item2;
                    visited[currentNode.Item1][1] = previousNode;
                }
                for (int i = 0; i < size; i++)
                {
                    if (map[row, i] != 0 && !visited.ContainsKey((char)(i+65)))
                    {
                        queue.Enqueue(new Tuple<char, int, char>((char)(i+65),map[row,i]+currentDistance, currentNode.Item1));
                    }
                } 
            }

            if (visited.ContainsKey(endNode))
            {
                GraphPath result = RebuildPath(visited, startEnd);
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine($"No path found between {startNode} and {endNode}!");
            }
        }
        /// <summary>
        /// Method for finding out final path based on list of distances from begining
        /// </summary>
        /// <param name="visited">List of nodes accessible from start node and distances from it with previous nodes for each other</param>
        /// <param name="startEnd">Path start and end</param>
        /// <returns></returns>
        private static GraphPath RebuildPath(Dictionary<char, int[]> visited, string[] startEnd)
        {
            char startNode = startEnd[0][0];
            char endNode = startEnd[1][0];
            char currentNode = endNode;
            GraphPath result = new GraphPath();
            result.Length = visited[endNode][0];
            result.Nodes.Add(currentNode.ToString());

            while (currentNode != startNode)
            {
                currentNode = (char)visited[currentNode][1];
                result.Nodes.Add(currentNode.ToString());
            }

            return result;
        }
        /// <summary>
        /// Method for printing map to console
        /// </summary>
        /// <param name="map">Map of graph</param>
        static void PrintMap(int[,] map)
        {
            int size = map.GetLength(0);
            Console.Write("    ");
            for (int i = 0; i < size; i++)
            {
                Console.Write((char)(i + 65) + "  ");
            }
            Console.WriteLine();
            for (int i = 0; i < size; i++)
            {
                Console.Write((char)(i + 65) + " ");
                for (int j = 0; j < size; j++)
                {
                    Console.Write("{0,3}", map[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        /// <summary>
        /// Method generating map based on nodes' distances
        /// </summary>
        /// <param name="records">List of nodes' distances</param>
        /// <returns>Graph map</returns>
        private static int[,] GenerateMap(List<string[]> records)
        {
            records.Sort((x, y) => x[0].CompareTo(y[0]));
            int size1 = records[records.Count() - 1][0][0];
            records.Sort((x, y) => x[1].CompareTo(y[1]));
            int size2 = records[records.Count() - 1][1][0];
            int size;
            if (size1 >= size2)
            {
                size = size1;
            }
            else
            {
                size = size2;
            }
            size -= 64;
            int[,] map = new int[size, size];

            foreach (string[] record in records)
            {
                int row = record[0][0] - 65;
                int column = record[1][0] - 65;
                map[row, column] = int.Parse(record[2]);
                map[column, row] = int.Parse(record[2]);
            }

            return map;
        }
        /// <summary>
        /// Method reading file with graph and question data and extracting from it node distances and list of nodes pairs to finds distances between
        /// </summary>
        /// <param name="fileName">Path to file with data</param>
        /// <param name="question">List of pairs of nodes to calculate distance between</param>
        /// <returns>List of distances between nodes</returns>
        static List<string[]> ReadFile(string fileName, out List<string[]> question)
        {
            List<string[]> records = new List<string[]>();
            question = new List<string[]>();
            StreamReader sr = new StreamReader(fileName);
            string line;
            using (sr)
            {
                line = sr.ReadLine();
                while (line != "")
                {
                    records.Add(line.Split(' '));
                    line = sr.ReadLine();
                }
                line = sr.ReadLine();
                while (line != null)
                {
                    question.Add(line.Split(' '));
                    line = sr.ReadLine();
                }
            }
            return records;
        }
        /// <summary>
        /// Priority queue implenetation for keeping current node, it's distance from begining and previous node
        /// </summary>
        class PriorityQueue
        {
            /// <summary>
            /// Keeps all data
            /// </summary>
            private SortedDictionary<double, char[]> list;
            /// <summary>
            /// Constructor initializing dictionary
            /// </summary>
            public PriorityQueue()
            {
                list = new SortedDictionary<double, char[]>();
            }
            /// <summary>
            /// Count of elements
            /// </summary>
            public int Count
            {
                get { return list.Count; }
            }
            /// <summary>
            /// Method for enqueueing node
            /// </summary>
            /// <param name="node">Node info - current node, distance to begining and previous node for resolving path</param>
            public void Enqueue(Tuple<char, int, char> node)
            {
                double key = node.Item2;
                while (true)
                {
                    char[] value = { node.Item1, node.Item3 };
                    if (!list.ContainsKey(key))
                    {
                        list.Add(key,value);
                        break;
                    }
                    else
                    {
                        key += 0.01d;
                    }
                }
            }
            /// <summary>
            /// Dequeueing node
            /// </summary>
            /// <returns>Node info - current node, distance to begining and previous node for resolving path</returns>
            public Tuple<char,int, char> Dequeue()
            {
                if (Count > 0)
                {
                    Tuple<char, int, char> result = new Tuple<char, int, char>(list.First().Value[0], (int)list.First().Key, list.First().Value[1]);
                    list.Remove(list.First().Key);
                    return result;
                }
                else
                {
                    throw new Exception("no more items");
                }
            }
            /// <summary>
            /// Allows to peek first node
            /// </summary>
            /// <returns>Node info - current node, distance to begining and previous node for resolving path</returns>
            public Tuple<char, int, char> Peek()
            {
                if (Count > 0)
                {
                    return new Tuple<char, int, char>(list.First().Value[0], (int)list.First().Key, list.First().Value[1]);
                }
                else
                {
                    throw new Exception("no items");
                }
            }
        }
    }
    /// <summary>
    /// Class for keeping final path - length and list of nodes
    /// </summary>
    internal class GraphPath
    {
        public int Length { get; set; }
        public List<string> Nodes { get; set; }
        public GraphPath()
        {
            Nodes = new List<string>();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Path is: ");
            foreach (string node in Nodes)
            {
                sb.Append($"{node} ");
            }
            sb.Append($"and it's length is: {Length}");
            return sb.ToString();
        }
    }
}
