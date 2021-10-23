using System.Windows;
using static Extended_Hanoi.HanoiUtil.Hanoi;
using System.Windows.Controls;
using Extended_Hanoi.Pages;

namespace Extended_Hanoi
{
    /// <summary>
    /// Interaction logic for Start.xaml
    /// </summary>
    public partial class Start : Page
    {
        public Start()
        {
            InitializeComponent();
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(DisksCountTextBox.Text, out int height) || height < 1)
            {
                _ = MessageBox.Show(Window.GetWindow(this), "Height of Tower is invalid!", "",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Generating.TowerHeight = height;
            Generating.TowerIsExtended = (bool)ExtendedRadio.IsChecked;
            NavigationService.Navigate(new System.Uri("/Pages/Generating.xaml", System.UriKind.Relative));
        }
    }
}
