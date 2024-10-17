using System.Diagnostics;
using System.IO;
using System.Media;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Gambling_But_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random rand = new Random();
        double money = 100;









        // you must debug the countsymbols method and replacesymbols methods













        public MainWindow()
        {
            InitializeComponent(); // Initialize the window
            UpdateMoneyDisplay();  // Show the initial amount of money
        }

        private string GetRandomSymbol()
        {
            List<string> files = new List<string>();
            foreach (string file in Directory.GetFiles("..\\..\\..\\Resources\\"))
            {
                files.Add(file);
            }
            
            int num = rand.Next(0, files.Count * 100);
            switch (num) {
                case > 870:
                    return files[8];

                case > 830:
                    return files[7];

                case > 770:
                    return files[6];

                case > 710:
                    return files[5];

                case > 640:
                    return files[4];

                case > 500:
                    return files[3];

                case > 300:
                    return files[2];

                default:
                    return files[rand.Next(0, 1)];
            }
        }
        public void PlaySound(string path)
        {
            MediaPlayer player = new();
            player.Open(new(path, UriKind.Relative));
            player.Play();
        }
        private async void GambleButton_Click(object sender, RoutedEventArgs e)
        {
            money -= 1; // Deduct money for each gamble
            SymbolsCanvas.Children.Clear();  // Clear previous symbols

            for (int i = 0; i < 6; i++)  // Generate symbols (6 columns)
            {
                for (int j = 0; j < 5; j++)  // 5 rows
                {
                    AddSymbol(new Point(i * 80, j * 80));
                    await Task.Delay(10);
                }
            }

            List<int> symbols = CountSymbols();
            for (int i = 0; i < symbols.Count; i++)
            {
                if (symbols[i] > 7)
                {
                    int test = 0;
                    foreach (int x in symbols)
                    {
                        test += x;
                    }
                    GambleButton.IsEnabled = false;
                    await Task.Delay(1000);
                    money += (symbols[i] - 7) * ((i + 1) * 0.3);
                    ReplaceAllSymbolInstances(i + 1);
                    await Task.Delay(1000);
                    symbols = CountSymbols();
                    i = 0;  // Reset the loop
                    GambleButton.IsEnabled = true;
                }
            }

            UpdateMoneyDisplay();  // Update money display after the symbols are handled
        }

        private void UpdateMoneyDisplay()
        {
            MoneyButton.Content = "$" + money.ToString();  // Update money text
        }

        private async void ReplaceAllSymbolInstances(int symbol)
        {
            string FishSoundPath = "..\\..\\..\\Resources\\Sounds\\fish.wav";
            List<UIElement> symbolInstances = new List<UIElement>();

            foreach (UIElement element in SymbolsCanvas.Children)
            {
                if (element is Label label && label.Name[1].ToString() == symbol.ToString())
                {
                    symbolInstances.Add(label);
                }
            }

            foreach (UIElement instance in symbolInstances)
            {
                SymbolsCanvas.Children.Remove(instance);
                PlaySound(FishSoundPath);
            }

            await Task.Delay(300);
            // Make existing symbols fall down to fill gaps
            for (int i = SymbolsCanvas.Children.Count - 1; i > 0; i--)
            {
                UIElement element = SymbolsCanvas.Children[i];
                while (GetControlAtLocation(new Point(Canvas.GetLeft(element), Canvas.GetTop(element) + 80)) == null && Canvas.GetTop(element) < 320)
                {
                    Canvas.SetTop(element, Canvas.GetTop(element) + 80);
                }
                await Task.Delay(20);
                PlaySound(FishSoundPath);
            }

            // Add new symbols to fill gaps
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (GetControlAtLocation(new Point(i * 80, j * 80)) == null)
                    {
                        AddSymbol(new Point(i * 80, j * 80));
                        await Task.Delay(50);
                        PlaySound(FishSoundPath);
                    }
                }
            }
        }

        private UIElement GetControlAtLocation(Point point)
        {
            foreach (UIElement element in SymbolsCanvas.Children)
            {
                if (Canvas.GetLeft(element) == point.X && Canvas.GetTop(element) == point.Y)
                {
                    return element;
                }
            }
            return null!;
        }

        private void AddSymbol(Point location)
        {
            Label label = new Label();
            string labelSymbol = GetRandomSymbol();
            Image image = new Image();

            // Load image from file
            BitmapImage bitmap = new BitmapImage(new Uri(labelSymbol, UriKind.Relative));
            image.Source = bitmap;
            label.Content = image;

            // Set size and location
            label.Width = 80;
            label.Height = 80;
            label.BorderBrush = Brushes.Black;
            label.BorderThickness = new Thickness(2);
            label.Name = labelSymbol[labelSymbol.Length - 6].ToString() + labelSymbol[labelSymbol.Length - 5].ToString();

            Canvas.SetLeft(label, location.X);
            Canvas.SetTop(label, location.Y);
            SymbolsCanvas.Children.Add(label);
        }

        private List<int> CountSymbols()
        {
            List<int> symbols = new List<int>(new int[9]);  // Initialize with 9 elements (0-8)

            foreach (UIElement element in SymbolsCanvas.Children)
            {
                if (element is Label label)
                {
                    int symbolIndex = int.Parse(label.Name[1].ToString()) - 1;
                    symbols[symbolIndex]++;
                }
            }
            return symbols;
        }
    }
}