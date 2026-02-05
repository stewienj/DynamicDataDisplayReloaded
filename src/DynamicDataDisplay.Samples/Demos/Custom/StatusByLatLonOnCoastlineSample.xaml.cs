using DynamicDataDisplay.Common.Auxiliary;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DynamicDataDisplay.Samples.Demos.Custom;

/// <summary>
/// Interaction logic for StatusByLatLonOnMapSample.xaml
/// </summary>
public partial class StatusByLatLonOnCoastLineSample : Page
{
    public StatusByLatLonOnCoastLineSample()
    {
        DataContext = new StatusByLatLonOnCoastLineViewModel();
        InitializeComponent();
    }
}

public class StatusByLatLonOnCoastLineViewModel : D3NotifyPropertyChanged
{

    public StatusByLatLonOnCoastLineViewModel()
    {
        Size90x45 = true;
    }

    private Task<uint[]> CreateBitmap(int width, int height)
    {
        lock (this)
        {
            return Task.Run(() =>
            {
                var pixels = new uint[width * height];
                uint alpha = 0x77000000;
                int colorIndex = 0;
                int pixelsIndex = 0;
                for (int y = 0; y < height; ++y)
                {
                    colorIndex = y;
                    for (int x = 0; x < width; ++x)
                    {
                        colorIndex = (colorIndex + 1) % 3;
                        pixels[pixelsIndex] = alpha | (uint)(255 << (colorIndex * 8));
                        pixelsIndex++;
                    }
                }
                return pixels;
            });
        }
    }

    private Size _pixelsSize = new Size(720, 360);
    public Size PixelsSize
    {
        get => _pixelsSize;
        set => SetProperty(ref  _pixelsSize, value);
    }

    private IList<uint> _pixels = [];

    public IList<uint> Pixels
    {
        get => _pixels;
        set => SetProperty(ref _pixels, value);
    }

    private bool _size90x45 = false;
    public bool Size90x45
    {
        get => _size90x45;
        set
        {
            SetProperty(ref _size90x45, value);
            if (_size90x45)
            {
                CreateBitmap(90, 45)
                    .ContinueWith(t =>
                    {
                        PixelsSize = new Size(90, 45);
                        Pixels = t.Result;
                    });
            }
        }
    }

    private bool _size180x90 = false;
    public bool Size180x90
    {
        get => _size180x90;
        set
        {
            SetProperty(ref _size180x90, value);
            if (_size180x90)
            {
                CreateBitmap(180, 90)
                .ContinueWith(t =>
                {
                    PixelsSize = new Size(180, 90);
                    Pixels = t.Result;
                });
            }
        }
    }

    private bool _size360x180 = false;
    public bool Size360x180
    {
        get => _size360x180;
        set
        {
            SetProperty(ref _size360x180, value);
            if (_size360x180)
            {
                CreateBitmap(360, 180)
                .ContinueWith(t =>
                {
                    PixelsSize = new Size(360, 180);
                    Pixels = t.Result;
                });
            }
        }
    }

    private bool _size720x360 = false;
    public bool Size720x360
    {
        get => _size720x360;
        set
        {
            SetProperty(ref _size720x360, value);
            if (_size720x360)
            {
                CreateBitmap(720, 360)
                .ContinueWith(t =>
                {
                    PixelsSize = new Size(720, 360);
                    Pixels = t.Result;
                });
            }
        }
    }

    private bool _size1024x512 = false;
    public bool Size1024x512
    {
        get => _size1024x512;
        set
        {
            SetProperty(ref _size1024x512, value);
            if (_size1024x512)
            {
                CreateBitmap(1024, 512)
                .ContinueWith(t =>
                {
                    PixelsSize = new Size(1024, 512);
                    Pixels = t.Result;
                });
            }
        }
    }

    private bool _size2048x1024 = false;
    public bool Size2048x1024
    {
        get => _size2048x1024;
        set
        {
            SetProperty(ref _size2048x1024, value);
            if (_size2048x1024)
            {
                CreateBitmap(2048, 1024)
                .ContinueWith(t =>
                {
                    PixelsSize = new Size(2048, 1024);
                    Pixels = t.Result;
                });
            }
        }
    }

    private bool _size4096x2048 = false;
    public bool Size4096x2048
    {
        get => _size4096x2048;
        set
        {
            SetProperty(ref _size4096x2048, value);
            if (_size4096x2048)
            {
                CreateBitmap(4096, 2048)
                .ContinueWith(t =>
                {
                    PixelsSize = new Size(4096, 2048);
                    Pixels = t.Result;
                });
            }
        }
    }
}
