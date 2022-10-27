
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
	public static class EffectManager
	{
		public static async void PlayAnimationDestruction(List<Vector3Int> positions, Tilemap tilemap, TileBase tile)
		{
			foreach (var position in positions)
			{
				tilemap.SetTile(position, tile);
			}

			await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
			tilemap.ClearAllTiles();
		}
	}
}