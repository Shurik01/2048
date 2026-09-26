using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Styling;
using System;

namespace _2048Avalonia.Views;

public partial class StartWindow : Window
{
    public StartWindow()
    {
        InitializeComponent();
    }

    private void Button_PointerExited(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        MyBorder.RenderTransform = TransformOperations.Parse("scale(1, 1)");
    }

    private void Button_PointerEntered(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        MyBorder.RenderTransform = TransformOperations.Parse("scale(1.054, 1.054)");
    }


    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var mainWindow = new MainWindow();
        mainWindow.Show();
        Close();
    }

}