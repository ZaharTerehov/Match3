
using System.Collections.Generic;
using Game.Level;
using UnityEngine;

namespace Game
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private BoardManager _boardManager;
		[SerializeField] private LevelManager _levelManager;
		[SerializeField] private Collider2D _board;
		
		private List<TileData> _selectedTiles = new List<TileData>();
		private Camera _camera;

		private void Start()
		{
			_camera = Camera.main;
		}

		private void Update()
		{
			if (Input.touchCount == 1 && !_boardManager.IsPermutationTokens)
			{
				var worldPoint = _camera.ScreenToWorldPoint(Input.touches[0].position);
				var position2D = new Vector2(worldPoint.x, worldPoint.y);
				var hit = Physics2D.Raycast(position2D, Vector2.zero);

				if (hit.collider == _board)
					SelectTiles(worldPoint);
			}
			else
			{
				if (_selectedTiles.Count < 1) 
					return;
				
				if (_levelManager.EnoughTokensToDestroy(_selectedTiles.Count))
				{
					AudioManager.PlaySound("DisappearanceTokens");
					
					_boardManager.SpawnToken(_selectedTiles);

					_levelManager.AddCountMove();
				}
					
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