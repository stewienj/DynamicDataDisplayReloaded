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
        private const int _maxCount = 50;
        private const int _minCount = 2;
        private int _count = 1;
        private int _countDirection = 1;
        private int _startTime;
        private DispatcherTimer _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
        private List<DevMarkerChart> _charts = new List<DevMarkerChart>();

        public TestSwitchingMarkersDataSource()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }


        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _startTime = Environment.TickCount;

            _charts.AddMany
            ([
                chart1,
                chart2,
                chart3,
                chart4,
                chart5,
                chart6,
                chart7,
                chart8,
            ]);

            _timer.Tick += timer_Tick;
            _timer.Start();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _timer.Tick -= timer_Tick;
            _timer.Stop();
        }

        void timer_Tick(object sender, EventArgs e)
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
