using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D9;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.SlimDX
{
	public abstract class SlimDXChart : FrameworkElement, IPlotterElement
	{
		private SlimDXHost dxHost;

		protected SlimDXHost DxHost
		{
			get { return dxHost; }
		}

		protected Device Device
		{
			get { return dxHost != null ? dxHost.Device : null; }
		}

		protected Direct3D Direct3D
		{
			get { return dxHost.Direct3D; }
		}

		private void OnDirectXRender(object sender, EventArgs e)
		{
			OnDirectXRender();
		}

		protected virtual void OnDirectXRender() { }

		#region IPlotterElement Members

		private Plotter2D plotter;
		protected Plotter2D Plotter
		{
			get { return plotter; }
		}

		public virtual void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			dxHost = this.plotter.Children.OfType<SlimDXHost>().FirstOrDefault();
			if (dxHost == null)
				throw new InvalidOperationException("First add DirectXHost to plotter.Children");

			dxHost.AddChild(this);
			dxHost.Render += OnDirectXRender;
		}

		public virtual void OnPlotterDetaching(Plotter plotter)
		{
			dxHost.RemoveChild(this);
			dxHost.Render -= OnDirectXRender;
			this.plotter = null;
		}

		Plotter IPlotterElement.Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}
