using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace _2048Avalonia.Views
{
    public partial class CloseWindow : Window
    {
        private readonly Bitmap _sadImage;
        private readonly Bitmap _happyImage;
        private readonly Bitmap _standartImage;
        public CloseWindow()
        {
            InitializeComponent();
            _sadImage = LoadBitmap("avares://2048Avalonia/Assets/SadCinnamoroll.png");
            _happyImage = LoadBitmap("avares://2048Avalonia/Assets/HappyCinnamoroll.png");
            _standartImage = LoadBitmap("avares://2048Avalonia/Assets/StandartCinnamoroll.png");
        }

        private static Bitmap LoadBitmap(string uri) =>
        new Bitmap(AssetLoader.Open(new System.Uri(uri)));

        private void No_Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(false);
        }

        private void Yes_Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(true);
        }

        private void Button_PointerExited(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            CinnamorollImage.Source = _standartImage;
        }

        private void Yes_Button_PointerEntered(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            CinnamorollImage.Source = _sadImage;
        }

        private void No_Button_PointerEntered(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            CinnamorollImage.Source = _happyImage;
        }
    }
}