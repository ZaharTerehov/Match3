
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using GoogleSpreadsheets;
using UnityEditor;
using UnityEngine;

namespace CodeGenerator
{
	public static class CodeGenerator
	{
		private static string _sheetId = "1A9Zk0BHFY8-hhSt-A_IZs2s7Z9pjylu4GNhd65EcFMk";
		
		[InitializeOnLoadMethod]
		public static async void GenerateAnalyticsClass() 
		{
			var downloadedSpreadsheet = await CVSLoader.DownloadTable(_sheetId);
			
			var methodData = SheetProcessor.ConvertStringToMethods(downloadedSpreadsheet);
			
			var methods = new StringBuilder();

			foreach (var (name, options) in methodData)
			{
				var content = GetContentMethod(options.ToString());
				
				methods.Append(GetMethod(name.ToString(), options.ToString(), content)); 
				methods.Append("\n");
			}

			var classCode = "// Copyright (c) 2012-2022 FuryLion Group. All Rights Reserved.\n" +
							"\nusing AnalyticsData;\nusing System.Collections.Generic;\n" + $@"
public static class Analytics
{{
{
		string.Join("\n",
			methods)
}
}}";

			File.WriteAllText($"{Application.dataPath}/Scripts/Analytics/Analytics.cs", classCode);
		}

		private static string GetContentMethod(string options)
		{
			var partsOptionMethod = options.Split(' ');

			var secondOption = new StringBuilder();
			var firstOption = new List<string>();

			for (var i = 1; i < partsOptionMethod.Length; i+=2)
			{
				var option = Regex.Replace(partsOptionMethod[i], "[^a-zA-Z]", "");

				firstOption.Add($"{option}");
				secondOption.Append($"{option}.ToString(), ");
			}

			var parameter = new StringBuilder();

			foreach (var option in firstOption)
			{
				parameter.Append($@"(""{option}"", {option}), ");
			}

			if (parameter.Length > 1)
				parameter.Remove(parameter.Length - 2, 2);
			if (secondOption.Length > 1)   
				secondOption.Remove(secondOption.Length - 2, 2);
			
			if(firstOption.Count > 0)
				return $"\t\tAnalyticsManager.Event({firstOption[0]}.ToString(), new List<(string, object)>() {{{parameter}}});";
			else
				return "\t\tAnalyticsManager.Event(null, null);";
		}
		private static string GetMethod(string nameMethod, string methodParameters, string context = "")
		{
			return $"\tpublic static void {nameMethod}({methodParameters}) " + 
					"\n\t{" +
					$"\n{context}" +
					"\n\t}\n"; 
		}
	}
}