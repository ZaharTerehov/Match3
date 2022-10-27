
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace GoogleSpreadsheets
{
	public static class CVSLoader
	{
		private static string _url = "https://docs.google.com/spreadsheets/d/*/export?format=csv";

		public static async UniTask<string> DownloadTable(string sheetId)
		{
			var actualUrl = _url.Replace("*", sheetId);
			
			return await DownloadRawCvsTable(actualUrl);
		}
		
		private static async UniTask<string> DownloadRawCvsTable(string actualUrl)
		{
			using var request = UnityWebRequest.Get(actualUrl);
			
			await request.SendWebRequest();
			
			if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError ||
				request.result == UnityWebRequest.Result.DataProcessingError)
				Debug.LogError(request.error);
			else
				return request.downloadHandler.text;
			
			return null;
		}
	}
}