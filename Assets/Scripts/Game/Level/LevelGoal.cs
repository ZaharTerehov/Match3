
using UnityEngine;

namespace Game.Level
{
	[System.Serializable]
	public class LevelGoal
	{
		[SerializeField] public TypeLevelGoal TypeLevelGoal;

		[SerializeField] public TypeTile TypeTile;
		
		[Range (1, 400)]
		[SerializeField] public int NeededToWinPoints;
	}
}