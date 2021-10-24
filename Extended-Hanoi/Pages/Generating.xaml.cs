using Extended_Hanoi.HanoiUtil;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Extended_Hanoi.Pages
{
    /// <summary>
    /// Interaction logic for Generating.xaml
    /// </summary>
    public partial class Generating : Page
    {
        public static int TowerHeight { get; set; }

        public static bool TowerIsExtended { get; set; }

        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        public Generating()
        {
            InitializeComponent();
            Loaded += Generating_Loaded;
        }

        private async void Generating_Loaded(object sender, RoutedEventArgs e)
        {
            List<Move> moves = new List<Move>();

            try
            {
                if (TowerIsExtended)
                {
                    await Task.Run(async () =>
                        await Hanoi.SolveExHanoi(Hanoi.Peg.P1, Hanoi.Peg.P2, Hanoi.Peg.P3, TowerHeight, moves, cts.Token)
                        );
                }
                else
                {
                    await Task.Run(async () =>
                        await Hanoi.SolveHanoi(Hanoi.Peg.P1, Hanoi.Peg.P2, Hanoi.Peg.P3, TowerHeight, moves, cts.Token)
                        );
                }

                Tower.Moves = moves;
                _ = NavigationService.Navigate(new Uri("/Pages/Tower.xaml", UriKind.Relative));
            }
            catch (Exception ex)
            {
                string msg = ex is OutOfMemoryException || ex is StackOverflowException
                    ? "Can't generate solution for this tower!\nPlease try a lower height."
                    : "Something went wrong!";

                if (!(ex is OperationCanceledException))
                {
                    _ = MessageBox.Show(Window.GetWindow(this), msg, "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }

                moves = null;
                NavigationService.GoBack();
            }

            Content = null;
            GC.Collect();
        }

        private void CancelGeneratingButton_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
        }
    }
}