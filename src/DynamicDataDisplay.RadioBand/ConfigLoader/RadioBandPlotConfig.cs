using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace DynamicDataDisplay.RadioBand.ConfigLoader
{
  /// <summary>
  /// Plot Config. Has to be read in using XDocument because the axis stuff is file order dependent
  /// </summary>
  public class RadioBandPlotConfig
  {
    private MemoryStream _originalStream = new MemoryStream();
    public RadioBandPlotConfig()
    {
    }

    private static Lazy<RadioBandPlotConfig> _spectrumDisplayDefault = new Lazy<RadioBandPlotConfig>(() =>
      {
        var assembly = Assembly.GetAssembly(typeof(RadioBandPlotConfig));
        var names = assembly.GetManifestResourceNames();
        using (var stream = assembly.GetManifestResourceStream("DynamicDataDisplay.RadioBand.Configs.SpectrumDisplayDefault.xml"))
        {
          return new RadioBandPlotConfig(stream);
        }
      }, true);

    public static RadioBandPlotConfig SpectrumDisplayDefault => _spectrumDisplayDefault.Value;

    public static RadioBandPlotConfig FromXmlFile(string filename)
    {
      using (Stream stream = File.OpenRead(filename))
      {
        return new RadioBandPlotConfig(stream);

      }
    }

    public RadioBandPlotConfig(Stream stream)
    {
      stream.CopyTo(_originalStream);
      _originalStream.Seek(0, SeekOrigin.Begin);
      XElement node = XDocument.Load(_originalStream).Root;
      Ticks = new double[0];
      FrequencyLabels = new List<FrequencyLabelSet>();

      foreach (var child in node.Elements())
      {
        switch (child.Name.LocalName)
        {
          case nameof(Ticks):
            Ticks = child.Value
              .Split(" \t\n\r,".ToArray(), StringSplitOptions.RemoveEmptyEntries)
              .Select(str => double.Parse(str))
              .ToArray();
            break;
          case nameof(FrequencyRangeLabels):
            FrequencyLabels.Add(FrequencyRangeLabels.FromXmlNode(child));
            break;
          case nameof(FrequencyPointLabels):
            FrequencyLabels.Add(FrequencyPointLabels.FromXmlNode(child));
            break;
        }
      }
    }

    internal void SaveToXml(string fileName)
    {
      using (Stream stream = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
      {
        _originalStream.Seek(0, SeekOrigin.Begin);
        _originalStream.WriteTo(stream);
      }
    }


    public double[] Ticks { get; private set; } = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

    public List<FrequencyLabelSet> FrequencyLabels { get; private set; } = new List<FrequencyLabelSet>();

    public IEnumerable<string> GetDebugInfo()
    {
      yield return "-------------------- Ticks --------------------";
      foreach (var tick in Ticks)
      {
        yield return $"{tick}";
      }
      foreach (var label in FrequencyLabels)
      {
        yield return "-------------------- Labels Title --------------------";
        foreach (var part in label.Description)
        {
          yield return part;
        }
        if (label is FrequencyRangeLabels rangeLabels)
        {
          foreach (var rangeLabel in rangeLabels.Labels)
          {
            yield return "-------------------- Range Label --------------------";
            foreach (var text in rangeLabel.LabelText)
            {
              yield return text;
            }
          }
        }
        if (label is FrequencyPointLabels pointLabels)
        {
          foreach (var pointLabel in pointLabels.Labels)
          {
            yield return "-------------------- Point Label --------------------";
            foreach (var text in pointLabel.LabelText)
            {
              yield return text;
            }
          }
        }
      }
    }
  }
}
