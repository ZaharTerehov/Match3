
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor.Scripts
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private BoardManager _boardManager;
		[SerializeField] private LevelEditor _levelEditor;

		private List<TileData> _selectedTiles = new List<TileData>();
		private Camera _camera;
		
		private void Start()
		{
			_camera = Camera.main;
		}
		
		private void Update()
		{
			var worldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
			
			if (Input.GetMouseButtonDown(0))
			{
				if(!_levelEditor.Toggle)
					_boardManager.BlockOrUnblockTile(worldPoint);
			}

			if (Input.GetMouseButton(0))
			{
				if(_levelEditor.Toggle)
					SelectTiles(worldPoint);
			}
			else
			{
				if (_selectedTiles.Count < 1) 
					return;
				
				if (_levelEditor.CountTokensNeededToDestroy <= _selectedTiles.Count)
					_boardManager.SpawnToken(_selectedTiles);
			
				_selectedTiles.Clear();
				_boardManager.ClearAllSelectedTilemap();
			}
		}

		private void SelectTiles(Vector3 worldPoint)
		{
			var selectedTileData = _boardManager.GetTileDataFromPosition(worldPoint);
		
			if (selectedTileData == null) 
				return;
			
			if (_selectedTiles.Count == 0)
			{
				_selectedTiles.Add(selectedTileData);
		
				_boardManager.SetSelectedTile(selectedTileData.Position);
		
				_boardManager.CheckTileForBonus(selectedTileData);
			}
			else
			{
				var lastTileData = _selectedTiles[_selectedTiles.Count - 1];
		
				if (!_selectedTiles.Contains(selectedTileData))
				{
					if (!lastTileData.IsTileNeighbor(selectedTileData)) 
						return;
					
					_selectedTiles.Add(selectedTileData);
					_boardManager.SetSelectedTile(selectedTileData.Position);
				}
				else
				{
					if (lastTileData == selectedTileData) 
						return;
		
					if (selectedTileData != _selectedTiles[_selectedTiles.Count - 2]) 
						return;
					
					_boardManager.RemoveSelectedTile(lastTileData.Position);
					_selectedTiles.Remove(lastTileData);
				}
			}
		}
	}
}