﻿using DynamicDataDisplay.ViewModelTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	/// Interaction logic for DxLineMultiColorSample.xaml
	/// </summary>
	public partial class DxLineMultiColorSample : Page
	{
		public DxLineMultiColorSample()
		{
			var viewModel = new DxLineMultiColorViewModel();
			this.DataContext = viewModel;
			IsVisibleChanged += (s, e) =>
			{
				if (!IsVisible)
				{
					viewModel.Dispose();
				}
			};
			InitializeComponent();

		}
	}

	public class DxLineMultiColorViewModel : INotifyPropertyChanged
	{
		int pointCount = 10_000;
		double scaler = 1.0 / 10_000;
		public volatile bool _hasBeenDisposed = false;

		public DxLineMultiColorViewModel()
		{
			StartCalculatingPoints1();
			StartCalculatingPoints2();
			StartCalculatingPoints3();
		}

		private void StartCalculatingPoints1()
		{
			Task.Factory.StartNew(()=> 
			{
				DateTime startTime = DateTime.Now;
				while (!_hasBeenDisposed)
				{
					float inversePointMax = 1f / (pointCount - 1);
					double phase = -2.0 * Math.PI * (startTime - DateTime.Now).TotalSeconds / 20.0;

					Points1 =
						Enumerable.Range(0, pointCount)
						.Select(i =>
						{
							float x = (float)(2.0 * Math.PI * i * scaler);
							float y = (float)(15 * Math.Sin(x + phase) * Math.Sin(x * 101.0));
							float colorIndex = (y + 15f) / 5f % 1f;
							byte colorIndexByte = (byte)(Math.Round(colorIndex * 255));
							return new D3PointAndColor(x, y, Color.FromRgb(0, colorIndexByte, (byte)(255- colorIndexByte)));
						}).ToArray();
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Points1)));
					Thread.Sleep(10);
				}
			}, TaskCreationOptions.LongRunning);
		}

		private void StartCalculatingPoints2()
		{
			Task.Factory.StartNew(() =>
			{
				DateTime startTime = DateTime.Now;
				while (!_hasBeenDisposed)
				{
					float inversePointMax = 1f / (pointCount - 1);
					double phase = 2.0 * Math.PI * (startTime - DateTime.Now).TotalSeconds / 10.0;

					Points2 =
						Enumerable.Range(0, pointCount)
						.Select(i =>
						{
							float x = (float)(2.0 * Math.PI * i * scaler);
							float y = (float)(10*Math.Sin(x + phase) * Math.Sin(x * 102.0));
							float colorIndex = (y + 10f) / 5f % 1f;
							byte colorIndexByte = (byte)(Math.Round(colorIndex * 255));
							return new D3PointAndColor(x, y, Color.FromRgb(colorIndexByte, (byte)(255 - colorIndexByte), 0));
						}).ToArray();
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Points2)));
					Thread.Sleep(10);
				}
			}, TaskCreationOptions.LongRunning);
		}
		private void StartCalculatingPoints3()
		{
			Task.Factory.StartNew(() =>
			{
				DateTime startTime = DateTime.Now;
				while (!_hasBeenDisposed)
				{
					float inversePointMax = 1f / (pointCount - 1);
					double phase = 2.0 * Math.PI * (startTime - DateTime.Now).TotalSeconds / 5.0;

					Points3 =
						Enumerable.Range(0, pointCount)
						.Select(i =>
						{
							float x = (float)(2.0 * Math.PI * i * scaler);
							float y = (float)(20.0 * Math.Sin(x) * Math.Sin(x * 103.0) * Math.Sin(phase));
							float colorIndex = (y + 20f) / 5f % 1f;
							byte colorIndexByte = (byte)(Math.Round(colorIndex * 255));
							return new D3PointAndColor(x, y, Color.FromRgb((byte)(255 - colorIndexByte), 0, colorIndexByte));
						}).ToArray();
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Points3)));
					Thread.Sleep(10);
				}
			}, TaskCreationOptions.LongRunning);
		}
		public void Dispose()
		{
			_hasBeenDisposed = true;
		}

		public IEnumerable<D3PointAndColor> Points1 { get; set; }
		public IEnumerable<D3PointAndColor> Points2 { get; set; }
		public IEnumerable<D3PointAndColor> Points3 { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
