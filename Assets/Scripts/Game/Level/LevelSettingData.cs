
using System.Collections.Generic;
using UnityEngine;

namespace Game.Level
{
	[CreateAssetMenu(fileName = "New LevelSettingData", menuName = "LevelSetting Data")]
	public class LevelSettingData : ScriptableObject
	{
		[Range(1, 10)]
		[Tooltip("Count Tokens Needed To Destroy")]
		[SerializeField] public int CountTokensNeededToDestroy;
		
		[Range(1, 100)]
		[SerializeField] public int MaxCountMoves;
		
		[Range(1, 50)]
		[Tooltip("Points For Destroying Token")]
		[SerializeField] public int PointsForDestroyingToken;
		
		[Space]
		[Header("Level goal data")]
		[SerializeField] public List<LevelGoal> LevelGoals;

		[Space]
		[Header("Ice and stone spawn chance")]
		[Range(1,99), SerializeField] public int IceSpawnChance;
		[Range(1,99), SerializeField] public int StoneSpawnChance;
		[Range(1,99), SerializeField] public int ComplicatedStoneSpawnChance;
		
		public List<Vector3Int> BlockedPositions;
	}
}