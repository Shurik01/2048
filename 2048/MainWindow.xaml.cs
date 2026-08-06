using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _2048
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<int, Brush> borderColors = new Dictionary<int, Brush>() { 
            { 0, (Brush)new BrushConverter().ConvertFromString("#FF76D6F0")!},
            { 2, (Brush)new BrushConverter().ConvertFromString("#FFD2FFFD")!},
            { 4, (Brush)new BrushConverter().ConvertFromString("#FFF6F9EC")!},
            { 8, (Brush)new BrushConverter().ConvertFromString("#FFFFE5F0")!},
            { 16, (Brush)new BrushConverter().ConvertFromString("#FFF3E7FF")!},
            { 32, (Brush)new BrushConverter().ConvertFromString("#FFCDD9FF")!},
            { 64, (Brush)new BrushConverter().ConvertFromString("#FFDFFFE2")!},
            { 128, (Brush)new BrushConverter().ConvertFromString("#FFF7FFBB")!},
            { 256, (Brush)new BrushConverter().ConvertFromString("#FFFFC3C3")!},
            { 512, (Brush)new BrushConverter().ConvertFromString("#FF24F9E9")!}
        };

        private Matrix2048 gameMatrix2048 = new Matrix2048();
        public MainWindow()
        {
            InitializeComponent();
            gameMatrix2048.StartGame();
            UpdateUI();
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
                        string oldText = textBlock.Text;
                        string newText = newValue == 0 ? "" : newValue.ToString();

                        textBlock.Text = newText;
                        if(borderColors.ContainsKey(newValue)) border.Background = (Brush)borderColors[newValue];
                        else
                        {
                            border.Background = (Brush)new BrushConverter().ConvertFromString("#FFF9F9E3")!;
                        }
                    }
                }
            }
        }

       
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A || e.Key == Key.Left || e.Key == Key.NumPad4) {
                if (gameMatrix2048.ToLeft()) 
                {
                    UpdateUI();
                    if (gameMatrix2048.SpawnNewNum()) 
                    {
                        UpdateUI();
                    } 
                }            
            }

            if (e.Key == Key.D || e.Key == Key.Right || e.Key == Key.NumPad6)
            {
                if (gameMatrix2048.ToRight())
                {
                    UpdateUI();
                    if (gameMatrix2048.SpawnNewNum())
                    {
                        UpdateUI();
                    }
                }
            }

            if (e.Key == Key.W || e.Key == Key.Up || e.Key == Key.NumPad8)
            {
                if (gameMatrix2048.ToUp())
                {
                    UpdateUI();
                    if (gameMatrix2048.SpawnNewNum())
                    {
                        UpdateUI();
                    }
                }
            }

            if (e.Key == Key.S || e.Key == Key.Down || e.Key == Key.NumPad2)
            {
                if (gameMatrix2048.ToDown())
                {
                    UpdateUI();
                    if (gameMatrix2048.SpawnNewNum())
                    {
                        UpdateUI();
                    }
                }
            }
        }
    }
}