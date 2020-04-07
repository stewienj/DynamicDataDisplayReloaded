using DynamicDataDisplay.Markers.MarkerGenerators;
using DynamicDataDisplay.Charts;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Shapes;

namespace DynamicDataDisplay.Markers
{
	/// <summary>
	/// Interaction logic for ForestDisplayGenerator.xaml
	/// </summary>
	public partial class SimpleForestDisplayGenerator : TemplateMarkerGenerator, ISupportInitialize
	{
		public SimpleForestDisplayGenerator()
		{
			InitializeComponent();
		}

		protected override FrameworkElement CreateMarkerCore(object dataItem)
		{
			FrameworkElement marker = base.CreateMarkerCore(dataItem);
			SetBindings(marker);
			return marker;
		}

		private void SetBindings(FrameworkElement marker)
		{
			ViewportPanel panel = marker as ViewportPanel;
			marker.SetBinding(ViewportPanel.ViewportBoundsProperty, boundsBinding);

			Ellipse crownEllipse = (Ellipse)panel.FindName("crownEllipse");
			crownEllipse.SetBinding(ViewportPanel.ViewportWidthProperty, crownWidthBinding);
			crownEllipse.SetBinding(ViewportPanel.ViewportHeightProperty, crownHeightBinding);
			crownEllipse.SetBinding(ViewportPanel.YProperty, crownYBinding);
			crownEllipse.SetBinding(ViewportPanel.XProperty, xBinding);
			crownEllipse.SetBinding(Ellipse.FillProperty, fillBinding);
			crownEllipse.SetBinding(Ellipse.StrokeProperty, strokeBinding);

			Rectangle trunkRect = (Rectangle)panel.FindName("trunkRect");

			trunkRect.SetBinding(ViewportPanel.XProperty, xBinding);
			trunkRect.SetBinding(ViewportPanel.ViewportWidthProperty, trunkWidthBinding);
			trunkRect.SetBinding(ViewportPanel.ViewportHeightProperty, trunkHeightBinding);
			trunkRect.SetBinding(Rectangle.FillProperty, fillBinding);
		}

		public string CrownWidthPath { get; set; }
		public string CrownHeightPath { get; set; }
		public string TreeSpeciesPath { get; set; }
		public string TrunkWidthPath { get; set; }
		public string TrunkHeightPath { get; set; }
		public string XPath { get; set; }
		public IValueConverter FillConverter { get; set; }
		public IValueConverter StrokeConverter { get; set; }

		#region ISupportInitialize Members

		void ISupportInitialize.BeginInit()
		{
			// do nothing
		}

		Binding crownWidthBinding;
		Binding crownHeightBinding;
		Binding crownYBinding;

		Binding xBinding;
		Binding fillBinding;
		Binding strokeBinding;

		Binding trunkHeightBinding;
		Binding trunkWidthBinding;

		MultiBinding boundsBinding;

		int endInitCounter = 2;
		void ISupportInitialize.EndInit()
		{
			endInitCounter--;
			if (endInitCounter == 0)
			{
				crownWidthBinding = new Binding(CrownWidthPath);
				crownHeightBinding = new Binding(CrownHeightPath);
				crownYBinding = new Binding(TrunkHeightPath);
				xBinding = new Binding(XPath);
				fillBinding = new Binding { Path = new PropertyPath(TreeSpeciesPath), Converter = FillConverter };
				strokeBinding = new Binding { Path = new PropertyPath(TreeSpeciesPath), Converter = StrokeConverter };

				trunkHeightBinding = new Binding(TrunkHeightPath);
				trunkWidthBinding = new Binding(TrunkWidthPath);

				boundsBinding = new MultiBinding();
				boundsBinding.Bindings.Add(crownHeightBinding);
				boundsBinding.Bindings.Add(trunkHeightBinding);
				boundsBinding.Bindings.Add(crownWidthBinding);
				boundsBinding.Bindings.Add(xBinding);
				boundsBinding.Converter = new ForestBoundsConverter();

				RaiseEvent(new RoutedEventArgs(LoadedEvent));
			}
		}

		#endregion
	}
}
