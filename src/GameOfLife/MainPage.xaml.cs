using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;


namespace GameOfLife
{

    public sealed partial class MainPage : Page
    {

        private static Brush ColorLifeStable { get; set; } = new SolidColorBrush();
        private static Brush ColorSurround { get; set; } = new SolidColorBrush();


        private Timer dispatcherTimer;


        //private Coord2D Amount = new Coord2D();
        private GolBoard Board = new GolBoard();


        public Dictionary<Coord2D, Rectangle> Squares = new Dictionary<Coord2D, Rectangle>();

        private Dictionary<Coord2D, Status> GenStart = new Dictionary<Coord2D, Status>();

        private Dictionary<Coord2D, Status>[] GenMinus = new Dictionary<Coord2D, Status>[3];


        #region Property: (Dictionary<Coord2D, Status>) Lcells

        public Dictionary<Coord2D, Status> Lcells
        {
            get => (Dictionary<Coord2D, Status>)GetValue(LcellsProperty);
            set => SetValue(LcellsProperty, value);
        }

        private static readonly DependencyProperty LcellsProperty =
            DependencyProperty.Register(
                nameof(Lcells),
                typeof(Dictionary<Coord2D, Status>),
                typeof(MainPage),
                new PropertyMetadata(new Dictionary<Coord2D, Status>())
                );

        #endregion

        #region Property: (int) LcellsCount

        public int LcellsCount
        {
            get => (int)GetValue(LcellsCountProperty);
            set => SetValue(LcellsCountProperty, value);
        }

        private static readonly DependencyProperty LcellsCountProperty =
            DependencyProperty.Register(
                nameof(LcellsCount),
                typeof(int),
                typeof(MainPage),
                new PropertyMetadata(default)
                );

        #endregion

        #region Property: (int) LcellsPercent

        public double LcellsPercent
        {
            get => (double)GetValue(LcellsPercentProperty);
            set => SetValue(LcellsPercentProperty, value);
        }

        private static readonly DependencyProperty LcellsPercentProperty =
            DependencyProperty.Register(
                nameof(LcellsPercent),
                typeof(double),
                typeof(MainPage),
                new PropertyMetadata(default)
                );

        #endregion


        #region Property: (Dictionary<Coord2D, Status>) Lcells

        public Dictionary<Coord2D, Status> Scells
        {
            get => (Dictionary<Coord2D, Status>)GetValue(ScellsProperty);
            set => SetValue(ScellsProperty, value);
        }

        private static readonly DependencyProperty ScellsProperty =
            DependencyProperty.Register(
                nameof(Scells),
                typeof(Dictionary<Coord2D, Status>),
                typeof(MainPage),
                new PropertyMetadata(new Dictionary<Coord2D, Status>())
                );

        #endregion

        #region Property: (int) ScellsCount

        public int ScellsCount
        {
            get => (int)GetValue(ScellsCountProperty);
            set => SetValue(ScellsCountProperty, value);
        }

        private static readonly DependencyProperty ScellsCountProperty =
            DependencyProperty.Register(
                nameof(ScellsCount),
                typeof(int),
                typeof(MainPage),
                new PropertyMetadata(default)
                );

        #endregion

        #region Property: (int) ScellsPercent

        public double ScellsPercent
        {
            get => (double)GetValue(ScellsPercentProperty);
            set => SetValue(ScellsPercentProperty, value);
        }

        private static readonly DependencyProperty ScellsPercentProperty =
            DependencyProperty.Register(
                nameof(ScellsPercent),
                typeof(double),
                typeof(MainPage),
                new PropertyMetadata(default)
                );

        #endregion


        #region Property: (int) DcellsCount

        public int DcellsCount
        {
            get => (int)GetValue(DcellsCountProperty);
            set => SetValue(DcellsCountProperty, value);
        }

        private static readonly DependencyProperty DcellsCountProperty =
            DependencyProperty.Register(
                nameof(DcellsCount),
                typeof(int),
                typeof(MainPage),
                new PropertyMetadata(default)
                );

        #endregion

        #region Property: (int) DcellsPercent

        public double DcellsPercent
        {
            get => (double)GetValue(DcellsPercentProperty);
            set => SetValue(DcellsPercentProperty, value);
        }

        private static readonly DependencyProperty DcellsPercentProperty =
            DependencyProperty.Register(
                nameof(DcellsPercent),
                typeof(double),
                typeof(MainPage),
                new PropertyMetadata(default)
                );

        #endregion


        #region Property: (int) Generation

        public int Generation
        {
            get => (int)GetValue(GenerationProperty);
            set => SetValue(GenerationProperty, value);
        }

        private static readonly DependencyProperty GenerationProperty =
            DependencyProperty.Register(
                nameof(Generation),
                typeof(int),
                typeof(MainPage),
                new PropertyMetadata(default)
                );

        #endregion

        #region Property: (string) Info

        public string Info
        {
            get => (string)GetValue(InfoProperty);
            set => SetValue(InfoProperty, value);
        }

        private static readonly DependencyProperty InfoProperty =
            DependencyProperty.Register(
                nameof(Info),
                typeof(string),
                typeof(MainPage),
                new PropertyMetadata(default)
                );

        #endregion


        bool timerIsRunning = false;
        bool timerIsWorking = false;
        private void calldispatcherTimer(bool startState = false)
        {
            try
            {
                dispatcherTimer?.Dispose();
                timerIsWorking = false;
                timerSlider.IsEnabled = !startState;
                BtnFill.IsEnabled = !startState;
                BtnSet.IsEnabled = !startState;
                NextButton.IsEnabled = !startState;

                if (startState)
                {
                    dispatcherTimer = new System.Threading.Timer(async delegate
                    {
                        if (!timerIsWorking)
                        {
                            await CoreApplication.MainView.CoreWindow.Dispatcher.TryRunAsync(CoreDispatcherPriority.High, () =>
                            {
                                timerIsWorking = true;
                                TimerTick();
                                timerIsWorking = false;
                            });
                        }
                    }, null, 0, (int)timerSlider.Value);
                }
                timerIsRunning = startState;
            }
            catch (Exception e)
            {

            }
        }
        public MainPage()
        {
            InitializeComponent();

            if (DeviceIsPhone())
            {
                ToggleFullScreen();
            }

            //dispatcherTimer.Interval = TimeSpan.FromMilliseconds(5);
            //dispatcherTimer.Tick += TimerTick;

            for (int i = 0; i < GenMinus.Length; i++)
            {
                GenMinus[i] = new Dictionary<Coord2D, Status>();
            }

            ColorLifeStable = GolBrush.Life;
            ColorSurround = GolBrush.Dead;
            Playarea.PointerReleased += MainPage_PointerReleased;
            Playarea.PointerPressed += MainPage_PointerPressed;
            BuildCellsDelay();
        }
        private async void BuildCellsDelay()
        {
            BtnSet.IsEnabled = false;
            BtnFill.IsEnabled = false;
            GenerateStateText.Text = "Please wait...";
            GenerateStateContainer.Visibility = Visibility.Visible;
            await Task.Delay(1500);
            await BuildCells();
        }
        private void MainPage_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            pointerPressed = true;
            SettingsPage.Visibility = Visibility.Collapsed;
        }

        private void MainPage_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            pointerPressed = false;
        }

        private void TimerTick(object sender, object e)
        {
            NextGeneration();
        }
        private void TimerTick()
        {
            NextGeneration();
        }

        #region UI: BUTTON

        private async void BtnSet_Click(object sender, RoutedEventArgs e)
        {
            await BuildCells();
        }
        private async Task BuildCells()
        {
            BtnSet.IsEnabled = false;
            BtnFill.IsEnabled = false;
            GenerateStateText.Text = "Generating...";
            GenerateStateContainer.Visibility = Visibility.Visible;

            Playarea.Children.Clear();

            Squares.Clear();

            //PlayAreaZoom.ChangeView(0d, 0d, 1.0f);
            PlayAreaZoom.UpdateLayout();

            double distance = 0.75;
            _ = int.TryParse(ElemWidth.Text, out int pxWidth);
            double pxdistWidth = pxWidth + distance; ;

            var bounds = Window.Current.Bounds;
            var trueHeight = bounds.Height - (TopBarGrid.ActualHeight + BottomBarGrid.ActualHeight);
            var trueWidth = bounds.Width;

            Board.SetBoardWidth((byte)(trueWidth / pxdistWidth));
            Board.SetBoardHeight((byte)(trueHeight / pxdistWidth));

            Info = $"Area: {Board.AmountOfCells} cells";
            ElemTotal.Text = $"Total: {Board.AmountOfCells}";

            await Task.Delay(1);

            var totalCreated = 0;
            for (byte x = 0; x < Board.Amount.X; x++)
            {
                for (byte y = 0; y < Board.Amount.Y; y++)
                {
                    await CreateCell(pxWidth, x, y, pxdistWidth, distance);
                    totalCreated++;
                    GenerateStateText.Text = $"Generated: {totalCreated}";
                    if (DrawCellsState.IsOn)
                    {
                        await NOP(0.0001);
                    }
                }
                if (DrawCellsState.IsOn)
                {
                    await NOP(0.0001);
                }
            }

            /*Parallel.For(0, Board.Amount.X - 1, x =>
            {
                Parallel.For(0, Board.Amount.Y - 1, y =>
                {
                });
            });*/
            //Reset cells
            SetDead(Squares.Keys.ToArray());
            NextGeneration();
            Generation = 0;

            ColorizeGeneration();
            BtnSet.IsEnabled = true;
            BtnFill.IsEnabled = true;
            GenerateStateContainer.Visibility = Visibility.Collapsed;
        }
        private async Task NOP(double durationSeconds)
        {
            var durationTicks = Math.Round(durationSeconds * Stopwatch.Frequency);
            var sw = Stopwatch.StartNew();
            TaskCompletionSource<bool> timeout = new TaskCompletionSource<bool>();
            await Task.Run(async () =>
            {
                while (sw.ElapsedTicks < durationTicks)
                {
                    await Task.Delay(0);
                }
                timeout.SetResult(true);
            });

            await timeout.Task;
        }
        private async Task CreateCell(int pxWidth, int x, int y, double pxdistWidth, double distance)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.TryRunAsync(CoreDispatcherPriority.High, () =>
            {
                Rectangle rect = new Rectangle()
                {
                    Width = pxWidth,
                    Height = pxWidth,
                    Fill = GolBrush.Dead
                };
                rect.PointerPressed += Rectangle_PointerPressed;
                //rect.PointerMoved += Rectangle_PointerMoved;
                lock (Squares)
                {
                    Squares.Add(new Coord2D(x, y), rect);
                }
                Playarea.Children.Add(rect);
                Canvas.SetLeft(rect, x * pxdistWidth + distance);
                Canvas.SetTop(rect, y * pxdistWidth + distance);
            });
        }


        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            SetDead(Squares.Keys.ToArray());
            NextGeneration();
            Generation = 0;
            pointerPressed = false;
        }

        private void BtnFill_Click(object sender, RoutedEventArgs e)
        {

            Generation = 0;
            pointerPressed = false;

            Lcells.Clear();
            Scells.Clear();


            IEnumerable<Coord2D> coord2Ds = Board.RandomCells(percentSlider.Value);

            foreach (Coord2D entry in coord2Ds)
            {
                Squares[entry].Fill = GolBrush.Life;
            }


            NextGeneration();
        }


        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            //if (dispatcherTimer.IsEnabled)
            if (timerIsRunning)
            {
                //dispatcherTimer.Stop();
                calldispatcherTimer(false);
                PlayButton.Icon = new SymbolIcon(Symbol.Play);
            }
            else
            {
                //dispatcherTimer.Start();
                calldispatcherTimer(true);
                PlayButton.Icon = new SymbolIcon(Symbol.Pause);
            }
            pointerPressed = false;
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            NextGeneration();
            pointerPressed = false;
        }

        #endregion

        #region UI: TOGGLE_SWITCH

        private void ToggleSwitchStable_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;

            //dispatcherTimer.Stop();
            calldispatcherTimer(false);
            PlayButton.Icon = new SymbolIcon(Symbol.Play);

            foreach (var entry in Squares)
            {
                if (Squares[entry.Key].Fill == GolBrush.Stable)
                {
                    Squares[entry.Key].Fill = GolBrush.Life;
                }
            }

            //dispatcherTimer.Start();

            ColorLifeStable = toggleSwitch.IsOn ? GolBrush.Stable : GolBrush.Life;
        }

        private void ToggleSwitchSurround_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;

            ColorSurround = toggleSwitch.IsOn ? GolBrush.Next : GolBrush.Dead;
        }

        bool pointerPressed = false;
        private void Rectangle_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (pointerPressed)
            {
                Rectangle rect = sender as Rectangle;

                rect.Fill = (rect.Fill == GolBrush.Dead) ? GolBrush.Life : GolBrush.Dead;
            }
        }
        private void Rectangle_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            {
                Rectangle rect = sender as Rectangle;

                rect.Fill = (rect.Fill == GolBrush.Dead) ? GolBrush.Life : GolBrush.Dead;
            }
        }

        #endregion

        public bool RandomColors
        {
            get
            {
                return GolBrush.RandomColors;
            }
            set
            {
                GolBrush.RandomColors = value;
            }
        }
        private void NextGeneration()
        {

            Generation++;


            if (Lcells.Count == 0)
            {

                GenStart.Clear();

                foreach (KeyValuePair<Coord2D, Rectangle> entry in Squares)
                {
                    if ((entry.Value.Fill != GolBrush.Dead && entry.Value.Fill != GolBrush.Next) || entry.Value.Fill == ColorLifeStable)
                    {
                        try
                        {
                            GenStart.Add(entry.Key, Status.Life);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    if (entry.Value.Fill == ColorSurround)
                    {
                        try
                        {
                            GenStart.Add(entry.Key, Status.Surround);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            else
            {
                GenStart = GolHelper.MergeLeft(Lcells, Scells);
            }


            GolHelper.Record(ref GenMinus, GenStart);

            GenerationStop(GenMinus);


            FindLivingCells(Lcells, GenStart.Keys);

            FindSurroundingCells(Scells, Lcells.Keys);


            ColorizeGeneration();
        }


        private void FindLivingCells(Dictionary<Coord2D, Status> alive, IEnumerable<Coord2D> coord2Ds)
        {

            alive.Clear();

            foreach (Coord2D coord2D in coord2Ds)
            {
                if (alive.ContainsKey(coord2D))
                {
                    continue;
                }

                if (IsCellAlive(coord2D))
                {
                    try
                    {
                        alive.Add(coord2D, Status.Life);

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        private bool IsCellAlive(Coord2D center)
        {

            bool alive = false;

            try
            {
                int neighbour = 0;


                foreach (Tuple<sbyte, sbyte> entry in GolHelper.NextCells)
                {

                    if (neighbour > 3)
                    {
                        break;
                    }

                    Coord2D coord2D = NeighbourCell(Board.Amount, center, entry.Item1, entry.Item2);

                    if ((Squares[coord2D].Fill != GolBrush.Dead && Squares[coord2D].Fill != GolBrush.Next) || Squares[coord2D].Fill == ColorLifeStable)
                    {
                        neighbour++;
                    }
                }


                if (neighbour == 3 || ((Squares[center].Fill != GolBrush.Dead && Squares[center].Fill != GolBrush.Next) && neighbour == 2) || (Squares[center].Fill == ColorLifeStable && neighbour == 2))
                {
                    alive = true;
                }
            }
            catch (Exception ex)
            {

            }

            return alive;
        }

        private void FindSurroundingCells(Dictionary<Coord2D, Status> surround, IEnumerable<Coord2D> coord2Ds)
        {

            surround.Clear();

            foreach (Coord2D center in coord2Ds)
            {
                foreach (Tuple<sbyte, sbyte> nx in GolHelper.NextCells)
                {
                    try
                    {
                        surround.Add(
                            NeighbourCell(Board.Amount, center, nx.Item1, nx.Item2),
                            Status.Surround
                            );
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }


        private void ColorizeGeneration()
        {

            SetDead(GenStart.Keys.ToArray());


            Dictionary<Coord2D, Status> mergedCells = GolHelper.MergeLeft(Lcells, Scells);

            foreach (KeyValuePair<Coord2D, Status> entry in mergedCells)
            {

                Brush brush = GetBrushFromStatus(entry.Value);

                //if (Squares[entry.Key].Fill != brush)
                //{
                //    Squares[entry.Key].Fill = brush;
                //}

                _ = GenStart.TryGetValue(entry.Key, out Status statusOld);

                if (statusOld == entry.Value && entry.Value == Status.Life)
                {
                    Squares[entry.Key].Fill = ColorLifeStable;
                }
                else
                {
                    Squares[entry.Key].Fill = brush;
                }
            }


            #region INFORMATION update

            int amount = Board.Amount.X * Board.Amount.Y;

            LcellsCount = Lcells.Count();

            LcellsPercent = Math.Round(LcellsCount / (double)amount * 100, 1);

            DcellsCount = amount - LcellsCount;

            DcellsPercent = Math.Round(DcellsCount / (double)amount * 100, 1);

            ScellsCount = mergedCells.Count(x => x.Value == Status.Surround);

            ScellsPercent = Math.Round(ScellsCount / (double)amount * 100, 1);

            #endregion
        }

        private void SetDead(IEnumerable<Coord2D> collection)
        {
            foreach (Coord2D entry in collection)
            {
                _ = Squares.TryGetValue(entry, out Rectangle rec);

                rec.Fill = GolBrush.Dead;
            }
        }


        private void GenerationStop(Dictionary<Coord2D, Status>[] genMinus, DispatcherTimer timer)
        {

            if (genMinus[2].SequenceEqual(genMinus[0]))
            {
                timer.Stop();
                PlayButton.Icon = new SymbolIcon(Symbol.Play);
            }
        }
        private void GenerationStop(Dictionary<Coord2D, Status>[] genMinus)
        {

            if (genMinus[2].SequenceEqual(genMinus[0]))
            {
                calldispatcherTimer(false);
                PlayButton.Icon = new SymbolIcon(Symbol.Play);
            }
        }


        private static Coord2D NeighbourCell(Coord2D amount, Coord2D center, sbyte nX, sbyte nY)
        {

            byte x = NeighbourCoordinate(center.X, nX, amount.X);

            byte y = NeighbourCoordinate(center.Y, nY, amount.Y);

            return new Coord2D(x, y);
        }

        private static byte NeighbourCoordinate(byte pt, sbyte dist, byte amount)
        {
            int ptdist = pt + dist;

            if (ptdist >= amount)
            {
                ptdist = 0;
            }

            if (ptdist < 0)
            {
                ptdist = amount - 1;
            }

            return (byte)ptdist;
        }


        private static Brush GetBrushFromStatus(Status status)
        {

            switch (status)
            {
                case Status.Life:
                    return GolBrush.Life;
                case Status.Surround:
                    return ColorSurround;
                case Status.Dead:
                    return GolBrush.Dead;
                default:
                    return GolBrush.Dead;
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsPage.Visibility = SettingsPage.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var messageDialog = new MessageDialog(("Do you want to exit?"));
            messageDialog.Commands.Add(new UICommand(("Yes"), new UICommandInvokedHandler(this.CommandInvokedHandler)));
            messageDialog.Commands.Add(new UICommand(("No")));
            await messageDialog.ShowAsync();
        }
        private async void GitHubButton_Click(object sender, RoutedEventArgs e)
        {
            var options = new Windows.System.LauncherOptions();
            options.PreferredApplicationPackageFamilyName = "Microsoft.MicrosoftEdge_8wekyb3d8bbwe";
            options.PreferredApplicationDisplayName = "Microsoft Edge";
            // Launch the URI
            var success = await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/basharast/GameOfLife"), options);

            if (success)
            {
                // URI launched
            }
        }
        private async void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            var messageDialog = new MessageDialog(("Current development by Bashar Astifan\nOriginally developed by ykSneerG\n\nhttps://github.com/basharast/GameOfLife"));
            messageDialog.Commands.Add(new UICommand(("Close")));
            await messageDialog.ShowAsync();
        }
        private void CommandInvokedHandler(IUICommand command)
        {
            CoreApplication.Exit();
        }

        bool fullScreenState = false;
        private void ToggleFullScreen()
        {
            ApplicationView applicationView = ApplicationView.GetForCurrentView();
            if (applicationView.IsFullScreenMode)
            {
                applicationView.ExitFullScreenMode();
                fullScreenState = false;
            }
            else
            {
                fullScreenState = applicationView.TryEnterFullScreenMode();
            }
            Bindings.Update();
        }
        public static bool DeviceIsPhone()
        {
            try
            {
                EasClientDeviceInformation info = new EasClientDeviceInformation();
                string system = info.OperatingSystem;
                if (system.Equals("WindowsPhone"))
                {
                    return true;
                }
            }
            catch (Exception e)
            {

            }
            return false;
        }

        private void FullScreenState_Click(object sender, RoutedEventArgs e)
        {
            ToggleFullScreen();
        }
        double XSCurrent;
        double YSCurrent;
        private void SensorsGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            try
            {
                XSCurrent += e.Delta.Translation.X;
                YSCurrent += e.Delta.Translation.Y;
                Bindings.Update();
            }
            catch (Exception ex)
            {

            }
        }
    }
}