
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Extensions;
using Game.Level;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LevelEditor.Scripts
{
	public class BoardManager : MonoBehaviour
	{
		[Header("Size board")]
		[SerializeField] private Vector2Int _gridSize;

		[Space]
		[Header("Tilemap and tiles")]
		[SerializeField] private Tilemap _tilemap;
		[SerializeField] private List<Tile> _tiles = new List<Tile>();
		
		[Space]
		[Header("Bonuses")]
		[SerializeField] private Tile _rocket;
		[SerializeField] private Tile _bomb;
		
		[Space]
		[Header("Obstacles")]
		[SerializeField] private Tile _stoneTile;
		[SerializeField] private Tile _complicatedStoneTile;
		
		[Space]
		[Header("Selected tile")]
		[SerializeField] private Tilemap _tilemapSelected;
		[SerializeField] private Tile _selectedTile;
		[SerializeField] private List<Tile> _unselectedTiles;

		[Space]
		[SerializeField] private List<Tile> _tilesIce = new List<Tile>();

		[Space]
		[Header("Ice and stone spawn chance")]
		[Range(1,99), SerializeField] private int _iceSpawnChance = 5;
		[Range(1,99), SerializeField] private int _stoneSpawnChance = 5;
		[Range(1,99), SerializeField] private int _complicatedStoneSpawnChance = 5;

		[Space]
		[SerializeField] private float _tileFallingSpeed = 0.2f;
		
		[SerializeField] private Tile _blockedTile;
		
		public List<Vector3Int> BlockedPositions = new List<Vector3Int>();

		private List<Vector3Int> _emptyPositions = new List<Vector3Int>();
		private List<TileData> _currentTiles = new List<TileData>();
		private Tile _spawnBonus;

		public void BlockOrUnblockTile(Vector3 worldPoint)
		{
			var tilePosition = _tilemap.WorldToCell(worldPoint);
			
			if (tilePosition.x > 0 || tilePosition.x < -6 || tilePosition.y < 0 || tilePosition.y > 10)
				return;

			var selectedTile = _tilemap.GetTile<Tile>(tilePosition);

			if (selectedTile != _blockedTile)
			{
				BlockedPositions.Add(new Vector3Int(tilePosition.x, tilePosition.y, 0));
				_tilemap.SetTile(tilePosition, _blockedTile);
			}
			else
			{
				BlockedPositions.Remove(tilePosition); 
				SetRandomTile(tilePosition);
			}
		}

		public void Init(int iceSpawnChance, int stoneSpawnChance, int complicatedStoneSpawnChance, 
			List<Vector3Int> blockedPosition)
		{
			BlockedPositions = blockedPosition;
			
			for (var x = 0; x < _gridSize.x; x++)
			{
				for (var y = 0; y < _gridSize.y; y++)
				{
					_currentTiles.Add(new TileData(new Vector3Int(-x, y, 0), null, TypeTile.Red));
				}
			}

			_iceSpawnChance = iceSpawnChance;
			_stoneSpawnChance = stoneSpawnChance;
			_complicatedStoneSpawnChance = complicatedStoneSpawnChance;
			
			GameBoardFilling();
		}
		
		private void GameBoardFilling()
		{
			_tilemap.ClearAllTiles();
			
			foreach (var tileData in _currentTiles)
			{
				var position = tileData.Position;
				SetRandomTile(position);
			}

			SettingBlockedTile(BlockedPositions);
		}

		private void SettingBlockedTile(List<Vector3Int> positions)
		{
			if (positions != null)
			{
				foreach (var position in positions)
				{
					_tilemap.SetTile(position, _blockedTile);
				}
			}
		}
		
		private Tile SetRandomTile(Vector3Int spawnPosition)
		{
			if (BlockedPositions != null)
			{
				if (BlockedPositions.Contains(spawnPosition))
					return null;
			}

			if (UnityEngine.Random.Range(1, 100) <= _stoneSpawnChance)
			{
				_tilemap.SetTile(spawnPosition, _stoneTile);
				return _stoneTile;
			}
			else if (UnityEngine.Random.Range(1, 100) <= _complicatedStoneSpawnChance)
			{
				_tilemap.SetTile(spawnPosition, _complicatedStoneTile);
				return _complicatedStoneTile;
			}
			else
			{
				var tile = _tiles[UnityEngine.Random.Range(0, _tiles.Count)];
				_tilemap.SetTile(spawnPosition, tile);
				
				if (UnityEngine.Random.Range(1, 100) <= _iceSpawnChance)
					_tilemap.SetTile(spawnPosition, _tilesIce[UnityEngine.Random.Range(0, _tilesIce.Count)]);
			}
			
			if (_spawnBonus != null)
			{
				_tilemap.SetTile(spawnPosition, _spawnBonus);
			
				var spawnBonus = _spawnBonus;
				_spawnBonus = null;
			
				return spawnBonus;
			}

			return null;
		}
		
		public async void SpawnToken(List<TileData> selectedPosition, bool isBonus = false)
		{
			_emptyPositions.Clear();
			
			var emptyPositions= RemoveTiles(selectedPosition);

			if (!isBonus)
			{
				if (emptyPositions.Count >= 5 && emptyPositions.Count <= 7)
					_spawnBonus = _rocket;
				if (emptyPositions.Count >= 8)
					_spawnBonus = _bomb;
			}
		
			foreach (var position in emptyPositions)
			{
				RemoveOrReplaceNearbyObstructions(position);
			}
			
			foreach (var position in emptyPositions)
			{
				_emptyPositions.Add(position);
			}
		
			var removedPositions = new List<Vector3Int>();
		
			while (_emptyPositions.Count > 0)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(_tileFallingSpeed));
				
				for (var i = 0; i < _emptyPositions.Count; i++)
				{
					var position = _emptyPositions[i];
		
					if (position.y == _gridSize.y)
					{
						var downPosition = Vector3IntExtension.GetDown(position);
		
						var spawnTile = SetRandomTile(downPosition);
		
						removedPositions.Add(position);
		
						if (!_tilesIce.Contains(spawnTile)) 
							continue;
						
						for (var j = 0; j <= position.y - 1; j++)
						{
							if(_tilemap.GetTile<Tile>(new Vector3Int(position.x,j,0)) == null)
								SetRandomTile(new Vector3Int(position.x, j, 0));
						}
					}
					else
					{
						position.y++;
						
						var topTile = _tilemap.GetTile<Tile>(position);
		
						if (_tilesIce.Contains(topTile))
						{
							for (var j = position.y; j < _gridSize.y; j++)
							{
								var nextTileIce = _tilemap.GetTile<Tile>(new Vector3Int(position.x,j,0));
								
								if (_tilesIce.Contains(nextTileIce)) 
									continue;
								
								var nextTile = _tilemap.GetTile<Tile>(new Vector3Int(position.x,j,0));
		
								SetTile(_emptyPositions[i], nextTile);
								SetTile(new Vector3Int(position.x,j,0), null);
								
								SetRandomTile(new Vector3Int(position.x,j,0));
		
								break;
							}
		
							var downPosition = Vector3IntExtension.GetDown(position);
		
							var tile = _tilemap.GetTile<Tile>(downPosition);
							
							if(tile == null)
								SetRandomTile(downPosition);
							
							removedPositions.Add(_emptyPositions[i]);
							break;
						}
						
						if (topTile != null)
						{
							SetTile(position, null);
							SetTile(_emptyPositions[i], topTile);
						}
						
						else
						{
							for (var y = position.y; y <= _gridSize.y; y++)
							{
								var fallingTileNext = _tilemap.GetTile<Tile>(new Vector3Int(position.x, y, 0));
		
								if (y == _gridSize.y)
								{
									SetRandomTile(_emptyPositions[i]);
									break;
								}
		
								if (fallingTileNext == null) 
									continue;
								
								SetTile(_emptyPositions[i], fallingTileNext);
								break;
							}
						}
		
						_emptyPositions[i] = position;
					}
				}
		
				foreach (var position in removedPositions)
				{
					_emptyPositions.Remove(position);
				}
				
				removedPositions.Clear();
			}
		}
		
		private void SetTile(Vector3Int position, Tile tile)
		{
			if (BlockedPositions.Contains(position))
				return;
			
			if (tile == _blockedTile)
			{
				SetRandomTile(position);
				return;
			}

			if (!BlockedPositions.Contains(position))
				_tilemap.SetTile(position, tile);
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
			
			RemoveTiles(positionsLightObstacles);
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
		
		public void CheckTileForBonus(TileData tileData)
		{
			var tilesData = new List<TileData>();

			if (tileData.Tile == _rocket)
			{
				var yPosition = tileData.Position.y;

				for (var x = 0; x < _gridSize.x; x++)
				{
					tilesData.Add(GetTileData(new Vector3Int(-x, yPosition, 0)));
				}
			}
			else if (tileData.Tile == _bomb)
			{
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
		
		private List<Vector3Int> RemoveTiles(List<TileData> selectedTiles)
		{
			var emptyGridPositions = new List<Vector3Int>();
			
			foreach (var tile in selectedTiles)
			{
				var tilePosition = tile.Position;
		
				if (!BlockedPositions.Contains(tilePosition))
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