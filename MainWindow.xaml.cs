using System;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BattleshipGame
{
    public partial class MainWindow : Window
    {
        private int GridSize = 10; // Размер поля 10x10
        private int[,] playerGrid; // Поле игрока
        private int[,] computerGrid; // Поле компьютера

        private Button[,] playerButtons; // Кнопки для отображения поля игрока
        private Button[,] computerButtons; // Кнопки для отображения поля компьютера
        private int computer_score = 0;
        private int user_score = 0;
        public MainWindow()
        {

            InitializeComponent();
            
            sizeInput.Text = GridSize.ToString();

            sizeInput.IsEnabled = false;
        }
        // Инициализация визуальных кнопок для полей игрока и компьютера
        private void InitializeGrids()
        {
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    // Кнопки для игрока
                    Button playerButton = new Button();
                    playerButton.Width = 50;
                    playerButton.Height = 50;
                    playerButton.Background = Brushes.LightBlue;
                    PlayerGrid.Children.Add(playerButton);
                    Grid.SetRow(playerButton, row);
                    Grid.SetColumn(playerButton, col);
                    playerButtons[row, col] = playerButton;

                    // Кнопки для компьютера
                    Button computerButton = new Button();
                    computerButton.Width = 50;
                    computerButton.Height = 50;
                    computerButton.Background = Brushes.LightGray;
                    computerButton.Click += ComputerButton_Click; // Добавляем обработчик клика для выстрела
                    ComputerGrid.Children.Add(computerButton);
                    Grid.SetRow(computerButton, row);
                    Grid.SetColumn(computerButton, col);
                    computerButtons[row, col] = computerButton;
                }
            }
        }

        // Обновление поля с учетом размещения кораблей (если hidden == true, то скрываем корабли)
        private void UpdateGridUI(Button[,] buttons, int[,] grid, Grid uiGrid, bool hidden)
        {
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    if (!hidden)
                    {
                        buttons[row, col].Background = grid[row, col] == 1 ? Brushes.Blue : Brushes.LightBlue;
                    }
                }
            }
        }

        // Обработчик клика по кнопке на поле компьютера (выстрел игрока)
        private void ComputerButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            int row = Grid.GetRow(clickedButton);
            int col = Grid.GetColumn(clickedButton);

            // Проверяем, попал ли игрок в корабль
            if (computerGrid[row, col] == 1)
            {
                user_score++;
                clickedButton.Background = Brushes.Red; // Попадание
                computerGrid[row, col] = -1; // Отмечаем как поражённую клетку
            }
            else if (computerGrid[row, col] == -1)
            {
                MessageBox.Show("Корабель в цій клітинці вже вражений. Повторіть свій ход.");
                return;
            }
            else
            {
                clickedButton.IsEnabled = false;
                clickedButton.Background = Brushes.White; // Промах
                computerGrid[row, col] = -2; // Отмечаем как пустую проверенную клетку
            }


            if (user_score == 20)
            {
                victory();
            }
            // clickedButton.IsEnabled = false; // Деактивируем кнопку

            // Ход компьютера
            ComputerTurn();
        }

        // Ход компьютера (случайный выбор клетки на поле игрока)
        private void ComputerTurn()
        {
            Random rand = new Random();
            bool validShot = false;
            
            while (!validShot)
            {
                int row = rand.Next(GridSize);
                int col = rand.Next(GridSize);

                if (playerGrid[row, col] == 1)
                {
                    computer_score++;
                    playerButtons[row, col].Background = Brushes.Red; // Компьютер попал
                    playerGrid[row, col] = -1; // Помечаем клетку как поражённую
                    validShot = true;
                }
                else if (playerGrid[row, col] == 0)
                {
                    playerButtons[row, col].Background = Brushes.White; // Компьютер промахнулся
                    playerGrid[row, col] = -1; // Помечаем клетку как пустую проверенную
                    validShot = true;
                }
            }
            if (computer_score == 20)
            {
                victory();
            }
        }
        private void victory()
        {
            string user_name = userName.Text;
            if (computer_score == 20)
            {
                MessageBox.Show($@"
Перемога комп'ютера!
Рахунок: 
комп'ютер: {computer_score}
користувач {user_name}: {user_score}
");
            }
            else if (user_score == 20)
            {
                MessageBox.Show($@"
Перемога користувача!
Рахунок: 
користувач {user_name}: {user_score}
комп'ютер: {computer_score}
");
            }

            user_score = 0;
            computer_score = 0;
        }

        private void initAndStart_Click(object sender, RoutedEventArgs e)
        {
            GridSize = Convert.ToInt32(sizeInput.Text);
            
            playerGrid = new int[GridSize, GridSize];
            computerGrid = new int[GridSize, GridSize]; // Поле компьютера

            playerButtons = new Button[GridSize, GridSize]; // Кнопки для отображения поля игрока
            computerButtons = new Button[GridSize, GridSize];
 
            InitializeGrids();
            // Автоматически размещаем корабли на обоих полях
            BattleshipGame.PlaceShips(playerGrid, GridSize, GridSize);
            BattleshipGame.PlaceShips(computerGrid, GridSize, GridSize);

            // Отображаем поле игрока
            UpdateGridUI(playerButtons, playerGrid, PlayerGrid, false);

            // Поле компьютера скрывается (корабли скрыты)
            UpdateGridUI(computerButtons, computerGrid, ComputerGrid, true);
            sizeInput.IsEnabled = false;
            // initAndStart.IsEnabled = false;


        }

        private void open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)

                reset_proc(true, FileOperations.NewReadArrayFromFile(openFileDialog.FileName)); 
                
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                FileOperations.WriteArrayToFile(computerGrid, saveFileDialog.FileName);
            }
        
        }

        void reset_proc(bool rewrite_computer_map = false, int[,] computer_map_from_file = null)
        {
            
            user_score = 0;
            computer_score = 0;

            if (rewrite_computer_map && computer_map_from_file != null)
            {
                GridSize = computer_map_from_file.GetLength(0);
            }
            GridSize = Convert.ToInt32(sizeInput.Text);

            playerGrid = null;
            computerGrid = null; // Поле компьютера

            playerButtons = null; // Кнопки для отображения поля игрока
            computerButtons = null;

            playerGrid = new int[GridSize, GridSize];
            if (rewrite_computer_map && computer_map_from_file != null)
            {
                computerGrid = computer_map_from_file; // Поле компьютера
            }
            else
            {
                computerGrid = new int[GridSize, GridSize]; // Поле 
            }

            playerButtons = new Button[GridSize, GridSize]; // Кнопки для отображения поля игрока
            computerButtons = new Button[GridSize, GridSize];

            InitializeGrids();
            // Автоматически размещаем корабли на обоих полях
            BattleshipGame.PlaceShips(playerGrid, GridSize, GridSize);
            if (!rewrite_computer_map)
            {
                BattleshipGame.PlaceShips(computerGrid, GridSize, GridSize);
            }


            // Отображаем поле игрока
            UpdateGridUI(playerButtons, playerGrid, PlayerGrid, false);

            // Поле компьютера скрывается (корабли скрыты)
            UpdateGridUI(computerButtons, computerGrid, ComputerGrid, true);
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            reset_proc();
        }
    }



    public static class BattleshipGame
    {
        private static Random rand = new Random();

        // Функция для размещения всех кораблей
        public static void PlaceShips(int[,] array, int x_dim, int y_dim)
        {
            PlaceShip(array, 4); // Размещение одного четырехпалубного корабля
            for (int i = 0; i < 2; i++) { PlaceShip(array, 3); } // Два трехпалубных
            for (int i = 0; i < 3; i++) { PlaceShip(array, 2); } // Три двухпалубных
            for (int i = 0; i < 4; i++) { PlaceShip(array, 1); } // Четыре однопалубных
        }

        // Функция для размещения корабля заданной длины
        private static void PlaceShip(int[,] array, int shipLength)
        {
            int x_dim = array.GetLength(1); // Ширина массива (по X)
            int y_dim = array.GetLength(0); // Высота массива (по Y)

            bool placed = false;

            // Пока корабль не размещён
            while (!placed)
            {
                bool horizontal = rand.Next(2) == 0; // Определяем направление (0 - горизонтально, 1 - вертикально)
                int startX = rand.Next(x_dim); // Начальная координата X
                int startY = rand.Next(y_dim); // Начальная координата Y

                // Проверяем, можно ли разместить корабль в указанной позиции
                if (CanPlaceShip(array, startX, startY, shipLength, horizontal))
                {
                    // Размещаем корабль как непрерывную последовательность 1
                    PlaceShipOnBoard(array, startX, startY, shipLength, horizontal);
                    placed = true; // Корабль успешно размещён
                }
            }
        }

        // Проверка, можно ли разместить корабль по заданным координатам
        private static bool CanPlaceShip(int[,] array, int startX, int startY, int shipLength, bool horizontal)
        {
            int x_dim = array.GetLength(1);
            int y_dim = array.GetLength(0);

            // Проверка всех клеток, где планируется разместить корабль
            for (int i = 0; i < shipLength; i++)
            {
                int x = horizontal ? startX + i : startX; // Если горизонтально, увеличиваем X
                int y = horizontal ? startY : startY + i; // Если вертикально, увеличиваем Y

                // Проверяем, не выходит ли за пределы доски или уже занято
                if (x >= x_dim || y >= y_dim || array[y, x] != 0)
                {
                    return false;
                }

                // Проверка на соседние клетки (не должно быть кораблей рядом)
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int checkX = x + dx;
                        int checkY = y + dy;
                        if (checkX >= 0 && checkX < x_dim && checkY >= 0 && checkY < y_dim && array[checkY, checkX] != 0)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        // Размещение корабля в виде непрерывной последовательности 1
        private static void PlaceShipOnBoard(int[,] array, int startX, int startY, int shipLength, bool horizontal)
        {
            for (int i = 0; i < shipLength; i++)
            {
                int x = horizontal ? startX + i : startX;
                int y = horizontal ? startY : startY + i;
                array[y, x] = 1; // Устанавливаем 1 для клеток, где расположен корабль
            }
        }
    }

}
