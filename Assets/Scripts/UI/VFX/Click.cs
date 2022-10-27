
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace UI.VFX
{
	public class Click : MonoBehaviour
	{
		[SerializeField] private ParticleSystem _particleSystem;

		public async void Play()
		{
			_particleSystem.gameObject.transform.localScale = new Vector3(2,2,2);
			_particleSystem.Emit(1);

			await ChangeSize();

			_particleSystem.Stop();
			gameObject.SetActive(false);
		}
		
		private Tween ChangeSize()
		{
			return _particleSystem.gameObject.transform.DOScale(new Vector3(0,0,0), 2);
		}
	}
}