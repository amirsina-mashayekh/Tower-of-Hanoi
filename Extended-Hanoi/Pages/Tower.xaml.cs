using Extended_Hanoi.HanoiUtil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

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
        /// <c>Canvas.Left</c> value for disks on each peg.
        /// </summary>
        private readonly double[] disksCanvasLeft = new double[3];

        /// <summary>
        /// Minimum <c>Canvas.Button</c> value for disks.
        /// </summary>
        private double DisksMinButton;

        /// <summary>
        /// <c>Canvas.Button</c> value for disks while moving between pegs.
        /// </summary>
        private double DisksMoveButton;

        public static List<Move> Moves { get; set; }

        private readonly int movesCount;

        private int _movesCursor;

        private int MovesCursor
        {
            get => _movesCursor;
            set
            {
                _movesCursor = value;
                if (value == 0)
                {
                    FirstMoveButton.IsEnabled = false;
                    PrevMoveButton.IsEnabled = false;
                }
                else if (value == 1)
                {
                    FirstMoveButton.IsEnabled = true;
                    PrevMoveButton.IsEnabled = true;
                }
                else if (value == movesCount)
                {
                    LastMoveButton.IsEnabled = false;
                    NextMoveButton.IsEnabled = false;
                }
                else if (value == movesCount - 1)
                {
                    LastMoveButton.IsEnabled = true;
                    NextMoveButton.IsEnabled = true;
                }
            }
        }

        private readonly List<Border> p1Disks = new List<Border>();

        private readonly List<Border> p2Disks = new List<Border>();

        private readonly List<Border> p3Disks = new List<Border>();

        private readonly ReadOnlyCollection<List<Border>> pegs;

        public Tower()
        {
            Loaded += Tower_Loaded;
            InitializeComponent();
            pegs = new ReadOnlyCollection<List<Border>>(new List<List<Border>>()
            {
                p1Disks, p2Disks, p3Disks
            });
            movesCount = Moves.Count;
        }

        private void Tower_Loaded(object sender, RoutedEventArgs e)
        {
            // Remove Generating page from navigation history
            _ = NavigationService.RemoveBackEntry();

            // Calculate needed fields
            MaxDiskWidth = Floor.Width / 3;
            disksCanvasLeft[0] = Canvas.GetLeft(Floor);
            disksCanvasLeft[1] = disksCanvasLeft[0] + MaxDiskWidth;
            disksCanvasLeft[2] = disksCanvasLeft[1] + MaxDiskWidth;
            DisksMinButton = Canvas.GetBottom(Floor) + Floor.Height;
            DisksMoveButton = Canvas.GetBottom(Peg1) + Peg1.Height + 15;

            MovesCursor = 0;

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

            // Add all disks to first peg if tower is standard.
            int ex = Generating.TowerIsExtended ? 1 : 0;
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
                        BorderThickness = new Thickness(0, 0.8, 0, 0)
                    }
                };
                pegs[i % 3 * ex].Add(disk);
                _ = TowerCanvas.Children.Add(disk);
            }

            for (int i = 0; i < pegs.Count; i++)
            {
                int pDisksCount = pegs[i].Count;
                for (int j = 0; j < pDisksCount; j++)
                {
                    Border disk = pegs[i][j];
                    Canvas.SetBottom(disk, DisksMinButton + (j * DisksHeight));
                    Canvas.SetLeft(disk, disksCanvasLeft[i]);
                }
            }
        }

        private void BackToStartPageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private CancellationTokenSource playCTS = new CancellationTokenSource(1);

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (playCTS.IsCancellationRequested)
            {
                playCTS = new CancellationTokenSource();
                _ = Play(playCTS.Token);
                FirstMoveButton.IsEnabled = false;
                PrevMoveButton.IsEnabled = false;
                NextMoveButton.IsEnabled = false;
                LastMoveButton.IsEnabled = false;
            }
            else
            {
                playCTS.Cancel();
                FirstMoveButton.IsEnabled = true;
                PrevMoveButton.IsEnabled = true;
                NextMoveButton.IsEnabled = true;
                LastMoveButton.IsEnabled = true;
            }
        }

        private async void NextMoveButton_Click(object sender, RoutedEventArgs e)
        {
            await PerformMove(Moves[MovesCursor++]);
        }

        private async void PrevMoveButton_Click(object sender, RoutedEventArgs e)
        {
            await PerformMove(Move.Reverse(Moves[--MovesCursor]));
        }

        private async void LastMoveButton_Click(object sender, RoutedEventArgs e)
        {
            while (MovesCursor < movesCount)
            {
                await PerformMove(Moves[MovesCursor++], false);
            }
        }

        private async void FirstMoveButton_Click(object sender, RoutedEventArgs e)
        {
            while (MovesCursor > 0)
            {
                await PerformMove(Move.Reverse(Moves[--MovesCursor]), false);
            }
        }

        private async Task PerformMove(Move move, bool animate = true)
        {
            int srcNum = (int)move.Source;
            List<Border> src = pegs[srcNum];
            int dstNum = (int)move.Destination;
            List<Border> dst = pegs[dstNum];

            // Move logically
            int srcLastIndex = src.Count - 1;
            Border disk = src[srcLastIndex];
            dst.Add(src[srcLastIndex]);
            src.RemoveAt(srcLastIndex);

            // Move visually
            // TODO: Animation
            Canvas.SetBottom(disk, DisksMinButton + ((dst.Count - 1) * DisksHeight));
            Canvas.SetLeft(disk, disksCanvasLeft[dstNum]);
            if (animate)
            {
                await Task.Delay(1000 * (100 - (int)SpeedSlider.Value) / 100);
            }
        }

        private async Task Play(CancellationToken cancellationToken)
        {
            PlayPauseButton.Content = "\u23F8";

            while (MovesCursor < movesCount)
            {
                await PerformMove(Moves[MovesCursor++]);
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }

            PlayPauseButton.Content = "\u23F5";
        }
    }
}