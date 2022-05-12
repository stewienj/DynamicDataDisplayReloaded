using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DynamicDataDisplay.SharpDX9.Textures
{

    /// <summary>
    /// Used as a texture that can be shared.
    /// </summary>
    public class DxSharedTexture : DependencyObject
    {
        private BitmapSource _imageSource;
        private Texture _texture = null;

        public Texture GetTexture(Device device)
        {
            if (device == null || _imageSource == null)
            {
                return null;
            }

            if (_texture != null)
            {
                return _texture;
            }



            MemoryStream stream = new MemoryStream();
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(_imageSource));
            encoder.Save(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return TextureFromStream(device, stream, (int)_imageSource.Width, (int)_imageSource.Height, System.Drawing.Color.Transparent.ToArgb());
        }

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

        public BitmapSource TextureSource
        {
            get { return (BitmapSource)GetValue(TextureSourceProperty); }
            set { SetValue(TextureSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SourceFilename.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextureSourceProperty =
            DependencyProperty.Register("TextureSource", typeof(BitmapSource), typeof(DxSharedTexture), new PropertyMetadata(null, (s,e)=>
            {
                if (s is DxSharedTexture dxSharedTexture)
                {
                    dxSharedTexture._imageSource = e.NewValue as BitmapSource;
                }
            }));
    }
}
