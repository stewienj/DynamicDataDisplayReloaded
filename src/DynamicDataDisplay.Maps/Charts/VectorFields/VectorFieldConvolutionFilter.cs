namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public abstract class VectorFieldConvolutionFilter
	{
		public abstract void Filter(int[] pixels, int width, int height);
	}
}
