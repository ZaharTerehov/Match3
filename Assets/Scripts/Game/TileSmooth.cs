
using DG.Tweening;
using UnityEngine;

namespace Game
{
	public class TileSmooth : MonoBehaviour
	{
		public Sprite Sprite;
		public SpriteRenderer SpriteRenderer;

		public Tween MoveToPosition(Vector3 startPosition, Vector3 endPosition, float speedMoving)
		{
			transform.position = startPosition;
			return transform.DOMove(endPosition, speedMoving).SetSpeedBased().SetEase(Ease.OutBack).Pause();
		}
	}
}