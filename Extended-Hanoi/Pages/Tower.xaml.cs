using Extended_Hanoi.HanoiUtil;
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

namespace Extended_Hanoi.Pages
{
    /// <summary>
    /// Interaction logic for Tower.xaml
    /// </summary>
    public partial class Tower : Page
    {
        public static List<Move> Moves { get; set; }

        public Tower()
        {
            InitializeComponent();
            Loaded += Tower_Loaded;
        }

        private void Tower_Loaded(object sender, RoutedEventArgs e)
        {
            // Remove Generating page from navigation history
            _ = NavigationService.RemoveBackEntry();
        }

        private void BackToStartPageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
