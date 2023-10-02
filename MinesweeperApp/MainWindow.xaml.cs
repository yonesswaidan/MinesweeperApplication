using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Minesweeper
{
    public partial class MainWindow : Window
    {
        private int Rows = 10;
        private int Columns = 10;
        private int Mines = 10;
        private Button[,] mineButtons;
        private bool[,] mineLocations;
        private bool gameOver;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            mineButtons = new Button[Rows, Columns];
            mineLocations = new bool[Rows, Columns];
            gameOver = false;
            GenerateMinesweeperGrid();
        }

        private void GenerateMinesweeperGrid()
        {
            minefield.Children.Clear();
            Random random = new Random();

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    Button button = new Button
                    {
                        Tag = "0",
                        IsEnabled = true,
                        Style = (Style)FindResource("MineButtonStyle")
                    };

                    button.Click += Button_Click;

                    minefield.Children.Add(button);
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                    mineButtons[row, col] = button;
                }
            }

            // Den palcer dem tilfældigt
            int minesToPlace = Mines;
            while (minesToPlace > 0)
            {
                int row = random.Next(Rows);
                int col = random.Next(Columns);

                if (!mineLocations[row, col])
                {
                    mineLocations[row, col] = true;
                    minesToPlace--;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (gameOver)
            {
                return;
            }

            Button button = (Button)sender;
            int row = Grid.GetRow(button);
            int col = Grid.GetColumn(button);

            if (mineLocations[row, col])
            {
                button.Content = "X";
                GameOver();
            }
            else
            {
                int adjacentMines = CountAdjacentMines(row, col);
                if (adjacentMines == 0)
                {
                    RevealEmptyCells(row, col);
                }
                else
                {
                    button.Content = adjacentMines.ToString();
                }
                button.IsEnabled = false;
                CheckForWin();
            }
        }

        private int CountAdjacentMines(int row, int col)
        {
            int mines = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int newRow = row + i;
                    int newCol = col + j;

                    if (newRow >= 0 && newRow < Rows && newCol >= 0 && newCol < Columns)
                    {
                        if (mineLocations[newRow, newCol])
                        {
                            mines++;
                        }
                    }
                }
            }
            return mines;
        }

        private void RevealEmptyCells(int row, int col)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int newRow = row + i;
                    int newCol = col + j;

                    if (newRow >= 0 && newRow < Rows && newCol >= 0 && newCol < Columns)
                    {
                        Button button = mineButtons[newRow, newCol];
                        if (button.IsEnabled)
                        {
                            int adjacentMines = CountAdjacentMines(newRow, newCol);
                            if (adjacentMines == 0)
                            {
                                button.Content = "";
                                button.IsEnabled = false;
                                RevealEmptyCells(newRow, newCol);
                            }
                            else
                            {
                                button.Content = adjacentMines.ToString();
                                button.IsEnabled = false;
                            }
                        }
                    }
                }
            }
        }

        private void GameOver()
        {
            MessageBox.Show("Game Over!");
            gameOver = true;
        }

        private void CheckForWin()
        {
            int unrevealedCount = 0;
            foreach (Button button in mineButtons)
            {
                if (button.IsEnabled && button.Content.ToString() != "X")
                {
                    unrevealedCount++;
                }
            }

            if (unrevealedCount == Mines)
            {
                MessageBox.Show("Congratulations! You won!");
                gameOver = true;
            }
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            InitializeGame();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Minesweeper by [Yones Khaled Swaidan]");
        }
    }
}
