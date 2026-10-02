using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            { 0,    ColorBrush("#FF76D6F0") },
            { 2,    ColorBrush("#FFD2FFFD") },
            { 4,    ColorBrush("#FFF6F9EC") },
            { 8,    ColorBrush("#FFFFE5F0") },
            { 16,   ColorBrush("#FFF3E7FF") },
            { 32,   ColorBrush("#FFCDD9FF") },
            { 64,   ColorBrush("#FFDFFFE2") },
            { 128,  ColorBrush("#FFF7FFBB") },
            { 256,  ColorBrush("#FFFFC5E0") },
            { 512,  ColorBrush("#FF24F9E9") },
            { 1024, ColorBrush("#FFC883D1") }
        };

        private readonly Matrix2048 gameMatrix2048 = new Matrix2048();

        private class VisualTile { public int Row, Col, Value; public Border Ui; }
        private readonly List<VisualTile> _activeTiles = new List<VisualTile>();

        public MainWindow()
        {
            InitializeComponent();

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

        public void GameOver() => gameOverOverlay.IsVisible = true;

        private VisualTile CreateTile(int row, int col, int value)
        {
            var border = new Border
            {
                Classes = { "MyBorder" },
                Width = _cellSize - 6,
                Height = _cellSize - 6,
                Background = borderColors.TryGetValue(value, out var brush) ? brush : ColorBrush("#FFF9F9E3"),
                Child = new TextBlock
                {
                    Classes = { "TextBlockStyle" },
                    Text = value.ToString(),
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    Foreground = value <= 512
                        ? new SolidColorBrush(Color.Parse("#FF7D78D1"))
                        : Brushes.White
                }
            };
            Canvas.SetLeft(border, col * _cellSize + 3);
            Canvas.SetTop(border, row * _cellSize + 3);
            return new VisualTile { Row = row, Col = col, Value = value, Ui = border };
        }

        public void UpdateUI()
        {
            canvasTiles.Children.Clear();
            _activeTiles.Clear();

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    int currentValue = gameMatrix2048.GetValue(row, col);
                    if (currentValue > 0)
                    {
                        var tile = CreateTile(row, col, currentValue);
                        canvasTiles.Children.Add(tile.Ui);
                        _activeTiles.Add(tile);

                        if (_mergeDestinations.Contains((row, col)))
                        {
                            AnimatePulseAsync(tile.Ui);
                        }
                        else if (_slideDestinations.Contains((row, col)))
                        {
                            // плитка уже проехала — просто рисуем её на новом месте
                        }
                        else if (_previousMatrix[row, col] == 0)
                        {
                            AnimatePopAsync(tile.Ui);
                        }
                    }
                }
            }
            SaveMatrix();

            // --- Счёт ---
            curScore.Text = _curScore.ToString();

            if (_curScore > _bestScore)
            {
                _bestScore = _curScore;
                ScoreFileManager.SaveBestScore(_bestScore);
            }

            bestScore.Text = _bestScore.ToString();
        }

        private async void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (_isAnimating) return;

            MoveResult moveResult = new MoveResult();
            moveResult.IsMoved = false;
            Key pressedKey = e.Key;

            switch (e.Key)
            {
                case Key.A or Key.Left or Key.NumPad4: moveResult = gameMatrix2048.ToLeft(); break;
                case Key.D or Key.Right or Key.NumPad6: moveResult = gameMatrix2048.ToRight(); break;
                case Key.W or Key.Up or Key.NumPad8: moveResult = gameMatrix2048.ToUp(); break;
                case Key.S or Key.Down or Key.NumPad2: moveResult = gameMatrix2048.ToDown(); break;
                default: return;
            }

            if (!moveResult.IsMoved) return;

            _curScore += moveResult.Score;

            _isAnimating = true;

            _slideDestinations.Clear();
            _mergeDestinations.Clear();

            // 1. Вычисляем ходы на основе СТАРОЙ матрицы
            var moves = CalculateMoves(_previousMatrix, pressedKey);

            // 2. Запускаем анимацию скольжения для существующих плиток
            var animationTasks = new List<Task>();
            foreach (var move in moves)
            {
                if (move.IsMerged)
                    _mergeDestinations.Add((move.ToRow, move.ToCol));
                else
                    _slideDestinations.Add((move.ToRow, move.ToCol));

                var tile = _activeTiles.FirstOrDefault(t => t.Row == move.FromRow && t.Col == move.FromCol);
                if (tile != null)
                {
                    animationTasks.Add(AnimateMoveAsync(tile.Ui, move.ToRow, move.ToCol));
                }
            }

            // 3. Ждём завершения скольжения
            await Task.WhenAll(animationTasks);

            // 4. Добавляем новую случайную плитку в логику
            gameMatrix2048.SpawnNewNum();

            // 5. Перерисовываем UI: старые плитки удалятся, на их месте появятся новые
            //    с анимацией Pop (новая плитка) или Pulse (слияние)
            UpdateUI();

            if (gameMatrix2048.IsGameOver())
                GameOver();

            _slideDestinations.Clear();
            _mergeDestinations.Clear();

            _isAnimating = false;
        }

        private void btn_again_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            gameMatrix2048.StartGame();
            gameOverOverlay.IsVisible = false;
            _curScore = 0;
            SaveMatrix();
            UpdateUI();
        }

        // --- АЛГОРИТМ ВЫЧИСЛЕНИЯ ХОДОВ ---
        private class MoveInfo
        {
            public int FromRow, FromCol, ToRow, ToCol;
            public bool IsMerged;
        }

        private List<MoveInfo> CalculateMoves(int[,] oldMatrix, Key pressedKey)
        {
            var moves = new List<MoveInfo>();
            bool isHorizontal = pressedKey is Key.Left or Key.Right or Key.A or Key.D or Key.NumPad4 or Key.NumPad6;
            bool isReverse = pressedKey is Key.Right or Key.D or Key.NumPad6 or Key.Down or Key.S or Key.NumPad2;

            for (int i = 0; i < 4; i++)
            {
                int[] line = new int[4];
                for (int j = 0; j < 4; j++)
                {
                    int r = isHorizontal ? i : (isReverse ? 3 - j : j);
                    int c = isHorizontal ? (isReverse ? 3 - j : j) : i;
                    line[j] = oldMatrix[r, c];
                }

                int[] result = new int[4];
                bool[] merged = new bool[4];
                int targetPos = 0;

                for (int j = 0; j < 4; j++)
                {
                    if (line[j] == 0) continue;

                    if (targetPos > 0 && result[targetPos - 1] == line[j] && !merged[targetPos - 1])
                    {
                        // Слияние
                        result[targetPos - 1] *= 2;
                        merged[targetPos - 1] = true;

                        int fromR = isHorizontal ? i : (isReverse ? 3 - j : j);
                        int fromC = isHorizontal ? (isReverse ? 3 - j : j) : i;
                        int toR = isHorizontal ? i : (isReverse ? 3 - (targetPos - 1) : (targetPos - 1));
                        int toC = isHorizontal ? (isReverse ? 3 - (targetPos - 1) : (targetPos - 1)) : i;

                        moves.Add(new MoveInfo { FromRow = fromR, FromCol = fromC, ToRow = toR, ToCol = toC, IsMerged = true });
                    }
                    else
                    {
                        // Просто перемещение
                        result[targetPos] = line[j];

                        int fromR = isHorizontal ? i : (isReverse ? 3 - j : j);
                        int fromC = isHorizontal ? (isReverse ? 3 - j : j) : i;
                        int toR = isHorizontal ? i : (isReverse ? 3 - targetPos : targetPos);
                        int toC = isHorizontal ? (isReverse ? 3 - targetPos : targetPos) : i;

                        moves.Add(new MoveInfo { FromRow = fromR, FromCol = fromC, ToRow = toR, ToCol = toC, IsMerged = false });
                        targetPos++;
                    }
                }
            }
            return moves;
        }

        // --- АНИМАЦИИ ---

        private async Task AnimateMoveAsync(Border border, int toRow, int toCol)
        {
            double currentLeft = Canvas.GetLeft(border);
            double currentTop = Canvas.GetTop(border);
            double targetLeft = toCol * _cellSize + 3;
            double targetTop = toRow * _cellSize + 3;

            var animation = new Animation
            {
                Duration = TimeSpan.FromMilliseconds(120),
                Easing = new CubicEaseOut(),
                FillMode = FillMode.Forward,
                Children =
                {
                    new KeyFrame { Cue = new Cue(0d), Setters = {
                        new Setter(Canvas.LeftProperty, currentLeft),
                        new Setter(Canvas.TopProperty, currentTop)
                    }},
                    new KeyFrame { Cue = new Cue(1d), Setters = {
                        new Setter(Canvas.LeftProperty, targetLeft),
                        new Setter(Canvas.TopProperty, targetTop)
                    }}
                }
            };
            await animation.RunAsync(border, CancellationToken.None);
        }

        private async void AnimatePopAsync(Border border)
        {
            var scale = new ScaleTransform(0, 0);
            border.RenderTransform = scale;
            border.RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);

            var animation = new Animation
            {
                Duration = TimeSpan.FromMilliseconds(200),
                Easing = new BackEaseOut(),
                FillMode = FillMode.Forward,
                Children =
                {
                    new KeyFrame { Cue = new Cue(0d), Setters = {
                        new Setter(ScaleTransform.ScaleXProperty, 0.0),
                        new Setter(ScaleTransform.ScaleYProperty, 0.0)
                    }},
                    new KeyFrame { Cue = new Cue(1d), Setters = {
                        new Setter(ScaleTransform.ScaleXProperty, 1.0),
                        new Setter(ScaleTransform.ScaleYProperty, 1.0)
                    }}
                }
            };

            await animation.RunAsync(border, CancellationToken.None);
            border.RenderTransform = null;
        }

        private async void AnimatePulseAsync(Border border)
        {
            var scale = new ScaleTransform(1, 1);
            border.RenderTransform = scale;
            border.RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);

            var animation = new Animation
            {
                Duration = TimeSpan.FromMilliseconds(200),
                Easing = new CubicEaseOut(),
                FillMode = FillMode.Forward,
                Children =
                {
                    new KeyFrame { Cue = new Cue(0.0), Setters = {
                        new Setter(ScaleTransform.ScaleXProperty, 1.0),
                        new Setter(ScaleTransform.ScaleYProperty, 1.0)
                    }},
                    new KeyFrame { Cue = new Cue(0.5), Setters = {
                        new Setter(ScaleTransform.ScaleXProperty, 1.2),
                        new Setter(ScaleTransform.ScaleYProperty, 1.2)
                    }},
                    new KeyFrame { Cue = new Cue(1.0), Setters = {
                        new Setter(ScaleTransform.ScaleXProperty, 1.0),
                        new Setter(ScaleTransform.ScaleYProperty, 1.0)
                    }}
                }
            };

            await animation.RunAsync(border, CancellationToken.None);
            border.RenderTransform = null;
        }
    }
}