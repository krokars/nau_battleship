public class BattleshipValidator
{
    // Размер игрового поля
    private const int GridSize = 10;

    public static bool ValidateBattlefield(int[,] grid)
    {
        int[] shipCounts = new int[5]; // Индекс 1 - однопалубные, 2 - двухпалубные и т.д.

        // Массив для отслеживания посещённых ячеек, чтобы не учитывать один и тот же корабль дважды
        bool[,] visited = new bool[GridSize, GridSize];

        // Проходим по всему полю
        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                // Если найден корабль и эта клетка ещё не посещена
                if (grid[row, col] == 1 && !visited[row, col])
                {
                    // Проверяем размер корабля и его корректное расположение
                    int shipSize = GetShipSize(grid, visited, row, col);

                    if (shipSize < 1 || shipSize > 4)
                    {
                        return false; // Если корабль неправильного размера
                    }

                    // Увеличиваем количество кораблей соответствующего размера
                    shipCounts[shipSize]++;
                }
            }
        }

        // Проверяем, что количество кораблей каждого типа соответствует правилам
        return shipCounts[1] == 4 && // 4 однопалубных
               shipCounts[2] == 3 && // 3 двухпалубных
               shipCounts[3] == 2 && // 2 трёхпалубных
               shipCounts[4] == 1;   // 1 четырёхпалубный
    }

    // Метод для определения размера корабля и пометки его ячеек как посещённые
    private static int GetShipSize(int[,] grid, bool[,] visited, int startRow, int startCol)
    {
        int size = 0;
        bool isHorizontal = false, isVertical = false;

        // Проверяем горизонтальное расположение
        for (int col = startCol; col < GridSize && grid[startRow, col] == 1; col++)
        {
            if (!IsShipAdjacent(grid, startRow, col)) // Проверка на соседние корабли
            {
                visited[startRow, col] = true;
                size++;
                isHorizontal = true;
            }
            else
            {
                return 0; // Нарушено правило неприкосновенности
            }
        }

        // Проверяем вертикальное расположение
        for (int row = startRow; row < GridSize && grid[row, startCol] == 1; row++)
        {
            if (!IsShipAdjacent(grid, row, startCol)) // Проверка на соседние корабли
            {
                visited[row, startCol] = true;
                size++;
                isVertical = true;
            }
            else
            {
                return 0; // Нарушено правило неприкосновенности
            }
        }

        // Если корабль имеет как горизонтальное, так и вертикальное расположение, это ошибка
        if (isHorizontal && isVertical)
        {
            return 0;
        }

        return size;
    }

    // Метод для проверки, есть ли соседние корабли в пределах 1 клетки
    private static bool IsShipAdjacent(int[,] grid, int row, int col)
    {
        // Проверка всех соседних клеток (включая диагональные)
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int newRow = row + i;
                int newCol = col + j;
                if (newRow >= 0 && newRow < GridSize && newCol >= 0 && newCol < GridSize)
                {
                    if (grid[newRow, newCol] == 1 && (i != 0 || j != 0))
                    {
                        return true; // Найдено соседнее занятие
                    }
                }
            }
        }
        return false;
    }
}
