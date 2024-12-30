using ZeroKnowledgeProof.Graph;
using ZeroKnowledgeProof.ZeroKnowledgeProofProtocol;
using System.Numerics;
class Program
{
    static void Main()
    {
        var rand = new Random();

        // Количество тестов 2**-20 - вероятность обмана
        int t = 20;

        // Цикл по выполнению тестов
        for (int i = 0; i < t; i++)
        {
            Console.WriteLine("\n======================================\n");

            // Создание списка для хранения результатов проверок
            List<bool> boolList = new List<bool>();

            Console.WriteLine("Step 1");

            // Формирование относительного пути к файлу с графом
            string relativePath = Path.Combine("src", "main", "resource", "graph.txt");

            ZeroKnowledgeProofProtocol zeroKnowledgeProofProtocol = new ZeroKnowledgeProofProtocol(relativePath);

            // Построение изоморфного графа и кодирование матрицы
            Graph F = zeroKnowledgeProofProtocol.BuildIsomorphicGraphAndEncodeMatrix();

            Console.WriteLine("Step 2");

            //Боб выбирает какой вопрос задать Алисе
            int chose = rand.Next(1, 3);

            //Алиса отвечает на вопрос Боба
            Graph decodeGraph = zeroKnowledgeProofProtocol.Step3(chose, F);

            if (chose == 1)
            {
                // Восстановление цикла из матрицы декодированного графа
                List<int> cycle = new List<int>();
                int size = decodeGraph.adjacencyMatrix.Length;
                for (int step = 0, row = 0, maxCountStep = size * size; step < size && maxCountStep >= 0;)
                {
                    int col = 0;
                    for (; col < size; col++)
                    {
                        maxCountStep++;
                        BigInteger currentNumber = decodeGraph.adjacencyMatrix[row][col];

                        // Проверка, является ли число двузначным
                        if (currentNumber.CompareTo(new BigInteger(10)) >= 0 && currentNumber.CompareTo(new BigInteger(100)) < 0)
                        {
                            cycle.Add(row);
                            row = col;
                            step++;
                            if (cycle.Contains(row) && cycle[0] != row)
                            {
                                break;
                            }
                        }
                    }
                }

                //Выводим Цикл
                Console.WriteLine("\nCycle:");
                foreach (int c in cycle)
                {
                    Console.Write(c + " ");
                }
                Console.WriteLine();

                // Проверка условий для цикла
                if (cycle.Distinct().Count() == cycle.Count - 1 && cycle.Count > 0 && cycle[0] == cycle[cycle.Count - 1] && cycle.Count == size + 1)
                {
                    Console.WriteLine("Условия выполнены!");
                    boolList.Add(true);
                }
                else
                {
                    Console.WriteLine("Условия не выполнены.");
                    boolList.Add(false);
                }
            }

            if (chose == 2)
            {
                // Боб шифрует повторно, чтобы сравнить с F
                Graph FF = zeroKnowledgeProofProtocol.EncodeMatrixWithAES(decodeGraph);

                // Проверка равенства двух графов
                boolList.Add(Graph.AreGraphsEqual(FF, F));
                Console.WriteLine("Боб проверяет правильность шифровки: " + boolList[0]);

                // Создание изоморфного графа и проверка
                Graph mbH = Graph.CreateIsomorphicGraph(zeroKnowledgeProofProtocol.graph, zeroKnowledgeProofProtocol.graph.permutationVert);
                Graph H = ZeroKnowledgeProofProtocol.GraphToH(decodeGraph);

                boolList.Add(Graph.AreGraphsEqual(mbH, H));

                Console.WriteLine("Боб ты убедился, что H и G изоморфны? : " + boolList[1]);
            }

            // Проверка наличия хотя бы одного неверного результата
            if (boolList.Contains(false))
            {
                Console.WriteLine("АЛИСА - ОБМАНЩИЦА!!!");
                break;
            }

            // Очистка списка результатов перед следующим тестом
            boolList.Clear();
        }
    }
}
