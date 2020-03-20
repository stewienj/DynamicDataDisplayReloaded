using DynamicDataDisplay.RadioBand.ConfigLoader;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace DynamicDataDisplay.RadioBand
{
	/// <summary>
	/// Interaction logic for FrequencyFilterSlider.xaml
	/// </summary>
	public partial class FrequencyFilterSlider : UserControl
	{
		// ************************************************************************
		// Private Fields
		// ************************************************************************
		#region Private Fields

		/// <summary>
		/// Type of mouse capture that is in effect if any.
		/// </summary>
		private enum MarkerCapturedType
		{
			None,
			Min,
			Max
		}

		private RadioBandTransform _transform = new RadioBandTransform(RadioBandPlotConfig.SpectrumDisplayDefault);

		/// <summary>
		/// Type of mouse capture that is in effect if any.
		/// </summary>
		private MarkerCapturedType _mouseCaptured = MarkerCapturedType.None;
		/// <summary>
		/// Cached calculation of the frequency to the slider position for the minimum indicator
		/// </summary>
		private double _minPos = 0;
		/// <summary>
		/// Cached calculation of the frequency to the slider position for the maximum indicator
		/// </summary>
		private double _maxPos = 1;

		#endregion Private Fields

		// ************************************************************************
		// Public Methods
		// ************************************************************************
		#region Public Methods

		/// <summary>
		/// Default Constructor
		/// </summary>
		public FrequencyFilterSlider()
		{
			InitializeComponent();
			LostMouseCapture += RadioBandSelector_LostMouseCapture;
			RedrawControl();
		}

		// Using a DependencyProperty as the backing store for MinimumFrequency.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MinimumFrequencyProperty =
			DependencyProperty.Register("MinimumFrequency", typeof(double), typeof(FrequencyFilterSlider), new PropertyMetadata(0.1E9,
			  (d, e) => ((FrequencyFilterSlider)d).UpdateFromExternal(),
			  // Coerce Value
			  (d, value) =>
			  {
				  FrequencyFilterSlider slider = (FrequencyFilterSlider)d;
				  double current = (double)value;
				  double min = slider._transform.ViewportToFrequency(0);
				  return Math.Max(current, min);
			  }
		   ));


		// Using a DependencyProperty as the backing store for MaximumFrequency.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MaximumFrequencyProperty =
			DependencyProperty.Register("MaximumFrequency", typeof(double), typeof(FrequencyFilterSlider), new PropertyMetadata(100E9,
			  (d, e) => ((FrequencyFilterSlider)d).UpdateFromExternal(),
			  // Coerce Value
			  (d, value) =>
			  {
				  FrequencyFilterSlider slider = (FrequencyFilterSlider)d;
				  double current = (double)value;
				  double max = slider._transform.ViewportToFrequency(1);
				  return Math.Min(current, max);
			  }
		   ));


		#endregion Public Methods

		// ************************************************************************
		// Private Methods
		// ************************************************************************
		#region Private Methods

		/// <summary>
		/// Updates the cached min max positions of the sliders, and redraws the control
		/// </summary>
		private void UpdateFromExternal()
		{
			_minPos = _transform.FrequencyToViewport(MinimumFrequency);
			_maxPos = _transform.FrequencyToViewport(MaximumFrequency);
			RedrawControl();
		}

		/// <summary>
		/// Gets which slider the mouse point is nearest to
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		private MarkerCapturedType NearestMarker(Point point)
		{
			double minStart = _minPos * _canvasUserSelection.ActualWidth - 4;
			double minEnd = _minPos * _canvasUserSelection.ActualWidth + ArrowHeight * 0.5;
			bool nearMin = point.X > minStart && point.X < minEnd;

			double maxStart = _maxPos * _canvasUserSelection.ActualWidth - ArrowHeight * 0.5;
			double maxEnd = _maxPos * _canvasUserSelection.ActualWidth + 4;
			bool nearMax = point.X > maxStart && point.X < maxEnd;

			if (nearMax && nearMin)
				return point.X >= (minStart + maxStart) * 0.5 ? MarkerCapturedType.Max : MarkerCapturedType.Min;
			else if (nearMin)
				return MarkerCapturedType.Min;
			else if (nearMax)
				return MarkerCapturedType.Max;
			else
				return MarkerCapturedType.None;
		}

		public static double SpeedOfLight = 299792458; // Meters per second
		private string HzTom(double frequency) => (SpeedOfLight / frequency).ToString();

		/// <summary>
		/// Clamps the value passed in to between 0 and 1
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private double Limit0To1(double value) => Math.Max(0, Math.Min(1, value));

		private double ArrowHeight => _labelFrequency.ActualHeight;

		/// <summary>
		/// Redraws the control
		/// </summary>
		private void RedrawControl()
		{
			_labelFrequency.Text = $"Frequency Range: {MinimumFrequency.ToString().ToEngineeringNotation(3)}Hz - {MaximumFrequency.ToString().ToEngineeringNotation(3)}Hz";
			_labelWavelength.Text = $"Wavelength: {HzTom(MinimumFrequency).ToEngineeringNotation(3)}m - {HzTom(MaximumFrequency).ToEngineeringNotation(3)}m";

			_canvasMin.Width = Limit0To1(_minPos) * _canvasUserSelection.ActualWidth + 1;
			_canvasMax.Width = (1.0 - Limit0To1(_maxPos)) * _canvasUserSelection.ActualWidth + 1;


			double height = _canvasUserSelection.ActualHeight - ArrowHeight;

			DoArrow(_polygonMinTop, new Point(_canvasMin.Width, 0), false);
			DoArrow(_polygonMinBottom, new Point(_canvasMin.Width, height), false);

			DoArrow(_polygonMaxTop, new Point(0, 0), true);
			DoArrow(_polygonMaxBottom, new Point(0, height), true);
		}

		/// <summary>                      --\
		/// Creates an arrow shaped polygon   \
		/// With line at the back             /
		/// </summary>                     --/
		/// <param name="ploygon"></param>
		/// <param name="offset"></param>
		/// <param name="flip"></param>
		private void DoArrow(Polygon ploygon, Point offset, bool flip)
		{
			double multiplier = flip ? -1 : 1;
			double arrowHeight = ArrowHeight;

			// Draws arrow with line at back starting top left, +Y is down
			ploygon.Points.Clear();
			ploygon.Points.Add(new Point(offset.X - multiplier * 0.5, offset.Y));
			ploygon.Points.Add(offset);
			ploygon.Points.Add(new Point(offset.X + multiplier * arrowHeight * 0.5, offset.Y + arrowHeight * 0.5));
			ploygon.Points.Add(new Point(offset.X, offset.Y + arrowHeight));
			ploygon.Points.Add(new Point(offset.X - multiplier * 0.5, offset.Y + arrowHeight));
		}

		#endregion Private Methods

		// ************************************************************************
		// Properties
		// ************************************************************************
		#region Properties

		/// <summary>
		/// Gets/Sets the minimum slider position
		/// </summary>
		public double MinimumFrequency
		{
			get => (double)GetValue(MinimumFrequencyProperty);
			set => SetValue(MinimumFrequencyProperty, value);
		}

		/// <summary>
		/// Gets/sets the maximum slider position
		/// </summary>
		public double MaximumFrequency
		{
			get => (double)GetValue(MaximumFrequencyProperty);
			set => SetValue(MaximumFrequencyProperty, value);
		}

		#endregion Properties

		// ************************************************************************
		// Events And Event Handlers
		// ************************************************************************
		#region Events And Event Handlers

		/// <summary>
		/// Handles when mouse capture is lost
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RadioBandSelector_LostMouseCapture(object sender, MouseEventArgs e)
		{
			_mouseCaptured = MarkerCapturedType.None;
			Mouse.OverrideCursor = null;
		}

		/// <summary>
		/// Handles when the mouse moves, sets the cursor if required, and moves the slider
		/// controls if the user has the left mouse button down.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _canvasUserSelection_MouseMove(object sender, MouseEventArgs e)
		{
			//Mouse.
			Point point = e.GetPosition(_canvasUserSelection);
			if (_mouseCaptured != MarkerCapturedType.None)
			{
				// We are moving one of the lines
				double newPos = Limit0To1(point.X / _canvasUserSelection.ActualWidth);
				double minPos = newPos, maxPos = newPos;
				switch (_mouseCaptured)
				{
					case MarkerCapturedType.Min:
						maxPos = Math.Max(newPos, _maxPos);
						break;
					case MarkerCapturedType.Max:
						minPos = Math.Min(newPos, _minPos);
						break;
				}
				MinimumFrequency = _transform.ViewportToFrequency(minPos);
				MaximumFrequency = _transform.ViewportToFrequency(maxPos);
			}
			else
			{
				MarkerCapturedType nearestMarker = NearestMarker(point);
				Mouse.SetCursor(nearestMarker == MarkerCapturedType.None ? Cursors.Arrow : Cursors.SizeWE);
			}
		}

		/// <summary>
		/// Captures the mouse when the user holds down the left mouse button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _canvasUserSelection_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (_mouseCaptured == MarkerCapturedType.None)
			{
				// Capture mouse, record mouse click location, moves are relative from this
				_mouseCaptured = NearestMarker(e.GetPosition(_canvasUserSelection));
				if (_mouseCaptured != MarkerCapturedType.None)
				{
					Mouse.Capture(this, CaptureMode.Element);
					// For some reason I have to use the override cursor
					Mouse.OverrideCursor = Cursors.SizeWE;
				}
			}
		}

		/// <summary>
		/// Releases mouse capture when the user releases the left mouse button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _canvasUserSelection_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			// Release everthing
			Mouse.Capture(null);
			Mouse.OverrideCursor = null;
			_mouseCaptured = MarkerCapturedType.None;
		}

		/// <summary>
		/// Redraws the control when the control area resizes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _canvasUserSelection_SizeChanged(object sender, SizeChangedEventArgs e) => RedrawControl();

		#endregion Events And Event Handlers
	}
}
