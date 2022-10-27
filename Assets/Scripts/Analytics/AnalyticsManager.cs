
using System.Collections.Generic;
using AnalyticsData.AnalyticsHendler;

namespace AnalyticsData
{
	public class AnalyticsManager
	{
		private static List<IAnalytics> _analytics = new List<IAnalytics>()
		{
			new Analytics1Hendler(),
			new Analytics2Hendler()
		};

		public static void Event(string eName, List<(string, object)> param)
		{
			foreach (var analytic in _analytics)
			{
				analytic.SendEvent(eName, param);
			}
		}
	}
}