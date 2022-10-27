
using UnityEngine;

namespace AnalyticsPlugins
{
	public class Analytics1
	{
		public void SendEvent(string eName, params string[] param)
		{
			var str = $"[Analytics 1] [{eName}] ";

			foreach (var p in param)
			{
				str += $"{p} ";
			}

			Debug.Log(str);
		}
	}
}