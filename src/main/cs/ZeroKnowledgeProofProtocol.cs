namespace ZeroKnowledgeProof.ZeroKnowledgeProofProtocol;

using ZeroKnowledgeProof.Graph;
using ZeroKnowledgeProof.GraphReader;

using System.Security.Cryptography;

using System.Numerics;

public class ZeroKnowledgeProofProtocol
{

    public Graph graph { get; private set; }

    private AesCryptoServiceProvider aes;

    public ZeroKnowledgeProofProtocol(string filePath)
    {
        // Чтение графа из файла
        graph = GraphReader.ReadGraphFromFile(filePath);

        // Генерация ключа и вектора инициализации для AES
        aes = new AesCryptoServiceProvider();
        aes.GenerateKey();
        aes.GenerateIV();
    }

    public Graph BuildIsomorphicGraphAndEncodeMatrix()
    {
        // Вывод исходного графа и его гамильтонова цикла
        Console.WriteLine("Изначальный граф:");
        graph.PrintGraph();
        Console.WriteLine();

        Console.WriteLine("Гамильтонов цикл:");
        foreach (int e in graph.gamiltonCycle)
        {
            Console.Write(e + " ");
        }
        Console.WriteLine("\n");

        //получаем изоморфный граф
        int size = graph.adjacencyMatrix.Length;
        graph.permutationVert = Graph.GenerateRandomPermutation(size);
        Graph.PrintPermutation(graph.permutationVert);

        Graph isomorphicGraph = Graph.CreateIsomorphicGraph(graph, graph.permutationVert);

        Console.WriteLine("Получаем изоморфный граф:");
        isomorphicGraph.PrintGraph();
        Console.WriteLine();

        //кодируем матрицу
        Graph encryptedGraph = Graph.EncodeMatrixWithRandomValues(isomorphicGraph);
        Console.WriteLine("Кодируем матрицу:");
        encryptedGraph.PrintGraph();
        Console.WriteLine();

        // Шифрование элементов матрицы
        encryptedGraph = EncodeMatrixWithAES(encryptedGraph);

        Console.WriteLine("Зашифрованная матрица: ");
        encryptedGraph.PrintGraph();
        Console.WriteLine();

        return encryptedGraph;
    }


    public Graph EncodeMatrixWithAES(Graph originalGraph)
    {
        int size = originalGraph.adjacencyMatrix.Length;
        Graph encryptedGraph = new Graph(size);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                // Преобразование BigInteger в массив байтов и шифрование с использованием RSA
                byte[] originalData = originalGraph.adjacencyMatrix[i][j].ToByteArray();
                byte[] encryptedData = EncryptWithAES(originalData);

                // Сохранение зашифрованных данных в матрице
                encryptedGraph.adjacencyMatrix[i][j] = new BigInteger(encryptedData);
            }
        }

        return encryptedGraph;
    }

    public Graph Step3(int chose, Graph F)
    {
        Graph retGraph = null;
        switch (chose)
        {
            case 1:
                Console.WriteLine("1. Каков гамильтонов цикл для графа H ?");
                Console.WriteLine("Step 3: Дешифрованный Цикл:");
                retGraph = WhatHamiltonianCycle(F);
                retGraph.PrintGraph();
                break;
            case 2:
                Console.WriteLine("2. Действительно ли граф H изоморфен G ?");
                Console.WriteLine("Step 3: Дешифрованный Граф:");
                retGraph = decodeGraph(F);
                retGraph.PrintGraph();
                Graph.PrintPermutation(graph.permutationVert);
                break;
            default:
                Console.WriteLine("Нет такого вопроса! Выберитое число от 1 до 2");
                break;
        }
        return retGraph;

    }

    public Graph WhatHamiltonianCycle(Graph F)
    {
        int size = F.adjacencyMatrix.Length;
        Graph decryptedGraph = (Graph)F.Clone();

        //Заменяем Элементы цикла на перестановку
        List<int> newGamiltonCycle = new List<int>(graph.gamiltonCycle);
        for (int i = 0; i < newGamiltonCycle.Count; i++)
        {
            newGamiltonCycle[i] = graph.permutationVert[newGamiltonCycle[i]].Item2;
        }

        for (int i = 0; i < newGamiltonCycle.Count - 1; i++)
        {
            int fromVertex = newGamiltonCycle[i];
            int toVertex = newGamiltonCycle[i + 1];

            // Дешифруем
            byte[] encryptedData = F.adjacencyMatrix[fromVertex][toVertex].ToByteArray();
            byte[] decryptedData = DecryptWithAES(encryptedData);

            // сохраняем дешифровонное значение 
            decryptedGraph.adjacencyMatrix[fromVertex][toVertex] = new BigInteger(decryptedData);
        }
        return decryptedGraph;
    }

    public Graph decodeGraph(Graph g)
    {
        int size = g.adjacencyMatrix.Length;
        Graph decryptedGraph = (Graph)g.Clone();

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                byte[] encryptedData = g.adjacencyMatrix[i][j].ToByteArray();
                byte[] decryptedData = DecryptWithAES(encryptedData);

                BigInteger originalValue = new BigInteger(decryptedData);

                decryptedGraph.adjacencyMatrix[i][j] = originalValue;
            }
        }
        return decryptedGraph;
    }

    public byte[] EncryptWithAES(byte[] data)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
            }
            return ms.ToArray();
        }
    }

    public byte[] DecryptWithAES(byte[] data)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
            }
            return ms.ToArray();
        }
    }

    public static BigInteger DeleteFirstElement(BigInteger value)
    {
        // Преобразование в строку, удаление старшего символа и обратное преобразование в BigInteger
        string stringValue = value.ToString();
        if (stringValue.Length > 1)
        {
            stringValue = stringValue.Substring(1);
            value = BigInteger.Parse(stringValue);
        }
        else
        {
            // Если число состоит из одной цифры, то просто устанавливаем в 0
            value = 0;
        }
        return value;
    }

    public static Graph GraphToH(Graph HH)
    {
        int size = HH.adjacencyMatrix.Length;
        Graph H = (Graph)HH.Clone();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                H.adjacencyMatrix[i][j] = DeleteFirstElement(H.adjacencyMatrix[i][j]);
            }
        }
        return H;
    }
}
