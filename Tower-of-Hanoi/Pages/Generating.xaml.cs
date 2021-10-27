using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tower_of_Hanoi.HanoiUtil;

namespace Tower_of_Hanoi.Pages
{
    /// <summary>
    /// Interaction logic for Generating.xaml
    /// </summary>
    public partial class Generating : Page
    {
        public static int TowerHeight { get; set; }

        public static bool TowerIsExtended { get; set; }

        private static List<Move> moves;

        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        public Generating()
        {
            InitializeComponent();
            Loaded += Generating_Loaded;
        }

        private async void Generating_Loaded(object sender, RoutedEventArgs e)
        {
            moves = new List<Move>();

            System.Timers.Timer movesUpdateTimer = new System.Timers.Timer(100)
            {
                AutoReset = true
            };
            movesUpdateTimer.Elapsed += MovesUpdateTimer_Elapsed;

            try
            {
                movesUpdateTimer.Start();
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

                Tower.Moves = new ReadOnlyCollection<Move>(moves);
                _ = NavigationService.Navigate(new Uri("/Pages/Tower.xaml", UriKind.Relative));
            }
            catch (Exception ex)
            {
                movesUpdateTimer.Stop();
                string msg = ex is OutOfMemoryException || ex is StackOverflowException
                    ? "Can't generate solution for this tower!\nPlease try a lower height."
                    : "Something went wrong!";

                if (!(ex is OperationCanceledException))
                {
                    _ = MessageBox.Show(Window.GetWindow(this), msg, "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }

                NavigationService.GoBack();
            }

            movesUpdateTimer.Stop();
            moves = null;
            Content = null;
            GC.Collect();
        }

        private void MovesUpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                _ = Dispatcher.Invoke(() => MovesTextBlock.Text = moves.Count.ToString("#,0") + " Moves");
            }
            catch (NullReferenceException)
            {
                // Sometimes occures if operation is finished or canceled while counting
            }
        }

        private void CancelGeneratingButton_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
        }
    }
}