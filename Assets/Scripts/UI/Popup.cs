
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI
{
	public class Popup : MonoBehaviour
	{
		[SerializeField] private Animation _animation;
		[SerializeField] private AnimationClip _animationClose;
		[SerializeField] private AnimationClip _animationOpen;

		public bool UseFadePanel;

		public virtual async UniTask OpenStart()
		{
			gameObject.SetActive(true);

			if(_animationOpen != null)
				await PlayAnimationClip(_animationOpen);
			
			OpenCompleted();
		}

		protected virtual void OpenCompleted()
		{
			
		}
		
		public virtual async UniTask CloseStart()
		{
			if(_animationClose != null)
				await PlayAnimationClip(_animationClose);
			
			gameObject.SetActive(false);

			CloseCompleted();
		}
		
		private async UniTask PlayAnimationClip(AnimationClip animationClip)
		{
			_animation.clip = animationClip;
			_animation.Play();

			while (_animation.isPlaying)
			{
				await UniTask.Yield();
			}
		}
		
		protected virtual void CloseCompleted()
		{
			
		}
	}
}