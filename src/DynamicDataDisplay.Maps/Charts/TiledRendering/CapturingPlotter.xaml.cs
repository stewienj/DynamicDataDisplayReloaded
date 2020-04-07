namespace DynamicDataDisplay.Maps.Charts
{
	/// <summary>
	/// Represents a plotter with now extra place around it, e.g. without Left, Right, Top, Bottom panels and without
	/// Footer panel and Header panel. Used in tiled rendering.
	/// </summary>
	public partial class CapturingPlotter : Plotter2D
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CapturingPlotter"/> class.
		/// </summary>
		public CapturingPlotter()
		{
			InitializeComponent();
		}
	}
}
