using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        }

        private void CancelGeneratingButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
