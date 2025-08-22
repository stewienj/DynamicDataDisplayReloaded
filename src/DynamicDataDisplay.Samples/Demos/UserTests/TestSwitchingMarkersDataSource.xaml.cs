using DynamicDataDisplay.Charts.Markers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DynamicDataDisplay.Samples.Demos.UserTest
{ 
    /// <summary>
    /// Interaction logic for DifferentBuildInMarkersPage.xaml
    /// </summary>
    public partial class TestSwitchingMarkersDataSource : Page
    {
        /// <summary>
        /// Maximum number of markers to display in a line
        /// </summary>
        private const int _maxCount = 50;
        /// <summary>
        /// Minimum number of markers to display in a line
        /// </summary>
        private const int _minCount = 2;
        /// <summary>
        /// Current number of markers to display in a line
        /// </summary>
        private int _count = 1;
        /// <summary>
        /// Used to increase or decrease the number of markers
        /// </summary>
        private int _countDirection = 1;
        /// <summary>
        /// Time we started in CPU ticks, used for getting a relative time
        /// </summary>
        private int _startTime;
        /// <summary>
        /// Timer used to update the markers
        /// </summary>
        private DispatcherTimer _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
        /// <summary>
        /// A list of chart controls that we are updating
        /// </summary>
        private List<DevMarkerChart> _charts = new List<DevMarkerChart>();

        public TestSwitchingMarkersDataSource()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        /// <summary>
        /// Called when the control is loaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _startTime = Environment.TickCount;
            _charts.AddMany([
                chart1,
                chart2,
                chart3,
                chart4,
                chart5,
                chart6,
                chart7,
                chart8]);

            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _timer.Tick -= Timer_Tick;
            _timer.Stop();
        }

        /// <summary>
        /// Periodic chart updater
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_count >= _maxCount)
            {
                _countDirection = -1;
            }
            else if (_count <= _minCount)
            {
                _countDirection = 1;
            }
            _count += _countDirection;

            for (int i = 0; i < _charts.Count; i++)
            {
                _charts[i].ItemsSource = CreateCollection(i+1);
            }
        }

        /// <summary>
        /// Creates an immutable collection of points from the current time, and data source number
        /// </summary>
        private ImmutableList<Point> CreateCollection(int dataSourceNumber)
        {
            int time = Environment.TickCount;

            var points = new Point[_count];

            for (int i = 0; i < _count; i++)
            {
                double x = i / (double)_count;
                points[i] = new Point(x, 0.1 * dataSourceNumber + 0.06 * Math.Sin(10 * x + Math.Sqrt(dataSourceNumber + 1) * 0.0005 * (time - _startTime)));
            }

            return ImmutableList.Create(points);
        }
    }
}
