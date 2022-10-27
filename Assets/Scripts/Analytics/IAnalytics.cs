
using System.Collections.Generic;

namespace AnalyticsData
{
	public interface IAnalytics
	{
		void SendEvent(string eName, List<(string, object)> param);
	}
}