
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Popup
{
	[RequireComponent(typeof(Animation))]
	public class FloatingText : MonoBehaviour
	{
		private Animation _animation;

		private void Awake()
		{
			_animation = gameObject.GetComponent<Animation>();
		}

		public async UniTask ShowPopupText()
		{
			gameObject.SetActive(true);
			_animation.Play();

			while (_animation.isPlaying)
			{
				await UniTask.Yield();
			}

			if(!_animation.isPlaying)
				gameObject.SetActive(false);
		}	
	}
}