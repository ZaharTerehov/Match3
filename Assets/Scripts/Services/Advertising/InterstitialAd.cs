
using UnityEngine;
using UnityEngine.Advertisements;

namespace Services.Advertising
{
	public class InterstitialAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
	{
		[SerializeField] private string _androidAdUnitId = "Interstitial_Android";
		[SerializeField] private string _iOsAdUnitId = "Interstitial_iOS";
		
		private string _adUnitId;
 
		private void Awake()
		{
			#if UNITY_IOS
				_adUnitId = _iOsAdUnitId;
			#elif UNITY_ANDROID || UNITY_EDITOR
				_adUnitId = _androidAdUnitId;
			#endif
		}
		
		public void LoadAd()
		{ 
			Advertisement.Load(_adUnitId, this);
		}
		
		public void OnShowAd()
		{
			Advertisement.Show(_adUnitId, this);
		}
		
		public void OnUnityAdsAdLoaded(string adUnitId) { }
 
		public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
		{
			Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
		}
 
		public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
		{
			Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
		}
 
		public void OnUnityAdsShowStart(string adUnitId) { }
		public void OnUnityAdsShowClick(string adUnitId) { }
		public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState) { }
	}
}