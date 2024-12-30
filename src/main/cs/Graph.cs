namespace ZeroKnowledgeProof.Graph;

using System;
using System.Collections.Generic;
using System.Numerics;

public class Graph : ICloneable
{
    public BigInteger[][] adjacencyMatrix { get; set; } = null;
    public List<int> gamiltonCycle = new List<int>();
    public List<Tuple<int, int>> permutationVert { get; set; }
    public int numVertices { get; } = 0;

    public Graph(int numVertices)
    {
        permutationVert = new List<Tuple<int, int>>();
        this.numVertices = numVertices;
        adjacencyMatrix = new BigInteger[numVertices][];
        for (int i = 0; i < numVertices; i++)
        {
            adjacencyMatrix[i] = new BigInteger[numVertices];
        }
    }

    public void AddEdge(int fromVertex, int toVertex)
    {
        adjacencyMatrix[fromVertex][toVertex] = 1;
    }

    public void PrintGraph()
    {
        const int columnWidth = 12; // Ширина каждой колонки

        for (int i = 0; i < adjacencyMatrix.Length; i++)
        {
            for (int j = 0; j < adjacencyMatrix[i].Length; j++)
            {
                string stringValue = adjacencyMatrix[i][j].ToString();

                // Выводим первые две цифры и три последние цифры
                if (stringValue.Length > 6)
                {
                    string firstDigit = stringValue.Substring(0, 2);
                    string lastThreeDigits = stringValue.Substring(stringValue.Length - 3);

                    Console.Write($"{firstDigit}...{lastThreeDigits,-7} ");
                }
                else
                {
                    Console.Write($"{stringValue,-columnWidth} ");
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    public object Clone()
    {
        int size = adjacencyMatrix.Length;
        Graph clonedGraph = new Graph(size);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                clonedGraph.adjacencyMatrix[i][j] = adjacencyMatrix[i][j];
            }
        }
        return clonedGraph;
    }

    //Выводим список перестановок
    public static void PrintPermutation(List<Tuple<int, int>> permutationVert)
    {
        Console.WriteLine("Случайный список перестановки вершин:");
        foreach (var pair in permutationVert)
        {
            Console.WriteLine($"Вершина {pair.Item1} => Вершина {pair.Item2}");
        }
        Console.WriteLine();
    }

    //Используя перестановки создаем изоморфный граф
    public static Graph CreateIsomorphicGraph(Graph graph, List<Tuple<int, int>> permutation)
    {
        int size = graph.adjacencyMatrix.Length;
        Graph isomorphicGraph = new Graph(size);

        // Перестановка вершин
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                isomorphicGraph.adjacencyMatrix[permutation[i].Item2][permutation[j].Item2] = graph.adjacencyMatrix[permutation[i].Item1][permutation[j].Item1];
            }
        }

        return isomorphicGraph;
    }

    //Генерируем List перестановок
    public static List<Tuple<int, int>> GenerateRandomPermutation(int numVertices)
    {
        Random random = new Random();
        List<int> vertices = new List<int>();
        for (int i = 0; i < numVertices; i++)
        {
            vertices.Add(i);
        }

        // Перемешать вершины случайным образом
        for (int i = vertices.Count - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            int temp = vertices[i];
            vertices[i] = vertices[j];
            vertices[j] = temp;
        }

        // Создать список перестановки вершин
        List<Tuple<int, int>> permutation = new List<Tuple<int, int>>();
        for (int i = 0; i < numVertices; i++)
        {
            permutation.Add(new Tuple<int, int>(i, vertices[i]));
        }

        return permutation;
    }

    //Кодируем значения матрицы смежности
    public static Graph EncodeMatrixWithRandomValues(Graph graph)
    {
        Random random = new Random();
        int size = graph.adjacencyMatrix.Length;
        Graph returned = new Graph(size);
        int numberOfDigits = size.ToString().Length;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int randomValue = random.Next(1, size + 1);
                int pow = (int)Math.Pow(10, numberOfDigits);

                //конкатенация двух чисел
                returned.adjacencyMatrix[i][j] = randomValue * pow + graph.adjacencyMatrix[i][j];
            }
        }
        return returned;
    }

    public static bool AreGraphsEqual(Graph first, Graph second)
    {
        if (first == null || second == null)
        {
            Console.WriteLine("Ошибка при сравнении двух графов: first == null || second == null");
            return false; // Если хотя бы один из графов null, они не равны
        }

        if (first.numVertices != second.numVertices)
        {
            Console.WriteLine("Ошибка при сравнении двух графов: first.numVertices != second.numVertices");
            return false; // Если количество вершин разное, графы не равны
        }

        // Сравниваем матрицы смежности
        for (int i = 0; i < first.numVertices; i++)
        {
            for (int j = 0; j < first.numVertices; j++)
            {
                if (first.adjacencyMatrix[i][j] != second.adjacencyMatrix[i][j])
                {
                    return false; // Если хотя бы одно значение в матрице различно, графы не равны
                }
            }
        }

        return true; // Если все проверки пройдены, графы равны
    }
}


