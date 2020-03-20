using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DynamicDataDisplay.RadioBand
{
  /// <summary>
  /// Interaction logic for RadioBandGroupLabel.xaml
  /// </summary>
  public partial class RadioBandGroupLabel : UserControl
  {
    public RadioBandGroupLabel()
    {
      InitializeComponent();
    }


    public string Text
    {
      get { return (string)GetValue(TextProperty); }
      set { SetValue(TextProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(RadioBandGroupLabel), new PropertyMetadata(null));



    public Thickness TextMargin
    {
      get { return (Thickness)GetValue(TextMarginProperty); }
      set { SetValue(TextMarginProperty, value); }
    }

    // Using a DependencyProperty as the backing store for TextMargin.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TextMarginProperty =
        DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(RadioBandGroupLabel), new PropertyMetadata(new Thickness(2)));



  }
}
