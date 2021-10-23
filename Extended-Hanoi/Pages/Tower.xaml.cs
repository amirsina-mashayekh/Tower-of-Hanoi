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
        /// <summary>
        /// Maximum width of disks.
        /// </summary>
        private double MaxDiskWidth;

        /// <summary>
        /// Maximum height of disks.
        /// </summary>
        private const double MaxDiskHeight = 50;

        /// <summary>
        /// Current height of disks.
        /// </summary>
        private double DisksHeight;

        /// <summary>
        /// <c>Canvas.Left</c> value for disks on Peg 1.
        /// </summary>
        private double p1DisksLeft;

        /// <summary>
        /// <c>Canvas.Left</c> value for disks on Peg 2.
        /// </summary>
        private double p2DisksLeft;

        /// <summary>
        /// <c>Canvas.Left</c> value for disks on Peg 3.
        /// </summary>
        private double p3DisksLeft;

        /// <summary>
        /// Minimum <c>Canvas.Button</c> value for disks.
        /// </summary>
        private double DisksMinButton;

        /// <summary>
        /// <c>Canvas.Button</c> value for disks while moving between pegs.
        /// </summary>
        private double DisksMoveButton;

        public static List<Move> Moves { get; set; }

        private readonly List<Border> p1Disks = new List<Border>();

        private readonly List<Border> p2Disks = new List<Border>();

        private readonly List<Border> p3Disks = new List<Border>();

        public Tower()
        {
            Loaded += Tower_Loaded;
            InitializeComponent();
        }

        private void Tower_Loaded(object sender, RoutedEventArgs e)
        {
            // Remove Generating page from navigation history
            _ = NavigationService.RemoveBackEntry();

            // Calculate needed fields
            MaxDiskWidth = Floor.Width / 3;
            p1DisksLeft = Canvas.GetLeft(Floor);
            p2DisksLeft = p1DisksLeft + MaxDiskWidth;
            p3DisksLeft = p2DisksLeft + MaxDiskWidth;
            DisksMinButton = Canvas.GetBottom(Floor) + Floor.Height;
            DisksMoveButton = Canvas.GetBottom(Peg1) + Peg1.Height + 15;

            double MinDiskWidth = Peg1.Width * 3;
            double MaxTotalDisksHeight = Peg1.Height - Floor.Height - 15;

            int height = Generating.TowerHeight;

            int disksCount = height;
            if (Generating.TowerIsExtended) { disksCount *= 3; }

            DisksHeight = Math.Min(MaxDiskHeight, MaxTotalDisksHeight / height);
            if (Generating.TowerIsExtended)
            {
                // In extended mode, all disks may go to a single peg.
                // So we divide DisksHeight so that all disks will fit in this situation
                DisksHeight /= 3;
            }

            double disksWidthStep = (MaxDiskWidth - MinDiskWidth) / disksCount;

            List<Border>[] pegs = new List<Border>[3];
            if (Generating.TowerIsExtended)
            {
                pegs[0] = p1Disks;
                pegs[1] = p2Disks;
                pegs[2] = p3Disks;
            }
            else
            {
                pegs[0] = pegs[1] = pegs[2] = p1Disks;
            }

            for (int i = 0; i < disksCount; i++)
            {
                double padding = (i * disksWidthStep / 2) + 5;
                Border disk = new Border()
                {
                    Width = MaxDiskWidth,
                    Height = DisksHeight,
                    Padding = new Thickness(padding, 0, padding, 0),
                    Child = new Border()
                    {
                        CornerRadius = new CornerRadius(DisksHeight / 16),
                        Background = new SolidColorBrush(Color.FromArgb(127, 0, 0, 255)),
                        BorderBrush = new SolidColorBrush(Color.FromArgb(190, 0, 0, 255)),
                        BorderThickness = new Thickness(0, 1, 0, 0)
                    }
                };
                pegs[i % 3].Add(disk);
                _ = TowerCanvas.Children.Add(disk);
            }

            int p1Count = p1Disks.Count;
            for (int i = 0; i < p1Count; i++)
            {
                Border disk = p1Disks[i];
                Canvas.SetBottom(disk, DisksMinButton + (i * DisksHeight));
                Canvas.SetLeft(disk, p1DisksLeft);
            }

            int p2Count = p2Disks.Count;
            for (int i = 0; i < p2Count; i++)
            {
                Border disk = p2Disks[i];
                Canvas.SetBottom(disk, DisksMinButton + (i * DisksHeight));
                Canvas.SetLeft(disk, p2DisksLeft);
            }

            int p3Count = p3Disks.Count;
            for (int i = 0; i < p3Count; i++)
            {
                Border disk = p3Disks[i];
                Canvas.SetBottom(disk, DisksMinButton + (i * DisksHeight));
                Canvas.SetLeft(disk, p3DisksLeft);
            }
        }

        private void BackToStartPageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}