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
    public class TimelineChartDateTimeLabelProvider : LabelProviderBase<double>
    {
        private DateTime _startDateTime = new DateTime();

        public TimelineChartDateTimeLabelProvider()
        {
        }

        /*
    public void SetDateAndTime(string scenarioStartDate, int startHour, int startMinute)
    {
      startOffsetMins = startHour * 60 + startMinute;

      if (!System.DateTime.TryParse(scenarioStartDate, out startDateTime))
      {
        // If no scenario start time use today's date (but start at 0 hour).
        System.DateTime utcNow = System.DateTime.UtcNow;
        startDateTime = new System.DateTime(utcNow.Year, utcNow.Month, utcNow.Day);
      }
    }
        */

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
                    labelText = _startDateTime.AddMinutes(tickInfo.Tick).ToString("dd/MM/yyyy HH:mm UTC");
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
