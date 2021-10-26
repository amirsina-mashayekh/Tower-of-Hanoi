using Extended_Hanoi.HanoiUtil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Extended_Hanoi.Pages
{
    /// <summary>
    /// Interaction logic for Tower.xaml
    /// </summary>
    public partial class Tower : Page, INotifyPropertyChanged
    {
        /// <summary>
        /// Maximum duration for an animation in milliseconds
        /// </summary>
        private const double maxAnimationDuration = 1000;

        /// <summary>
        /// Minimum duration for an animation in milliseconds
        /// </summary>
        private const double minAnimationDuration = 75;

        /// <summary>
        /// Current duration of animations
        /// </summary>
        private double animationTime;

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
        /// Minimum <c>Canvas.Bottom</c> value for disks.
        /// </summary>
        private double DisksMinBottom;

        /// <summary>
        /// <c>Canvas.Bottom</c> value for disks while moving between pegs.
        /// </summary>
        private double DisksMoveBottom;

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

                if (value == 0)
                {
                    FastReverseButton.IsEnabled = false;
                    PrevMoveButton.IsEnabled = false;
                    PlayPauseButton.IsEnabled = true;
                    FastForwardButton.IsEnabled = true;
                    NextMoveButton.IsEnabled = true;
                }
                else if (value == totalMoves)
                {
                    FastForwardButton.IsEnabled = false;
                    NextMoveButton.IsEnabled = false;
                    PlayPauseButton.IsEnabled = false;
                    FastReverseButton.IsEnabled = true;
                    PrevMoveButton.IsEnabled = true;
                }
                else
                {
                    FastReverseButton.IsEnabled = true;
                    PrevMoveButton.IsEnabled = true;
                    PlayPauseButton.IsEnabled = true;
                    FastForwardButton.IsEnabled = true;
                    NextMoveButton.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Represents the remaining moves
        /// </summary>
        public string RemainingMoves
        {
            get
            {
                int rem = totalMoves - MovesCursor;
                StringBuilder str = new StringBuilder(rem > 0 ? rem.ToString("#,0") : "No");
                _ = str.Append(" Move");
                if (rem != 1) { _ = str.Append('s'); }
                return str.ToString();
            }
            set => OnPropertyChanged();
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

        /// <summary>
        /// Cancellation Token Source for play/pause
        /// </summary>
        private CancellationTokenSource playCTS = new CancellationTokenSource(1);

        private int _animationSpeed;

        /// <summary>
        /// Animation speed (percent)
        /// </summary>
        public double AnimationSpeed
        {
            get => _animationSpeed;
            set
            {
                _animationSpeed = (int)value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

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
            DisksMinBottom = Canvas.GetBottom(Floor) + Floor.Height;
            DisksMoveBottom = Canvas.GetBottom(Peg1) + Peg1.Height + 15;

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
                    SnapsToDevicePixels = true,
                    Child = new Border()
                    {
                        CornerRadius = new CornerRadius(DisksHeight / 16),
                        Background = new SolidColorBrush(Color.FromArgb(127, 0, 0, 255)),
                        BorderBrush = new SolidColorBrush(Color.FromArgb(190, 0, 0, 255)),
                        BorderThickness = new Thickness(1, 1, 1, 0.5),
                        SnapsToDevicePixels = true
                    }
                };
                pegs[i % 3 * ex].Add(disk);
                _ = TowerCanvas.Children.Add(disk);
            }

            RedrawTower();

            RemainingMoves = "";         // To update properties binded to it
        }

        private void BackToStartPageButton_Click(object sender, RoutedEventArgs e)
        {
            playCTS.Cancel();
            Moves = null;
            NavigationService.GoBack();
            Content = null;
            GC.Collect();
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AnimationSpeed = SpeedSlider.Value;
            animationTime = maxAnimationDuration -
                    (Math.Log10(AnimationSpeed) * (maxAnimationDuration - minAnimationDuration) / 2);
        }

        private async void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPauseButton();

            // If CancellationToken is true, it means play isn't running
            if (playCTS.IsCancellationRequested)
            {
                FastReverseButton.IsEnabled = false;
                PrevMoveButton.IsEnabled = false;
                NextMoveButton.IsEnabled = false;
                FastForwardButton.IsEnabled = false;

                playCTS = new CancellationTokenSource();
                Task result = PlayAsync(playCTS.Token);
                try
                {
                    await result;
                }
                catch (OperationCanceledException) { }

                playCTS.Cancel();                       // To indicate play isn't running
                ShowPlayButton();
                MovesCursor = MovesCursor;              // Update buttons
                ControlsGrid.IsEnabled = true;
            }
            else
            {
                playCTS.Cancel();
                ControlsGrid.IsEnabled = false;
            }
        }

        private async void NextMoveButton_Click(object sender, RoutedEventArgs e)
        {
            ControlsGrid.IsEnabled = false;
            await PerformMoveAsync(Moves[MovesCursor++]);
            ControlsGrid.IsEnabled = true;
            RemainingMoves = "";         // To update properties binded to it
        }

        private async void PrevMoveButton_Click(object sender, RoutedEventArgs e)
        {
            ControlsGrid.IsEnabled = false;
            await PerformMoveAsync(Move.Reverse(Moves[--MovesCursor]));
            ControlsGrid.IsEnabled = true;
            RemainingMoves = "";         // To update properties binded to it
        }

        private async void FastForwardButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPauseButton();

            FastReverseButton.IsEnabled = false;
            PrevMoveButton.IsEnabled = false;
            PlayPauseButton.IsEnabled = true;
            NextMoveButton.IsEnabled = false;
            FastForwardButton.IsEnabled = false;

            playCTS = new CancellationTokenSource();
            await Task.Run(async () =>
            {
                while (_movesCursor < totalMoves && !playCTS.Token.IsCancellationRequested)
                {
                    /* Used cursor as field instead of property to:
                     * -Prevent unnecessary buttons update
                     * -Be able to access cursor from new thread
                     */
                    await PerformMoveAsync(Moves[_movesCursor++], false);
                }
            });

            RedrawTower();
            ShowPlayButton();
            MovesCursor = MovesCursor;  // To update buttons status
            ControlsGrid.IsEnabled = true;
        }

        private async void FastReverseButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPauseButton();

            FastReverseButton.IsEnabled = false;
            PrevMoveButton.IsEnabled = false;
            PlayPauseButton.IsEnabled = true;
            NextMoveButton.IsEnabled = false;
            FastForwardButton.IsEnabled = false;

            playCTS = new CancellationTokenSource();
            await Task.Run(async () =>
            {
                while (_movesCursor > 0 && !playCTS.Token.IsCancellationRequested)
                {
                    /* Used cursor as field instead of property to:
                     * -Prevent unnecessary buttons update
                     * -Be able to access cursor from new thread
                     */
                    await PerformMoveAsync(Move.Reverse(Moves[--_movesCursor]), false);
                }
            });

            RedrawTower();
            ShowPlayButton();
            MovesCursor = MovesCursor;  // To update buttons status
            ControlsGrid.IsEnabled = true;
        }

        /// <summary>
        /// Changes PlayPauseButton to Play mode
        /// </summary>
        private void ShowPlayButton()
        {
            PlayPauseButton.Content = "\u25B6";
            PlayPauseButton.ToolTip = "Play";
        }

        /// <summary>
        /// Changes PlayPauseButton to Pause mode
        /// </summary>
        private void ShowPauseButton()
        {
            PlayPauseButton.Content = "\u23F8";
            PlayPauseButton.ToolTip = "Pause";
        }

        /// <summary>
        /// Performs moves in <c>Moves</c> list until it is stopped or reached the end.
        /// </summary>
        /// <param name="cancellationToken">a <c>CancellationToken</c> to stop the task when needed.</param>
        /// <returns>A <c>Task</c> object.</returns>
        private async Task PlayAsync(CancellationToken cancellationToken)
        {
            while (_movesCursor < totalMoves)
            {
                await PerformMoveAsync(Moves[_movesCursor++]);
                RemainingMoves = "";         // To update properties binded to it
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        /// <summary>
        /// Performs move of a <c>Move</c> object.
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
                TimeSpan at = TimeSpan.FromMilliseconds(animationTime);

                await AnimateAsync(DisksMoveBottom, disk, Canvas.BottomProperty, at);
                await AnimateAsync(disksCanvasLeft[dstNum], disk, Canvas.LeftProperty, at);
                await AnimateAsync(DisksMinBottom + ((dst.Count - 1) * DisksHeight), disk, Canvas.BottomProperty, at);
            }

            RemainingMoves = "";         // To update properties binded to it
        }

        /// <summary>
        /// Performs a method animatedly.
        /// </summary>
        /// <param name="start">The initial value.</param>
        /// <param name="end">The final value.</param>
        /// <param name="method">The method to perform.</param>
        /// <param name="time">The time animation should take in milliseconds.</param>
        /// <returns>A <c>Task</c> object.</returns>
        private async Task AnimateAsync(double end, UIElement element, DependencyProperty property, TimeSpan time)
        {
            if (time.Ticks <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            TaskCompletionSource<bool> acs = new TaskCompletionSource<bool>();

            Storyboard storyboard = new Storyboard()
            {
                FillBehavior = FillBehavior.Stop
            };

            DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();
            _ = animation.KeyFrames.Add(new EasingDoubleKeyFrame(end, KeyTime.FromTimeSpan(time), new SineEase()));
            storyboard.Children.Add(animation);

            Storyboard.SetTarget(storyboard, element);
            Storyboard.SetTargetProperty(storyboard, new PropertyPath(property));
            storyboard.Completed += (sender, e) =>
            {
                acs.SetResult(true);
            };

            storyboard.Begin();

            _ = await acs.Task;

            element.SetValue(property, end);
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
                    Canvas.SetBottom(disk, DisksMinBottom + (j * DisksHeight));
                    Canvas.SetLeft(disk, disksCanvasLeft[i]);
                }
            }
        }
    }
}