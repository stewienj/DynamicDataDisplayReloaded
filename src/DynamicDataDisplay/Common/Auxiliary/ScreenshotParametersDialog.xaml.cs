using System.Windows;

namespace DynamicDataDisplay.Common.Auxiliary
{
	/// <summary>
	/// Interaction logic for ScreenshotParametersDialog.xaml
	/// </summary>
	public partial class ScreenshotParametersDialog : Window
	{

		public ScreenshotParametersDialog()
		{
			InitializeComponent();
		}

		public ScreenshotHelper.Parameters ScreenshotParameters
		{
			get;
			set;
		}


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

		private void OK_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
