using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipGame
{
    class FileOperations
    {
        // Функция для записи массива в файл
        public static void WriteArrayToFile(int[,] array, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int y = 0; y < array.GetLength(0); y++)
                {
                    for (int x = 0; x < array.GetLength(1); x++)
                    {
                        writer.Write(((array[y, x] < 0) ? (array[y, x] == -1 ? 1 : 0) : array[y, x])); // Записываем без разделителей
                    }
                    writer.WriteLine(); // Переход на новую строку
                }
            }
        }

        // Функция для чтения массива из файла

        public static int[,] NewReadArrayFromFile(string filePath)
        {
             // Укажите путь к вашему файлу

            try
            {
                // Читаем все строки из файла
                string[] lines = File.ReadAllLines(filePath);

                if (lines.Length == 0)
                {
                    Console.WriteLine("Файл пуст.");
                    return null;
                }

                // Определяем количество символов в первой строке
                int firstLineLength = lines[0].Length;

                // Проверяем, что файл содержит достаточно строк для квадратного массива
                if (lines.Length < firstLineLength)
                {
                    Console.WriteLine("Файл не содержит достаточно строк для создания квадратного массива.");
                    return null;
                }

                // Создаем квадратный массив размера N x N
                int size = firstLineLength;
                int[,] grid = new int[size, size];

                // Заполняем массив числами из файла
                for (int row = 0; row < size; row++)
                {
                    for (int col = 0; col < size; col++)
                    {
                        // Проверяем, что символ в диапазоне {-1, 0, 1}
                        char currentChar = lines[row][col];

                        if (currentChar == '1')
                            grid[row, col] = 1;
                        else if (currentChar == '0')
                            grid[row, col] = 0;
                        else if (currentChar == '-')
                        {
                            if (col + 1 < lines[row].Length && lines[row][col + 1] == '1') // Проверяем следующий символ
                            {
                                grid[row, col] = -1;
                                col++; // Пропускаем следующий символ, так как это часть "-1"
                            }
                            else
                            {
                                throw new FormatException("Недопустимый формат: символ '-' должен быть частью числа '-1'.");
                            }
                        }
                        else
                        {
                            throw new FormatException("Недопустимый символ в файле. Разрешены только {-1, 0, 1}.");
                        }
                    }
                }

                // Выводим результат на экран для проверки
                Console.WriteLine("Содержимое квадратного массива:");
                for (int row = 0; row < size; row++)
                {
                    for (int col = 0; col < size; col++)
                    {
                        Console.Write(grid[row, col] + " ");
                    }
                    Console.WriteLine();
                }
                return grid;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
                return null;
            }
        }
        public static int[,] ReadArrayFromFile(string filePath, int x_dim, int y_dim)
        {
            int[,] array = new int[y_dim, x_dim];

            using (StreamReader reader = new StreamReader(filePath))
            {
                int y = 0;
                string line;
                while ((line = reader.ReadLine()) != null && y < y_dim)
                {
                    for (int x = 0; x < line.Length && x < x_dim; x++)
                    {
                        array[y, x] = line[x] - '0'; // Преобразуем символы в числа
                    }
                    y++;
                }
            }

            return array;
        }

        // Метод для отображения массива (для проверки)
        public static void PrintBoard(int[,] array)
        {
            for (int y = 0; y < array.GetLength(0); y++)
            {
                for (int x = 0; x < array.GetLength(1); x++)
                {
                    Console.Write(array[y, x] + " ");
                }
                Console.WriteLine();
            }
        }

    }
}
