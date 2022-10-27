
using System.Collections.Generic;
using AnalyticsPlugins;

namespace AnalyticsData.AnalyticsHendler
{
	public class Analytics2Hendler : IAnalytics
	{
		private Analytics2 analytics2 = new Analytics2();
		
		public void SendEvent(string eName, List<(string, object)> param)
		{
			if (eName == null || param == null)
				return;
			
			analytics2.Event(eName, param);
		}
	}
}