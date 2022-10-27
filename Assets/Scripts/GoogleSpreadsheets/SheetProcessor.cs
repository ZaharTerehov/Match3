
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GoogleSpreadsheets
{
	public static class SheetProcessor
	{
		private const char _cellSeporator = ',';

		public static List<(StringBuilder, StringBuilder)> ConvertStringToMethods(string cvsRawData)
		{
			var lineEnding = GetPlatformSpecificLineEnd();
			var rows = cvsRawData.Split(lineEnding);

			var listMethod = new List<(StringBuilder name, StringBuilder options)>();

			for (var numberRow = 1; numberRow < rows.Length; numberRow++) 
			{
				var cells = rows[numberRow].Split(_cellSeporator);

				var nameMethod = new StringBuilder();

				GetNameMethod(cells[0], ref nameMethod);

				var options = new StringBuilder(); 

				for (var numberCell = 1; numberCell < cells.Length - 1; numberCell++)
				{
					var partsOptionMethod = cells[numberCell].Trim().Split(' ');
					
					var normalOptionsMethod = new List<string>();
					
					foreach (var partOption in partsOptionMethod)
					{
						normalOptionsMethod.Add(Regex.Replace(partOption, "[^a-zA-Z]", ""));
					}

					if (normalOptionsMethod.Count < 2) 
						continue;
					
					options.Append($"{normalOptionsMethod[0]} {normalOptionsMethod[1]}");
					
					if (numberCell > 0 && numberCell != cells.Length - 2)
						options.Append(", ");
				}

				listMethod.Add((nameMethod, options));
			}
			
			return listMethod;
		}

		private static void GetNameMethod(string rawMethodName, ref StringBuilder nameMethod)
		{
			var partsMethodName = rawMethodName.Trim().Split(' ');
			
			foreach (var part in partsMethodName)
			{
				nameMethod.Append(char.ToUpper(part[0]) + part.Substring(1, part.Length - 1));
			}
		}
		
		private static char GetPlatformSpecificLineEnd()
		{
			var lineEnding = '\n';
			
			#if UNITY_IOS
				lineEnding = '\r';
			#endif
			return lineEnding;
		}
	}
}