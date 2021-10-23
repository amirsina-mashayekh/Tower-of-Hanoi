using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Extended_Hanoi.HanoiUtil;

namespace Extended_Hanoi.Pages
{
    /// <summary>
    /// Interaction logic for Generating.xaml
    /// </summary>
    public partial class Generating : Page
    {
        public static int TowerHeight { get; set; }

        public static bool TowerIsExtended { get; set; }

        public Generating()
        {
            InitializeComponent();

            Loaded += Generating_Loaded;
        }

        private void Generating_Loaded(object sender, RoutedEventArgs e)
        {
            List<Move> moves = new List<Move>();

            // TODO: async
            try
            {
                if (TowerIsExtended)
                {
                    Hanoi.SolveExHanoi(Hanoi.Peg.P1, Hanoi.Peg.P2, Hanoi.Peg.P3, TowerHeight, moves);
                }
                else
                {
                    Hanoi.SolveHanoi(Hanoi.Peg.P1, Hanoi.Peg.P2, Hanoi.Peg.P3, TowerHeight, moves);
                }
            }
            catch (Exception)
            {
                _ = MessageBox.Show(Window.GetWindow(this), "Something went wrong!", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Tower.Moves = moves;

            _ = NavigationService.Navigate(new Uri("/Pages/Tower.xaml", UriKind.Relative));
        }

        private void CancelGeneratingButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
