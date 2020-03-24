using SharpDX.Direct3D9;
using System;
using System.Linq;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.SharpDX
{
	public abstract class SharpDXChart : FrameworkElement, IPlotterElement
	{
		private SharpDXHost dxHost;

		protected SharpDXHost DxHost
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
			dxHost = this.plotter.Children.OfType<SharpDXHost>().FirstOrDefault();
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
