
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Game.Pool;

namespace Game
{
	public class TileSmoothMovementManager : MonoBehaviour
	{
		[SerializeField] private Transform _container;

		[Space] 
		[SerializeField] private float _speedMoving;

		[Space]
		[SerializeField] private List<TileSmooth> _prefabs = new List<TileSmooth>();

		[Space]
		[SerializeField] private int _countPrefabs;

		private List<Pool<TileSmooth>> _pools = new List<Pool<TileSmooth>>();

		private void Start()
		{
			foreach (var prefab in _prefabs)
			{
				_pools.Add(new Pool<TileSmooth>(prefab, _countPrefabs, _container));
			}
		}
		
		public Tween MoveTile(Vector3 spawnPosition, Vector3 endPosition, Sprite sprite, bool isNewTile = false)
		{
			TileSmooth tileSmooth = null;

			for (var i = 0; i < _prefabs.Count; i++)
			{
				if (sprite != _prefabs[i].Sprite) 
					continue;
				
				tileSmooth = _pools[i].GetFreeElement();
			}

			if (tileSmooth == null)
				return null;

			var animationTile = DOTween.Sequence().Pause();

			if (isNewTile)
			{
				tileSmooth.SpriteRenderer.enabled = false;

				animationTile.Insert(0,
					DOVirtual.DelayedCall(0.1f, () => { tileSmooth.SpriteRenderer.enabled = true; }).Pause()).Pause();
			}
			
			animationTile.Join(tileSmooth.MoveToPosition(spawnPosition, endPosition, Random.Range(
				_speedMoving - 0.4f, _speedMoving)).SetSpeedBased());

			return animationTile;
		}

		public void DeactivateTile()
		{
			foreach (var pool in _pools)
			{
				pool.DeactivateAllElements();
			}
		}
	}
}