// Copyright (c) 2012-2022 FuryLion Group. All Rights Reserved.

using AnalyticsData;
using System.Collections.Generic;

public static class Analytics
{
	public static void LevelClosed(int levelIndex, bool result) 
	{
		AnalyticsManager.Event(levelIndex.ToString(), new List<(string, object)>() {("levelIndex", levelIndex), ("result", result)});
	}

	public static void LevelOpened(int levelIndex) 
	{
		AnalyticsManager.Event(levelIndex.ToString(), new List<(string, object)>() {("levelIndex", levelIndex)});
	}

	public static void SettingsOpened() 
	{
		AnalyticsManager.Event(null, null);
	}

	public static void MusicChanged(float value) 
	{
		AnalyticsManager.Event(value.ToString(), new List<(string, object)>() {("value", value)});
	}

	public static void SoundChanged(float value) 
	{
		AnalyticsManager.Event(value.ToString(), new List<(string, object)>() {("value", value)});
	}


}