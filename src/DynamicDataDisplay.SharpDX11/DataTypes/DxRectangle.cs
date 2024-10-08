namespace DynamicDataDisplay.SharpDX11.DataTypes
{
    // A regular quadrilateral in 2D space.
    public class DxRectangle
	{
        public DxRectangle(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

		public float X { get; set; }

		public float Y { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        public float Left => this.X;

        public float Top => this.Y;

        public float Right
        {
            get { return this.X + this.Width; }
            set { this.X = value - this.Width; }
        }

        public float Bottom
        {
            get { return this.Y - this.Height; }
            set { this.Y = value - this.Height; }
        }
    }
}
