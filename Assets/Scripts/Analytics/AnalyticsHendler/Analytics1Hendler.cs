
using System.Collections.Generic;
using AnalyticsPlugins;

namespace AnalyticsData.AnalyticsHendler
{
	public class Analytics1Hendler : IAnalytics
	{
		private Analytics1 analytics1 = new Analytics1();
		
		public void SendEvent(string eName, List<(string, object)> param)
		{
			if (eName == null || param == null)
				return;
			
			var paramString = new string[param.Count];

			for (var p = 0; p < paramString.Length; p++)
			{
				paramString[p] = param[p].Item1;
			}
			
			analytics1.SendEvent(eName, paramString);
		}
	}
}