using SharpDX.Direct3D9;
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace DynamicDataDisplay.SharpDX9.Helpers
{
    // Using managed pool may cause textures not to appear.
    public static class BaseDxHelpers
    {
        // Create a blank texture.
        public static Texture CreateTexture(Device device, int width, int height)
        {
            return new Texture(
                device,
                width,
                height,
                1,
                Usage.None,
                Format.A8R8G8B8,
                Pool.Default);
        }

        // Create a texture from an input filestream.
        public static Texture TextureFromStream(Device device, Stream stream, int width, int height, int colorKey)
        {
            return Texture.FromStream(
                device,
                stream,
                width,
                height,
                1,
                Usage.None,
                Format.A8R8G8B8,
                Pool.Default,
                Filter.Point,
                Filter.Point,
                colorKey);
        }

        // Create a texture from an input file.
        public static Texture TextureFromFile(Device device, string fileName, int width, int height, int colorKey)
        {
            return Texture.FromFile(
                device,
                fileName,
                width,
                height,
                1,
                Usage.None,
                Format.A8R8G8B8,
                Pool.Default,
                Filter.Point,
                Filter.Point,
                colorKey);
        }

        // Create a texture from an example PNG filestream.
        public static Texture GetDVDImageTexture(Device device)
        {
            // Load the PNG file.
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\src\DynamicDataDisplay.SharpDX9\Textures\dvdreduced.png");
            if (File.Exists(path))
            {
                // Get the image to find its dimensions.
                var image = new BitmapImage(new Uri(path));
                using (Stream stream = File.OpenRead(path))
                {
                    return TextureFromStream(device, stream, (int)image.Width, (int)image.Height, System.Drawing.Color.Transparent.ToArgb());
                }
            }

            return null;
        }
    }
}
