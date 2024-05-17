using DynamicDataDisplay.Charts;
using DynamicDataDisplay.Charts.Axes;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DynamicDataDisplay.FrequencyTimeline
{
    public class UtcDateTimeLabelProvider : LabelProviderBase<double>
    {
        public UtcDateTimeLabelProvider()
        {
        }

        public override UIElement[] CreateLabels(ITicksInfo<double> ticksInfo)
        {
            var ticks = ticksInfo.Ticks;
            UIElement[] res = new UIElement[ticks.Length];

            LabelTickInfo<double> tickInfo = new LabelTickInfo<double> { Info = ticksInfo.Info };

            for (int i = 0; i < res.Length; i++)
            {
                tickInfo.Tick = ticks[i];
                tickInfo.Index = i;

                string labelText = "";

                // Check we are adding minutes (not subtracting), we are not adding more than 9999 years
                if (tickInfo.Tick > 0 && tickInfo.Tick < 5_000_000_000)
                {
                    // We add the base scenario time and start hour and minutes to generate scenario time of day for the X-axis.                
                    labelText = DateTime.MinValue.AddMinutes(tickInfo.Tick).ToUniversalTime().ToString("dd/MM/yyyy HH:mm UTC");
                }
                else
                { 
                    labelText = "?";
                }

                TextBlock label = (TextBlock)GetResourceFromPool();
                if (label == null)
                {
                    label = new TextBlock();
                }

                label.Text = labelText;

                res[i] = label;

                ApplyCustomView(tickInfo, label);
            }
            return res;
        }
    }
}
