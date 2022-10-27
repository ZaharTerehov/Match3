
using System.Collections.Generic;
using Game.Level;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor.Scripts
{
	public class LevelEditor : MonoBehaviour
	{
		[SerializeField] private Slider _countTokensNeededToDestroy;
		[SerializeField] private Slider _maxCountMoves;
		[SerializeField] private Slider _pointsForDestroyingToken;
		[SerializeField] private Slider _iceSpawnChance;
		[SerializeField] private Slider _stoneSpawnChance;
		[SerializeField] private Slider _complicatedStoneSpawnChance;

		[SerializeField] private Slider _neededToWinPoints;
		[SerializeField] private Dropdown _typeLevelGoal;
		[SerializeField] private Dropdown _typeTile;

		[SerializeField] private Dropdown _levels;
		[SerializeField] private Dropdown _quests;
		[SerializeField] private Button _createNewLevel;
		[SerializeField] private Button _deleteLevel;
		[SerializeField] private Button _save;
		
		[SerializeField] private Button _createNewQuest;
		[SerializeField] private Button _deleteQuest;

		[SerializeField] private Toggle _toggle;

		[SerializeField] private BoardManager _boardManager;

		public int CountTokensNeededToDestroy => (int) _countTokensNeededToDestroy.value;
		public bool Toggle => _toggle.isOn;

		private List<LevelSettingData> _levelsSettingData = new List<LevelSettingData>();

		private void Start()
		{
			_save.onClick.AddListener(delegate { SaveLevel(_levels.value); });
			_createNewLevel.onClick.AddListener(CreateNewLevel);
			_deleteLevel.onClick.AddListener(DeleteLevel);
			_levels.onValueChanged.AddListener(SelectLevel);
			_quests.onValueChanged.AddListener(SetSettings);
			
			_createNewQuest.onClick.AddListener(AddNewQuest);
			_deleteQuest.onClick.AddListener(DeleteQuest);

			_levelsSettingData = new List<LevelSettingData>(Resources.LoadAll<LevelSettingData>("LevelSettingSO/"));

			InitDropdownLevels();
			SelectLevel(0);
			SetSettings(0);
		}

		private void CreateNewLevel()
		{
			var newLevelSettingData = ScriptableObject.CreateInstance<LevelSettingData>();

			_levelsSettingData.Add(newLevelSettingData);

			AssetDatabase.CreateAsset(newLevelSettingData,
				$"Assets/Resources/LevelSettingSO/Level + {_levelsSettingData.Count} + SettingData.asset");

			_levels.options.Add(new Dropdown.OptionData(_levelsSettingData.Count.ToString()));
			_levels.value = _levelsSettingData.Count;
			_boardManager.BlockedPositions = new List<Vector3Int>();

			AssetDatabase.SaveAssets();
		}

		private void InitDropdownLevels()
		{
			_levels.ClearOptions();

			for (var i = 1; i < _levelsSettingData.Count + 1; i++)
			{
				_levels.options.Add(new Dropdown.OptionData(i.ToString()));
			}
		}

		private void SaveLevel(int value, int numberQuest = 0)
		{
			var levelSettingData = _levelsSettingData[value];

			levelSettingData.CountTokensNeededToDestroy = (int) _countTokensNeededToDestroy.value;
			levelSettingData.MaxCountMoves = (int) _maxCountMoves.value;
			levelSettingData.PointsForDestroyingToken = (int) _pointsForDestroyingToken.value;
			levelSettingData.IceSpawnChance = (int) _iceSpawnChance.value;
			levelSettingData.StoneSpawnChance = (int) _stoneSpawnChance.value;
			levelSettingData.ComplicatedStoneSpawnChance = (int) _complicatedStoneSpawnChance.value;
			
			var currentQuest = levelSettingData.LevelGoals[numberQuest];

			currentQuest.NeededToWinPoints = (int) _neededToWinPoints.value;
			currentQuest.TypeLevelGoal = (TypeLevelGoal) _typeLevelGoal.value;
			currentQuest.TypeTile = (TypeTile) _typeTile.value;
			levelSettingData.BlockedPositions = _boardManager.BlockedPositions;
			
			_boardManager.Init(levelSettingData.IceSpawnChance, levelSettingData.StoneSpawnChance, 
				levelSettingData.ComplicatedStoneSpawnChance, levelSettingData.BlockedPositions);
			
			EditorUtility.SetDirty(levelSettingData);
			
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		private void SelectLevel(int selectedIndex)
		{
			InitDropdownQuests(_levelsSettingData[selectedIndex]);
			SetSettings(0);
		}

		private void AddNewQuest()
		{
			var levelSettingData = _levelsSettingData[_levels.value];

			var levelGoal = new LevelGoal
			{
				TypeTile = (TypeTile) _typeTile.value,
				TypeLevelGoal = (TypeLevelGoal) _typeLevelGoal.value,
				NeededToWinPoints = (int) _neededToWinPoints.value
			};

			levelSettingData.LevelGoals.Add(levelGoal);

			InitDropdownQuests(_levelsSettingData[_levels.value]);
		}

		private void DeleteQuest()
		{
			var levelSettingData = _levelsSettingData[_levels.value];
			levelSettingData.LevelGoals.RemoveAt(_quests.value);
		}

		private void SetSettings(int numberQuest)
		{
			var levelSettingData = _levelsSettingData[_levels.value];
			
			_countTokensNeededToDestroy.value = levelSettingData.CountTokensNeededToDestroy;
			_maxCountMoves.value = levelSettingData.MaxCountMoves;
			_pointsForDestroyingToken.value = levelSettingData.PointsForDestroyingToken;
			_iceSpawnChance.value = levelSettingData.IceSpawnChance;
			_stoneSpawnChance.value = levelSettingData.StoneSpawnChance;
			_complicatedStoneSpawnChance.value = levelSettingData.ComplicatedStoneSpawnChance;
			
			_neededToWinPoints.value = levelSettingData.LevelGoals[numberQuest].NeededToWinPoints;
			_typeLevelGoal.value = (int)levelSettingData.LevelGoals[numberQuest].TypeLevelGoal;
			_typeTile.value = (int)levelSettingData.LevelGoals[numberQuest].TypeTile;
			
			_boardManager.Init(levelSettingData.IceSpawnChance, levelSettingData.StoneSpawnChance, 
				levelSettingData.ComplicatedStoneSpawnChance, levelSettingData.BlockedPositions);
		}
		
		private void InitDropdownQuests(LevelSettingData levelSettingData)
		{
			if (levelSettingData.LevelGoals.Count == 0)
				AddNewQuest();
		
			_quests.ClearOptions();
			
			for (var i = 1; i < levelSettingData.LevelGoals.Count + 1; i++)
			{
				_quests.options.Add(new Dropdown.OptionData(i.ToString()));
			}
		}

		private void DeleteLevel()
		{
			_levelsSettingData.Remove(_levelsSettingData[_levels.value]);
			
			var number = _levels.value;
			number++;
			
			AssetDatabase.DeleteAsset($"Assets/Resources/LevelSettingSO/Level + {number} + SettingData.asset");
			
			var item = 1;

			foreach (var levelSettingData in _levelsSettingData)
			{
				SelectLevel(item-1);
				var assetPath = AssetDatabase.GetAssetPath(levelSettingData);
			
				AssetDatabase.RenameAsset(assetPath, $"Level + {item} + SettingData");
			
				SaveLevel(item-1);
				item++;
			}
			
			InitDropdownLevels();
			AssetDatabase.SaveAssets();
		}
	}
}