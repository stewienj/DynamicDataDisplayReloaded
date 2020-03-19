using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	/// <summary>
	/// Interaction logic for ScreenshotParametersControl.xaml
	/// </summary>
	public partial class ScreenshotParametersControl : UserControl
	{
		public ScreenshotParametersControl()
		{
			InitializeComponent();
		}

		public ScreenshotHelper.Parameters ScreenshotParameters
		{
			get { return (ScreenshotHelper.Parameters)GetValue(ScreenshotParametersProperty); }
			set { SetValue(ScreenshotParametersProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ScreenshotParameters.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ScreenshotParametersProperty =
			DependencyProperty.Register("ScreenshotParameters", typeof(ScreenshotHelper.Parameters), typeof(ScreenshotParametersControl), new PropertyMetadata(null));


		private void WidthHeightNotSet_Click(object sender, RoutedEventArgs e)
		{
			if (ScreenshotParameters != null)
			{
				ScreenshotParameters.Width = null;
				ScreenshotParameters.Height = null;
			}
		}

		private void WidthHeight640x480_Click(object sender, RoutedEventArgs e)
		{
			if (ScreenshotParameters != null)
			{
				ScreenshotParameters.Width = 640;
				ScreenshotParameters.Height = 480;
			}
		}

		private void WidthHeight1024x768_Click(object sender, RoutedEventArgs e)
		{
			if (ScreenshotParameters != null)
			{
				ScreenshotParameters.Width = 1024;
				ScreenshotParameters.Height = 768;
			}
		}

		private void WidthHeight1280x1024_Click(object sender, RoutedEventArgs e)
		{
			if (ScreenshotParameters != null)
			{
				ScreenshotParameters.Width = 1280;
				ScreenshotParameters.Height = 1024;
			}
		}

		private void WidthHeight1920x1080_Click(object sender, RoutedEventArgs e)
		{
			if (ScreenshotParameters != null)
			{
				ScreenshotParameters.Width = 1920;
				ScreenshotParameters.Height = 1080;
			}
		}

		private void DPI96_Click(object sender, RoutedEventArgs e)
		{
			if (ScreenshotParameters != null)
			{
				ScreenshotParameters.Dpi = 96;
			}
		}

		private void DPI150_Click(object sender, RoutedEventArgs e)
		{
			if (ScreenshotParameters != null)
			{
				ScreenshotParameters.Dpi = 150;
			}
		}

		private void DPI300_Click(object sender, RoutedEventArgs e)
		{
			if (ScreenshotParameters != null)
			{
				ScreenshotParameters.Dpi = 300;
			}
		}

		private void DPIScreen_Click(object sender, RoutedEventArgs e)
		{
			if (ScreenshotParameters != null)
			{
				ScreenshotParameters.Dpi = ScreenshotParameters.ScreenDpi;
			}
		}

	}
}
