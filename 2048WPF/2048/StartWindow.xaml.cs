using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _2048
{
    /// <summary>
    /// Логика взаимодействия для StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            // закрывается текущее окно
            Close();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation scaleXAnim = new DoubleAnimation { To = 1.054, Duration = TimeSpan.FromSeconds(0.2) };
            DoubleAnimation scaleYAnim = new DoubleAnimation { To = 1.054, Duration = TimeSpan.FromSeconds(0.2) };

            MyScale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnim);
            MyScale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnim);
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            // Возвращаем масштаб в исходные 1.0 (оригинальный размер)
            DoubleAnimation scaleXAnim = new DoubleAnimation { To = 1.0, Duration = TimeSpan.FromSeconds(0.2) };
            DoubleAnimation scaleYAnim = new DoubleAnimation { To = 1.0, Duration = TimeSpan.FromSeconds(0.2) };

            MyScale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnim);
            MyScale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnim);
        }
    }
}
