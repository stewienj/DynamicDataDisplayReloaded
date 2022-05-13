using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.ViewModelTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace DynamicDataDisplay.SamplesDX9.Demos.SharpDX
{
    /// <summary>
    /// Interaction logic for DxMultiThickLineTexturedSample.xaml
    /// </summary>
    public partial class DxMultiThickLineTexturedSample : Page
    {
        public DxMultiThickLineTexturedSample()
        {
            var viewModel = new DxMultiThickLineTexturedSampleViewModel();
            DataContext = viewModel;
            IsVisibleChanged += (s, e) =>
            {
                if (!IsVisible)
                {
                    viewModel.Dispose();
                }

                InitializeComponent();
            };

            Loaded += (s, e) =>
            {
                plotter.Visible = new DataRect(-.1, -.1, 1.2, 1.2);
            };
        }
    }

    /// <summary>
    /// This renders lines inside the cells of a grid
    /// </summary>
    public class DxMultiThickLineTexturedSampleViewModel : INotifyPropertyChanged, IDisposable
    {
        private static int _columnCount = 2;
        private static int _rowCount = 2;
        // 2 points per line, 1 line per cell
        private D3Point[] _points = new D3Point[_columnCount * _rowCount *2];
        public volatile bool _hasBeenDisposed = false;

        public DxMultiThickLineTexturedSampleViewModel()
        {
            StartArrayUpdate();
        }

        private void StartArrayUpdate()
        {
            Task.Factory.StartNew(() =>
            {
                double progress = 0.0;
                while (!_hasBeenDisposed)
                {
                    // Track the progress around the 4 sides of a rectangle, where
                    // progress is a floating point number from 0 to 4.
                    // Going clockwise convert progress to x,y coordinates.
                    (double cellX0, double cellY0) = progress switch
                    {
                        >= 0 and <= 1 => (0.0, progress), // Left side, bottom to top
                        <= 2 => (progress - 1.0, 1.0),    // Top side, left to right
                        <= 3 => (1.0, 3.0 - progress),    // Right side, top to bottom
                        <= 4 => (4.0 - progress, 0.0),    // Bottom side, right to left
                        _ => (0.0, 0.0)
                    };
                    // Draw a line through the center to the other side
                    (double cellX1, double cellY1) = (1.0 - cellX0, 1.0 - cellY0);

                    progress += 0.01;
                    progress = progress % 4.0;

                    double cellWidth = 1.0 / _columnCount;
                    double cellHeight = 1.0 / _rowCount;

                    for (int columnNo = 0; columnNo < _columnCount; columnNo++)
                    {
                        for (int rowNo = 0; rowNo < _rowCount; rowNo++)
                        {
                            double x0 = (columnNo + cellX0) * cellWidth;
                            double x1 = (columnNo + cellX1) * cellWidth;
                            double y0 = (rowNo + cellY0) * cellHeight;
                            double y1 = (rowNo + cellY1) * cellHeight;

                            _points[(columnNo * _rowCount + rowNo)*2] = new D3Point(x0, y0);
                            _points[(columnNo * _rowCount + rowNo) * 2+1] = new D3Point(x1, y1);
                        }
                    }

                    // Double buffer the current array
                    var temp = (Points as D3Point[]) ?? new D3Point[_columnCount * _rowCount * 2];
                    Points = _points;
                    _points = temp;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Points)));
                    Thread.Sleep(10);
                }
            });
        }

        public void Dispose()
        {
            _hasBeenDisposed = true;
        }

        public IEnumerable<D3Point> Points { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        
    }
}
