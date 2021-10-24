using Extended_Hanoi.HanoiUtil;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        /// <summary>
        /// A list which contains disk moves.
        /// </summary>
        public static List<Move> Moves { get; set; }

        /// <summary>
        /// Total moves count.
        /// </summary>
        private readonly int totalMoves;

        private int _movesCursor;

        /// <summary>
        /// A cursor which points to current move.
        /// </summary>
        private int MovesCursor
        {
            get => _movesCursor;
            set
            {
                _movesCursor = value;

                // If play is running, buttons status shouldn't be changed
                if (!playCTS.IsCancellationRequested) { return; }

                if (value == 0)
                {
                    FirstMoveButton.IsEnabled = false;
                    PrevMoveButton.IsEnabled = false;
                    PlayPauseButton.IsEnabled = true;
                    LastMoveButton.IsEnabled = true;
                    NextMoveButton.IsEnabled = true;
                }
                else if (value == totalMoves)
                {
                    LastMoveButton.IsEnabled = false;
                    NextMoveButton.IsEnabled = false;
                    PlayPauseButton.IsEnabled = false;
                    FirstMoveButton.IsEnabled = true;
                    PrevMoveButton.IsEnabled = true;
                }
                else
                {
                    FirstMoveButton.IsEnabled = true;
                    PrevMoveButton.IsEnabled = true;
                    PlayPauseButton.IsEnabled = true;
                    LastMoveButton.IsEnabled = true;
                    NextMoveButton.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Disks on peg 1.
        /// </summary>
        private readonly List<Border> p1Disks = new List<Border>();

        /// <summary>
        /// Disks on peg 2.
        /// </summary>
        private readonly List<Border> p2Disks = new List<Border>();

        /// <summary>
        /// Disks on peg 3.
        /// </summary>
        private readonly List<Border> p3Disks = new List<Border>();

        /// <summary>
        /// Array of pegs.
        /// </summary>
        private readonly List<Border>[] pegs = new List<Border>[3];

        public Tower()
        {
            Loaded += Tower_Loaded;
            InitializeComponent();
            pegs = new List<Border>[]
            {
                p1Disks, p2Disks, p3Disks
            };
            totalMoves = Moves.Count;
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
            if (Generating.TowerIsExtended)
            {
                // In extended mode, all disks may go to a single peg.
                // So we divide MaxTotalDisksHeight so that all disks will fit in this situation
                MaxTotalDisksHeight /= 3;
            }

            int height = Generating.TowerHeight;

            int disksCount = height;
            if (Generating.TowerIsExtended) { disksCount *= 3; }

            DisksHeight = Math.Min(MaxDiskHeight, MaxTotalDisksHeight / height);

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

            RedrawTower();
        }

        private void BackToStartPageButton_Click(object sender, RoutedEventArgs e)
        {
            playCTS.Cancel();
            NavigationService.GoBack();
        }

        private CancellationTokenSource playCTS = new CancellationTokenSource(1);

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (playCTS.IsCancellationRequested)
            {
                playCTS = new CancellationTokenSource();
                _ = PlayAsync(playCTS.Token);
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
            ControlsGrid.IsEnabled = false;
            await PerformMoveAsync(Moves[MovesCursor]);
            ControlsGrid.IsEnabled = true;
            MovesCursor++;
        }

        private async void PrevMoveButton_Click(object sender, RoutedEventArgs e)
        {
            MovesCursor--;
            ControlsGrid.IsEnabled = false;
            await PerformMoveAsync(Move.Reverse(Moves[MovesCursor]));
            ControlsGrid.IsEnabled = true;
            MovesCursor = MovesCursor;  // To update buttons status
        }

        private async void LastMoveButton_Click(object sender, RoutedEventArgs e)
        {
            ControlsGrid.IsEnabled = false;
            ControlsGridGrid.Cursor = Cursors.Wait;

            await Task.Run(async () =>
            {
                while (MovesCursor < totalMoves)
                {
                    /* Used cursor as field instead of property to:
                     * -Prevent unnecessary buttons update
                     * -Be able to access cursor from new thread
                     */
                    await PerformMoveAsync(Moves[_movesCursor++], false);
                }
            });

            MovesCursor = MovesCursor;  // To update buttons status
            ControlsGridGrid.Cursor = Cursors.Arrow;
            ControlsGrid.IsEnabled = true;
            RedrawTower();
        }

        private async void FirstMoveButton_Click(object sender, RoutedEventArgs e)
        {
            ControlsGrid.IsEnabled = false;
            ControlsGridGrid.Cursor = Cursors.Wait;

            await Task.Run(async () =>
            {
                while (MovesCursor > 0)
                {
                    /* Used cursor as field instead of property to:
                     * -Prevent unnecessary buttons update
                     * -Be able to access cursor from new thread
                     */
                    await PerformMoveAsync(Move.Reverse(Moves[--_movesCursor]), false);
                }
            });

            MovesCursor = MovesCursor;  // To update buttons status
            ControlsGridGrid.Cursor = Cursors.Arrow;
            ControlsGrid.IsEnabled = true;
            RedrawTower();
        }

        /// <summary>
        /// Performs a move using a <c>Move</c> object.
        /// </summary>
        /// <param name="move">A <c>Move</c> object.</param>
        /// <param name="visual">Determines should the move be visual.</param>
        /// <returns>A <c>Task</c> object.</returns>
        private async Task PerformMoveAsync(Move move, bool visual = true)
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

            if (visual)
            {
                // Move visually
                // TODO: Animation
                Canvas.SetBottom(disk, DisksMinButton + ((dst.Count - 1) * DisksHeight));
                Canvas.SetLeft(disk, disksCanvasLeft[dstNum]);
                await Task.Delay(1000 * (100 - (int)SpeedSlider.Value) / 100);
            }
        }

        /// <summary>
        /// Performs moves in <c>Moves</c> list until it is stopped or reached the end.
        /// </summary>
        /// <param name="cancellationToken">a <c>CancellationToken</c> to stop the task when needed.</param>
        /// <returns>A <c>Task</c> object.</returns>
        private async Task PlayAsync(CancellationToken cancellationToken)
        {
            PlayPauseButton.Content = "\u23F8";

            while (MovesCursor < totalMoves)
            {
                await PerformMoveAsync(Moves[MovesCursor++]);
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }

            if (!playCTS.IsCancellationRequested) { playCTS.Cancel(); }
            PrevMoveButton.IsEnabled = true;
            FirstMoveButton.IsEnabled = true;
            PlayPauseButton.Content = "\u23F5";
        }

        /// <summary>
        /// Sets place of all disks based on <c>pegs</c> array.
        /// </summary>
        private void RedrawTower()
        {
            for (int i = 0; i < pegs.Length; i++)
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
    }
}