namespace ZeroKnowledgeProof.GraphReader;

using System;
using System.IO;

using ZeroKnowledgeProof.Graph;

public class GraphReader
{
    // Метод для чтения графа из файла
    public static Graph ReadGraphFromFile(string filePath)
    {
        // Проверяем, существует ли файл
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("\nФайл не существует", filePath);
        }

        try
        {
            // Читаем все строки из файла
            string[] lines = File.ReadAllLines(filePath);

            // Проверяем, что файл не пуст
            if (lines.Length < 1)
            {
                throw new Exception("\nФайл пуст");
            }

            // Разбираем заголовок файла
            string[] header = lines[0].Split(' ');

            // Проверяем корректность формата заголовка
            if (header.Length != 2)
            {
                throw new Exception("\nНеверный формат заголовка файла");
            }

            // Извлекаем количество вершин (n) и рёбер (m)
            int n = int.Parse(header[0]);
            int m = int.Parse(header[1]);

            // Проверяем, что значения n и m в разумных пределах
            if (n > 1000 || m > n * n)
            {
                throw new Exception("\nНеверные значения n или m");
            }

            // Создаем объект графа
            Graph graph = new Graph(n);

            // Заполняем граф рёбрами
            for (int i = 1; i <= m; i++)
            {
                // Разбираем информацию о ребре
                string[] edgeInfo = lines[i].Split(' ');

                // Проверяем корректность формата строки с информацией о ребре
                if (edgeInfo.Length != 2)
                {
                    throw new Exception("\nНеверный формат строки с информацией о ребре");
                }

                // Извлекаем номера вершин ребра и добавляем ребро в граф
                int vertex1 = int.Parse(edgeInfo[0]) - 1;
                int vertex2 = int.Parse(edgeInfo[1]) - 1;

                graph.AddEdge(vertex1, vertex2);
            }

            // Разбираем информацию о цикле
            string[] cycleInfo = lines[m + 1].Split(' ');

            // Проверяем корректность формата цикла
            if (cycleInfo.Length != n + 1)
            {
                throw new Exception("\nНеверный формат цикла");
            }

            // Заполняем список гамильтонова цикла
            for (int i = 0; i < cycleInfo.Length; i++)
            {
                graph.gamiltonCycle.Add(int.Parse(cycleInfo[i]) - 1);
            }

            // Возвращаем граф
            return graph;
        }
        catch (Exception ex)
        {
            // Если произошла ошибка, добавляем информацию об ошибке и передаем её дальше
            throw new Exception("\nОшибка при чтении файла: " + ex.Message);
        }
    }
}

