using DynamicDataDisplay.Charts;
using DynamicDataDisplay.Charts.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DynamicDataDisplay.TimelineChart
{
    public class UtcDateTimeLabelProvider : LabelProviderBase<double>
    {
        private DateTime _startDateTime = new DateTime();

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

                if (tickInfo.Tick > 0)
                {
                    // We add the base scenario time and start hour and minutes to generate scenario time of day for the X-axis.                
                    labelText = _startDateTime.AddMinutes(tickInfo.Tick).ToUniversalTime().ToString("dd/MM/yyyy HH:mm UTC");
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
