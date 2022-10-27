
using System;
using System.Collections.Generic;
using Services.Advertising;
using UI;
using UI.Popups;
using UI.Windows;
using UnityEngine;

namespace Game.Level
{
	public class LevelManager : MonoBehaviour
	{
		[SerializeField] private List<LevelSettingData> _levelData = new List<LevelSettingData>();

		[SerializeField] private BoardManager _boardManager;
		
		public event Action<string, int> SetLevelGoal;
		public event Action<string> SetCountMoves;
		public event Action<float> SetValueOnProgressBar;
		public event Action<string, string> SetCountPoint;
		public event Action<int> InitLevels;
		public event Action<int> OpenLevel;
		public event Action Victory;
		public event Action Lose;
		public event Action LevelNotLoad;
		
		private TypeLevelGoal _typeLevelGoal;
		private TypeTile _typeTile;
		private int _neededToWinPoints;
		private int _countTokensNeededToDestroy;
		private int _maxCountMoves;
		private int _countOfPointsForDestroyedToken;

		private int _currentCountMoves;
		private int _currentCountPoint;

		private LevelSettingData _currentLevelSettingData;
		private int _currentLevel;
		private int _currentQuest;

		private Scoring _scoring;

		private int _passedLevels
		{
			get => PlayerPrefs.GetInt("PassedLevels");
			set => PlayerPrefs.SetInt("PassedLevels", value);
		}

		private void Start()
		{
			UIManager.LevelSelectionWindow.LoadSelectedLevel += OnLoadLevel;
			UIManager.GameWindow.ReloadCurrentLevel += OnReloadCurrentLevel;
			UIManager.EndGameWindow.ReloadCurrentLevel += OnReloadCurrentLevel;
			UIManager.EndGameWindow.LoadNextLevel += OnLoadNextLevel;

			AdvertisingManager.ShowRewardedAd(ReturnMove);

			SetLevelGoal += UIManager.GameWindow.OnSetGoalLevel;
			SetCountMoves += UIManager.GameWindow.OnSetCountMoves;
			SetValueOnProgressBar += UIManager.GameWindow.OnSetValueOnProgressBar;
			SetCountPoint += UIManager.GameWindow.OnSetCountPoint;
			InitLevels += UIManager.LevelSelectionWindow.OnInitBlockLevels;
			OpenLevel += UIManager.LevelSelectionWindow.OnOpenLevel;
			LevelNotLoad += UIManager.LevelSelectionWindow.OnShowPopupText;
			Victory += UIManager.EndGameWindow.OnVictory;
			Lose += UIManager.EndGameWindow.OnLose;
			
			InitLevels?.Invoke(_passedLevels);
		}

		private void Init(int numberQuest = 0)
		{
			_currentCountPoint = 0;
			_currentCountMoves = 0;
			_currentQuest = numberQuest;
			
			_typeLevelGoal = _currentLevelSettingData.LevelGoals[numberQuest].TypeLevelGoal;
			_typeTile = _currentLevelSettingData.LevelGoals[numberQuest].TypeTile;
			_neededToWinPoints = _currentLevelSettingData.LevelGoals[numberQuest].NeededToWinPoints;
			_countTokensNeededToDestroy = _currentLevelSettingData.CountTokensNeededToDestroy;
			_maxCountMoves = _currentLevelSettingData.MaxCountMoves;
			_countOfPointsForDestroyedToken = _currentLevelSettingData.PointsForDestroyingToken;

			if (_currentQuest == 0)
			{
				_boardManager.Init(_currentLevelSettingData.IceSpawnChance, _currentLevelSettingData.StoneSpawnChance,
					_currentLevelSettingData.ComplicatedStoneSpawnChance, _currentLevelSettingData.BlockedPositions);
			}

			if (_typeLevelGoal == TypeLevelGoal.ScoreCertainNumberTokens)
				_scoring = new ScoreCertainNumberPoints(_currentCountPoint, _typeTile);
			else if (_typeLevelGoal == TypeLevelGoal.DestroyAllObstaclesCertainType)
				_scoring = new DestroyObstacles(_currentCountPoint, _typeTile);
			else if (_typeLevelGoal == TypeLevelGoal.DestructionOfTokensCertainColor)
				_scoring = new DestructionOfTokensCertainColor(_currentCountPoint, _typeTile);
			
			SetGoal(_typeLevelGoal);
			SetCountMoves?.Invoke(_maxCountMoves.ToString());
			CalculateTaskCompletionPercentage();
		}

		private void OnLoadLevel(int selectedLevel)
		{
			if (CheckLevelSelection(selectedLevel) == false)
			{
				LevelNotLoad?.Invoke();
				return;
			}
			
			if(!UIManager.GameWindow.gameObject.activeSelf)
				UIManager.Open<GameWindow>();	
			
			_currentLevel = selectedLevel;
			_currentLevelSettingData = _levelData[selectedLevel];

			Init();
			
			Analytics.LevelOpened(_currentLevel);
		}
		
		private bool CheckLevelSelection(int selectedLevel)
		{
			return selectedLevel <= _passedLevels;
		}

		private void SetGoal(TypeLevelGoal typeLevelGoal)
		{
			var countPoints = _neededToWinPoints - _currentCountPoint;

			switch (typeLevelGoal)
			{
				case TypeLevelGoal.DestructionOfTokensCertainColor:
				{
					SetLevelGoal?.Invoke(_typeTile.ToString(), countPoints);
					break;
				}
				case TypeLevelGoal.DestroyAllObstaclesCertainType:
				{
					SetLevelGoal?.Invoke("Obstacles", countPoints);
					break;
				}
				case TypeLevelGoal.ScoreCertainNumberTokens:
				{
					SetLevelGoal?.Invoke("Tokens", countPoints);
					break;
				}
			}
		}

		private void CalculateTaskCompletionPercentage()
		{
			var percentage = _currentCountPoint / (float)_neededToWinPoints;
			SetValueOnProgressBar?.Invoke(percentage);
			SetCountPoint?.Invoke(_currentCountPoint.ToString(), _neededToWinPoints.ToString());
		}

		private void OnLoadNextLevel()
		{
			var nextLevel = _currentLevel;
			nextLevel++;

			if (nextLevel >= _levelData.Count)
				OnLoadLevel(_currentLevel);
			else
				OnLoadLevel(nextLevel);
		}

		private void OnReloadCurrentLevel()
		{
			OnLoadLevel(_currentLevel);
		}

		public bool EnoughTokensToDestroy(int count)
		{
			return count >= _countTokensNeededToDestroy;
		}

		private void VictoryGame()
		{
			if (_currentQuest < _currentLevelSettingData.LevelGoals.Count - 1)
			{
				UIManager.Open<NextQuestPopup>();
				_currentQuest++;
				Init(_currentQuest);
			}
			else
			{
				Analytics.LevelClosed(_currentLevel, true);
				
				Victory?.Invoke();
				UIManager.Open<EndGameWindow>();
				
				AudioManager.PlaySound("PassedLevel");	

				if (_currentLevel == _passedLevels)
				{
					_passedLevels++;
					OpenLevel?.Invoke(_passedLevels);
				}
			}
		}

		public void CheckPositionsForAssignmentsAddPoint(List<TileData> selectedPosition)
		{
			_currentCountPoint = _scoring.CheckPositionsForAssignmentsAddPoint(selectedPosition,
				_countOfPointsForDestroyedToken);

			CalculateTaskCompletionPercentage();
			
			CheckGameToVictory();

			SetGoal(_typeLevelGoal);
		}

		private bool CheckGameToVictory()
		{
			if (_currentCountPoint < _neededToWinPoints) 
				return false;
			
			VictoryGame();
			return true;
		}

		private void ReturnMove()
		{
			UIManager.Open<GameWindow>();
			
			_currentCountMoves -= 5;
			
			if(_currentCountMoves < 0)
				_currentCountMoves = 0;
			
			SetCountMoves?.Invoke((_maxCountMoves - _currentCountMoves).ToString());
		}
		
		public void AddCountMove()
		{
			_currentCountMoves++;

			SetCountMoves?.Invoke((_maxCountMoves - _currentCountMoves).ToString());

			if (CheckGameToVictory())
				return;
			
			if (_currentCountMoves >= _maxCountMoves)
			{
				Analytics.LevelClosed(_currentLevel, false);
				
				Lose?.Invoke();
				UIManager.Open<EndGameWindow>();

				AudioManager.PlaySound("LossLevel");
			}
		}
		
		private abstract class Scoring
		{
			protected int CurrentCountPoint;
			protected readonly TypeTile TypeTile;

			protected Scoring(int currentCountPoint, TypeTile typeTile)
			{
				CurrentCountPoint = currentCountPoint;
				TypeTile = typeTile;
			}
		
			public abstract int CheckPositionsForAssignmentsAddPoint(List<TileData> position, 
				int countOfPointsForDestroyedToken);
		}
		
		private class ScoreCertainNumberPoints : Scoring
		{
			public override int CheckPositionsForAssignmentsAddPoint(List<TileData> position, 
				int countOfPointsForDestroyedToken)
			{
				var countPoint = countOfPointsForDestroyedToken * position.Count; 

				CurrentCountPoint += countPoint;

				return CurrentCountPoint;
			}

			public ScoreCertainNumberPoints(int currentCountPoint, TypeTile typeTile) : base(currentCountPoint, typeTile)
			{
			}
		}

		private class DestroyObstacles : Scoring
		{
			public override int CheckPositionsForAssignmentsAddPoint(List<TileData> position, 
				int countOfPointsForDestroyedToken)
			{
				foreach (var tileData in position)
				{
					if (tileData.TypeTile != TypeTile.Ice && tileData.TypeTile != TypeTile.Stone) 
						continue;
						
					CurrentCountPoint += countOfPointsForDestroyedToken;
				}
			
				return CurrentCountPoint;
			}

			public DestroyObstacles(int currentCountPoint, TypeTile typeTile) : base(currentCountPoint, typeTile)
			{
			}
		}

		private class DestructionOfTokensCertainColor : Scoring
		{
			public override int CheckPositionsForAssignmentsAddPoint(List<TileData> position, 
				int countOfPointsForDestroyedToken)
			{
				foreach (var tileData in position)
				{
					if (tileData.TypeTile != TypeTile) 
						continue;
						
					CurrentCountPoint += countOfPointsForDestroyedToken;
				}
			
				return CurrentCountPoint;
			}

			public DestructionOfTokensCertainColor(int currentCountPoint, TypeTile typeTile) 
				: base(currentCountPoint, typeTile)
			{
			}
		}
	}
}