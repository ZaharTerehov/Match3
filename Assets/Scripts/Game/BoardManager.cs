
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Extensions;
using Game.Level;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
	public class BoardManager : MonoBehaviour
	{
		[SerializeField] private LevelManager _levelManager;
		[SerializeField] private TileSmoothMovementManager _tileSmoothMovementManager;

		[Header("Size board")]
		[SerializeField] private Vector2Int _gridSize;

		[Space]
		[Header("Tilemap and tiles")]
		[SerializeField] private Tilemap _tilemap;
		[SerializeField] private List<Tile> _tiles = new List<Tile>();
		[SerializeField] private List<Tile> _tilesIce = new List<Tile>();
		
		[Space]
		[Header("Obstacles")]
		[SerializeField] private Tile _stoneTile;
		[SerializeField] private Tile _complicatedStoneTile;

		[Space]
		[Header("Bonuses")]
		[SerializeField] private Tile _rocket;
		[SerializeField] private Tile _bomb;

		[Space]
		[Header("Selected tile")]
		[SerializeField] private Tilemap _tilemapSelected;
		[SerializeField] private Tile _selectedTile;
		
		[Space]
		[Header("Effect tilemap and tile")]
		[SerializeField] private Tilemap _tilemapEffect;
		[SerializeField] private TileBase _effectTile;

		[Space]
		[Header("Ice and stone spawn chance")]
		[Range(1,99), SerializeField] private int _iceSpawnChance = 5;
		[Range(1,99), SerializeField] private int _stoneSpawnChance = 5;
		[Range(1,99), SerializeField] private int _complicatedStoneSpawnChance = 5;

		[Space]
		[SerializeField] private Tile _blockedTile;
		[SerializeField] private List<Tile> _unselectedTiles;

		public bool IsPermutationTokens;

		private List<TileData> _currentTiles = new List<TileData>();
		private List<Vector3Int> _emptyPositions = new List<Vector3Int>();
		private List<Vector3Int> _blockedPositions = new List<Vector3Int>();

		private Tile _spawnBonus;
		private int  _numberOfPermutations;

		private List<Vector3Int> _startPositionsInTilemap = new List<Vector3Int>();
		private List<Vector3Int> _endPositionsInTilemap = new List<Vector3Int>();
		private List<Tile> _tilesToMove = new List<Tile>();
		
		private Sequence _tilesShufflingAnimation;
		private List<int> _latestTokenIndex = new List<int>();
		private float _timeBetweenFallingTiles;

		public void Init(int iceSpawnChance, int stoneSpawnChance, int complicatedStoneSpawnChance, 
			List<Vector3Int> blockedPositions)
		{
			ClearGameBoard();
			
			for (var x = 0; x < _gridSize.x; x++)
			{
				for (var y = 0; y < _gridSize.y; y++)
				{
					_currentTiles.Add(new TileData(new Vector3Int(-x, y, 0), null, TypeTile.Red));
				}
			}

			_iceSpawnChance = iceSpawnChance;
			_stoneSpawnChance = stoneSpawnChance;
			_blockedPositions = blockedPositions;
			_complicatedStoneSpawnChance = complicatedStoneSpawnChance;
			
			GameBoardFilling();
			IsPermutationTokens = false;
		}

		private void GameBoardFilling()
		{
			foreach (var tileData in _currentTiles)
			{
				var position = tileData.Position;
				
				SetRandomTile(position);
			}

			SettingBlockedTile(_blockedPositions);
		}

		private void ClearGameBoard()
		{
			_tilemap.ClearAllTiles();
		}

		private void SettingBlockedTile(List<Vector3Int> positions)
		{
			foreach (var position in positions)
			{
				_tilemap.SetTile(position, _blockedTile);
			}
		}

		private void SetRandomTile(Vector3Int spawnPosition)
		{
			if (_blockedPositions.Contains(spawnPosition) || _tilemap.GetTile<Tile>(spawnPosition) != null)
				return;

			if (Random.Range(1, 100) <= _stoneSpawnChance)
			{
				_tilemap.SetTile(spawnPosition, _stoneTile);
				return;
			}
			else if (Random.Range(1, 100) <= _complicatedStoneSpawnChance)
			{
				_tilemap.SetTile(spawnPosition, _complicatedStoneTile);
				return;
			}
			else
			{
				var tile = _tiles[Random.Range(0, _tiles.Count)];
				_tilemap.SetTile(spawnPosition, tile);
				
				if (Random.Range(1, 100) <= _iceSpawnChance)
					_tilemap.SetTile(spawnPosition, _tilesIce[Random.Range(0, _tilesIce.Count)]);
			}
			
			if (_spawnBonus != null)
			{
				_tilemap.SetTile(spawnPosition, _spawnBonus);
				_spawnBonus = null;
			}
		}

		private Tile GetRandomTile()
		{
			if (Random.Range(1, 100) <= _stoneSpawnChance)
				return _stoneTile;
			else if (Random.Range(1, 100) <= _complicatedStoneSpawnChance)
				return _complicatedStoneTile;
			else
				return _tiles[Random.Range(0, _tiles.Count)];
		}

		private void AddDataToBlendTile(Vector3Int startPosition, Vector3Int endPosition, Tile topTile, 
			Sequence tilesShufflingAnimation, bool isNewTile = false)
		{
			_startPositionsInTilemap.Add(new Vector3Int(startPosition.x, startPosition.y, 0));
			_endPositionsInTilemap.Add(endPosition);
			
			if (_spawnBonus != null)
			{
				_tilesToMove.Add(_spawnBonus);
				_spawnBonus = null;
			}
			else
				_tilesToMove.Add(topTile);

			var startPositionInTilemap = _tilemap.GetCellCenterWorld(new Vector3Int(startPosition.x,
				startPosition.y, 0));
			var endPositionInTilemap = _tilemap.GetCellCenterWorld(endPosition);

			if (!isNewTile)
			{
				tilesShufflingAnimation.Join(_tileSmoothMovementManager.MoveTile(startPositionInTilemap, 
					endPositionInTilemap, _tilesToMove[_tilesToMove.Count-1].sprite));
			}
			else
				_latestTokenIndex.Add(_startPositionsInTilemap.Count - 1);
		}

		public void SpawnToken(List<TileData> selectedPosition, bool isBonus = false)
		{
			_tilesShufflingAnimation = DOTween.Sequence().Pause();

			_timeBetweenFallingTiles = 0.15f;
			_numberOfPermutations = 0;
			IsPermutationTokens = true;
			
			_emptyPositions.Clear();
			
			var emptyPositions = RemoveTiles(selectedPosition);

			foreach (var position in emptyPositions)
			{
				RemoveOrReplaceNearbyObstructions(position);
			}
			
			foreach (var position in emptyPositions)
			{
				_emptyPositions.Add(position);
			}
			
			_emptyPositions = _emptyPositions.OrderBy(emptyPosition => emptyPosition.y).ToList();

			EffectManager.PlayAnimationDestruction(_emptyPositions, _tilemapEffect, _effectTile);

			_levelManager.CheckPositionsForAssignmentsAddPoint(selectedPosition);
			
			if (!isBonus)
			{
				if (_emptyPositions.Count >= 5 && _emptyPositions.Count <= 7)
					_spawnBonus = _rocket;
				if (_emptyPositions.Count >= 8)
					_spawnBonus = _bomb;
			}

			_startPositionsInTilemap.Clear();
			_endPositionsInTilemap.Clear();
			_tilesToMove.Clear();
			_latestTokenIndex.Clear();

			foreach (var emptyPosition in _emptyPositions)
			{
				SetTile(emptyPosition, null);
			}

			var filledColumnsByX = new List<int>();

			for (var i = 0; i < _emptyPositions.Count; i++)
			{
				if (filledColumnsByX.Contains(_emptyPositions[i].x))
				{
					var allPassed = false;

					for (var j = i; j < _emptyPositions.Count; j++)
					{
						if (!filledColumnsByX.Contains(_emptyPositions[j].x))
						{
							i = j;
							allPassed = false;
							break;
						}
						else
							allPassed = true;
					}

					if (allPassed)
						break;
				}
				
				filledColumnsByX.Add(_emptyPositions[i].x);

				for (var y = _emptyPositions[i].y; y < _gridSize.y; y++)
				{
					var currentTile = _tilemap.GetTile<Tile>(new Vector3Int(_emptyPositions[i].x, y, 
						_emptyPositions[i].z));

					if (currentTile == _blockedTile)
						break;

					if (currentTile == null)
					{
						for (var j = y; j <= _gridSize.y; j++)
						{
							var nextTileI = _tilemap.GetTile<Tile>(new Vector3Int(_emptyPositions[i].x, j, 
								_emptyPositions[i].z));

							if (nextTileI == _blockedTile || j == _gridSize.y)
							{
								var previousTile = _tilemap.GetTile<Tile>(new Vector3Int(_emptyPositions[i].x, 
									j-1, _emptyPositions[i].z));
							
								if (previousTile != _blockedTile)
								{
									if (new Vector3Int(_emptyPositions[i].x, j - 1, _emptyPositions[i].z) ==
										new Vector3Int(_emptyPositions[i].x, y, _emptyPositions[i].z))
									{
										AddDataToBlendTile(new Vector3Int(_emptyPositions[i].x, j - 1,
												_emptyPositions[i].z), new Vector3Int(_emptyPositions[i].x,
												y, _emptyPositions[i].z), GetRandomTile(),
											_tilesShufflingAnimation, true);
									}
									else
									{
										if (_tilesIce.Contains(previousTile))
										{
											AddDataToBlendTile(new Vector3Int(_emptyPositions[i].x, j - 1,
													_emptyPositions[i].z), new Vector3Int(_emptyPositions[i].x, 
													j-1, _emptyPositions[i].z), GetRandomTile(),
												_tilesShufflingAnimation, true);
										}
										
										AddDataToBlendTile(new Vector3Int(_emptyPositions[i].x, j - 1,
												_emptyPositions[i].z), new Vector3Int(_emptyPositions[i].x,
												y, _emptyPositions[i].z), GetRandomTile(),
											_tilesShufflingAnimation, true);
									}

									break;
								}
							}
							
							if (nextTileI != null && !_tilesIce.Contains(nextTileI) && nextTileI != _blockedTile)
							{
								AddDataToBlendTile(new Vector3Int(_emptyPositions[i].x, j, _emptyPositions[i].z),
									new Vector3Int(_emptyPositions[i].x, y, _emptyPositions[i].z),
									nextTileI, _tilesShufflingAnimation);
								
								SetTile(new Vector3Int(_emptyPositions[i].x, j, _emptyPositions[i].z), null);
								break;
							}
						}
					}
					else
					{
						if (!_tilesIce.Contains(currentTile))
						{
							AddDataToBlendTile(new Vector3Int(_emptyPositions[i].x, y, _emptyPositions[i].z),
								new Vector3Int(_emptyPositions[i].x, y - 1, _emptyPositions[i].z),
								currentTile, _tilesShufflingAnimation);
						}
					}
				}
			}

			foreach (var startPosition in _startPositionsInTilemap)
			{
				SetTile(startPosition, null);
			}

			StartSwappingTokens();
		}

		private async UniTask StartSwappingTokens()
		{
			_tilesShufflingAnimation.Play();
			var lastTilesShufflingAnimation = DOTween.Sequence();

			foreach (var lastIndexToken in _latestTokenIndex)
			{
				var startPositionInTilemap = _tilemap.GetCellCenterWorld(new Vector3Int(
					_startPositionsInTilemap[lastIndexToken].x,
					_startPositionsInTilemap[lastIndexToken].y, 0));
				var endPositionInTilemap = _tilemap.GetCellCenterWorld(_endPositionsInTilemap[lastIndexToken]);
				var sprite = _tilesToMove[lastIndexToken];

				lastTilesShufflingAnimation.Insert(_timeBetweenFallingTiles, _tileSmoothMovementManager.MoveTile(new Vector3(
						startPositionInTilemap.x, startPositionInTilemap.y + 0.25f, 1), endPositionInTilemap, 
					sprite.sprite, true).SetSpeedBased().Pause());
				
				_timeBetweenFallingTiles += 0.12f;
			}
	
			await lastTilesShufflingAnimation.Play();
			
			for (var i = 0; i < _startPositionsInTilemap.Count; i++)
			{
				SetTile(_endPositionsInTilemap[i], _tilesToMove[i]);
			}

			_tileSmoothMovementManager.DeactivateTile();
			IsPermutationTokens = false;
		} 

		private void SetTile(Vector3Int position, Tile tile)
		{
			if (_blockedPositions.Contains(position))
				return;
			
			if (tile == _blockedTile)
			{
				SetRandomTile(position);
				return;
			}

			if (!_blockedPositions.Contains(position))
				_tilemap.SetTile(position, tile);
		}

		public void CheckTileForBonus(TileData tileData)
		{
			var tilesData = new List<TileData>();

			if (tileData.Tile == _rocket)
			{
				AudioManager.PlaySound("Rocket");
				
				var yPosition = tileData.Position.y;

				for (var x = 0; x < _gridSize.x; x++)
				{
					tilesData.Add(GetTileData(new Vector3Int(-x, yPosition, 0)));
				}
			}
			else if (tileData.Tile == _bomb)
			{
				AudioManager.PlaySound("Bomb");
				
				var bombPosition = new Vector3Int(tileData.Position.x, tileData.Position.y, 0);

				var yStartPosition = bombPosition.y + 2;
				var yEndPosition = bombPosition.y - 3;
				var xStartPosition = bombPosition.x - 2;
				var xEndPosition = bombPosition.x + 3;

				if (yStartPosition > _gridSize.y)
					yStartPosition = _gridSize.y;

				if (yEndPosition < 0)	
					yEndPosition = -1;

				if (xStartPosition < -_gridSize.x + 1)
					xStartPosition = -_gridSize.x + 1;

				if (xEndPosition > 0)
					xEndPosition = 1;

				for (var y = yStartPosition; y > yEndPosition; y--)
				{
					for (var x = xStartPosition; x < xEndPosition; x++)
					{
						tilesData.Add(GetTileData(new Vector3Int(x, y, 0)));
					}
				}
			}
			
			SpawnToken(tilesData, true);
		}

		private TileData GetTileData(Vector3Int position)
		{
			TileData tileData;
			
			var tile = _tilemap.GetTile<Tile>(position);

			if(tile == _stoneTile)
				tileData = new TileData(position, tile, TypeTile.Stone);
			else if(_tilesIce.Contains(tile))
				tileData = new TileData(position, tile, TypeTile.Ice);
			else
			{
				var typeTile = (TypeTile)_tiles.IndexOf(tile);
				tileData = new TileData(position, tile, typeTile);
			}

			return tileData;
		}
		
		private void RemoveOrReplaceNearbyObstructions(Vector3Int position)
		{
			var positionsLightObstacles = new List<Vector3Int>();
			var positionsComplicatedStone = new List<Vector3Int>();

			var positionsToCheck = new List<Vector3Int>();
			var tilesData = new List<TileData>();

			positionsToCheck.Add(Vector3IntExtension.GetRight(position));
			positionsToCheck.Add(Vector3IntExtension.GetLeft(position));
			positionsToCheck.Add(Vector3IntExtension.GetDown(position));
			positionsToCheck.Add(Vector3IntExtension.GetTop(position));

			foreach (var positionToCheck in positionsToCheck)
			{
				CheckPositionForObstacle(positionToCheck, ref positionsLightObstacles, 
					ref positionsComplicatedStone);
			}

			foreach (var positionComplicatedStone in positionsComplicatedStone)
			{
				_tilemap.SetTile(positionComplicatedStone, _stoneTile);
			}

			foreach (var positions in positionsLightObstacles)
			{
				tilesData.Add(GetTileData(positions));
			}
			
			_levelManager.CheckPositionsForAssignmentsAddPoint(tilesData);
			RemoveTiles(positionsLightObstacles);
		}

		private void CheckPositionForObstacle(Vector3Int position, ref List<Vector3Int> positions, 
			ref List<Vector3Int> positionsComplicatedStone)
		{
			var tileOnBaseTilemap = _tilemap.GetTile<Tile>(position);

			if (_tilesIce.Contains(tileOnBaseTilemap))
			{
				var numberTile = _tilesIce.FindIndex(tile => tile == tileOnBaseTilemap);
				_tilemap.SetTile(position, _tiles[numberTile]);
			}
			else if (tileOnBaseTilemap == _stoneTile)
				positions.Add(position);
			else if(tileOnBaseTilemap == _complicatedStoneTile)
				positionsComplicatedStone.Add(position);
		}

		public TileData GetTileDataFromPosition(Vector3 mouseWorldPosition)
		{
			var tilePosition = _tilemap.WorldToCell(mouseWorldPosition);
			
			var selectedTileInTilemapObstacles = _tilemap.GetTile<Tile>(tilePosition);

			if (_tilesIce.Contains(selectedTileInTilemapObstacles))
				return null;
			
			var selectedTile = _tilemap.GetTile<Tile>(tilePosition);

			TileData tileData = null;

			if (selectedTile != null)
			{
				if(selectedTile == _stoneTile)
					tileData = new TileData(tilePosition, selectedTile, TypeTile.Stone);
				else
				{
					var typeTile = (TypeTile)_tiles.IndexOf(selectedTile);
					tileData = new TileData(tilePosition, selectedTile, typeTile);
				}
			}

			if (!CheckSelectivityTile(selectedTile))
				return null;

			return tileData;
		}

		private List<Vector3Int> RemoveTiles(List<TileData> selectedTiles)
		{
			var emptyGridPositions = new List<Vector3Int>();
			
			foreach (var tile in selectedTiles)
			{
				var tilePosition = tile.Position;

				if (!_blockedPositions.Contains(tilePosition))
				{
					_tilemap.SetTile(tilePosition, null);
					emptyGridPositions.Add(tilePosition);
				}
			}

			return emptyGridPositions;
		}
		
		private void RemoveTiles(List<Vector3Int> positions)
		{
			foreach (var position in positions)
			{
				_tilemap.SetTile(position, null);
				_emptyPositions.Add(position);
			}
		}

		private bool CheckSelectivityTile(Tile tile)
		{
			if (_unselectedTiles.Contains(tile))
				return false;
			
			return true;
		}
		
		public void SetSelectedTile(Vector3Int position)
		{
			_tilemapSelected.SetTile(position, _selectedTile);
		}
		
		public void RemoveSelectedTile(Vector3Int position)
		{
			_tilemapSelected.SetTile(position, null);
		}
		
		public void ClearAllSelectedTilemap()
		{
			_tilemapSelected.ClearAllTiles();
		}
	}
}