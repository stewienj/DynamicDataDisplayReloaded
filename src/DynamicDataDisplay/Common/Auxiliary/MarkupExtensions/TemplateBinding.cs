using System.Windows.Data;

namespace DynamicDataDisplay.MarkupExtensions
{
	public class TemplateBinding : Binding
	{
		public TemplateBinding()
		{
			RelativeSource = new RelativeSource { Mode = RelativeSourceMode.TemplatedParent };
		}

		public TemplateBinding(string propertyPath)
			: this()
		{
			Path = new System.Windows.PropertyPath(propertyPath);
		}
	}
}
