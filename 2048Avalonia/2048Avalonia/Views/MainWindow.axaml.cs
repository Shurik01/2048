using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System.Collections.Generic;

namespace _2048Avalonia.Views
{
    public partial class MainWindow : Window
    {
        private double _cellSize;
        private bool _isAnimating = false;
        private readonly int[,] _previousMatrix = new int[4, 4];

        private int _curScore;
        private int _bestScore;


        private readonly HashSet<(int row, int col)> _slideDestinations = new HashSet<(int, int)>();
        private readonly HashSet<(int row, int col)> _mergeDestinations = new HashSet<(int, int)>();
        private readonly Dictionary<int, IBrush> borderColors = new Dictionary<int, IBrush>()
        {
            { 0, ColorBrush("#FF76D6F0") },
            { 2, ColorBrush("#FFD2FFFD") },
            { 4, ColorBrush("#FFF6F9EC") },
            { 8, ColorBrush("#FFFFE5F0") },
            { 16, ColorBrush("#FFF3E7FF") },
            { 32, ColorBrush("#FFCDD9FF") },
            { 64, ColorBrush("#FFDFFFE2") },
            { 128, ColorBrush("#FFF7FFBB") },
            { 256, ColorBrush("#FFFFC5E0") },
            { 512, ColorBrush("#FF24F9E9") },
            { 1024, ColorBrush("#FFC883D1")}
        };

        private readonly Matrix2048 gameMatrix2048 = new Matrix2048();
        private class VisualTile { public int Row, Col, Value; public Border Ui; }
        private readonly List<VisualTile> _activeTiles = new List<VisualTile>();

        public MainWindow()
        {
            InitializeComponent();

            // Подписка на событие клавиатуры в Avalonia
            KeyDown += Window_KeyDown;

            _cellSize = 97.0;
            gameMatrix2048.StartGame();
            SaveMatrix();

            _curScore = 0;
            _bestScore = ScoreFileManager.LoadBestScore();

            UpdateUI();
        }

        private static IBrush ColorBrush(string hex) => SolidColorBrush.Parse(hex);

        private void SaveMatrix()
        {
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                    _previousMatrix[r, c] = gameMatrix2048.GetValue(r, c);
        }

        public void GameOver()
        {
            gameOverOverlay.IsVisible = true;
        }

        public void UpdateUI()
        {
            foreach (var child in grid2048.Children)
            {
                if (child is Border border)
                {
                    int row = Grid.GetRow(border);
                    int col = Grid.GetColumn(border);
                    int newValue = gameMatrix2048.GetValue(row, col);

                    if (border.Child is TextBlock textBlock)
                    {
                        textBlock.Text = newValue == 0 ? "" : newValue.ToString();

                        if (borderColors.TryGetValue(newValue, out var brush))
                        {
                            border.Background = brush;
                        }
                        else
                        {
                            border.Background = ColorBrush("#FFF9F9E3");
                        }
                    }
                }
            }
        }

        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            bool moved = false;

            switch (e.Key)
            {
                case Key.A:
                case Key.Left:
                case Key.NumPad4:
                    moved = gameMatrix2048.ToLeft();
                    break;
                case Key.D:
                case Key.Right:
                case Key.NumPad6:
                    moved = gameMatrix2048.ToRight();
                    break;
                case Key.W:
                case Key.Up:
                case Key.NumPad8:
                    moved = gameMatrix2048.ToUp();
                    break;
                case Key.S:
                case Key.Down:
                case Key.NumPad2:
                    moved = gameMatrix2048.ToDown();
                    break;
            }

            if (moved)
            {
                UpdateUI();
                if (gameMatrix2048.SpawnNewNum())
                {
                    UpdateUI();
                }
            }

            if (gameMatrix2048.IsGameOver())
            {
                GameOver();
            }
        }

        private void btn_again_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            gameMatrix2048.StartGame();
            gameOverOverlay.IsVisible = false;
            UpdateUI();
        }
    }
}