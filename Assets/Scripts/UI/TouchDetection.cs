
using Game.Pool;
using UI.VFX;
using UnityEngine;

namespace UI
{
	public class TouchDetection : MonoBehaviour
	{
		[SerializeField] private Click _particleEffects;
		[SerializeField] private Transform _container;

		private Pool<Click> _pool;
		
		private Camera _camera;

		private void Start()
		{
			_camera = Camera.main;
			
			_pool = new Pool<Click>(_particleEffects, 10, _container);
		}
		
		private void Update()
		{
			if (!Input.GetMouseButtonDown(0)) 
				return;
			
			var worldPoint = _camera.ScreenToWorldPoint(Input.touches[0].position);
			var clickEffects = _pool.GetFreeElement();
			
			clickEffects.transform.position = new Vector3(worldPoint.x, worldPoint.y, 0);
			clickEffects.Play();
		}
	}
}