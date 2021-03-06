﻿using System.Windows.Automation.Peers;

namespace DynamicDataDisplay.Common
{
	public class PlotterAutomationPeer : FrameworkElementAutomationPeer
	{
		public PlotterAutomationPeer(Plotter owner)
			: base(owner)
		{

		}

		protected override AutomationControlType GetAutomationControlTypeCore()
		{
			return AutomationControlType.Custom;
		}

		protected override string GetClassNameCore()
		{
			return "Plotter";
		}
	}
}
