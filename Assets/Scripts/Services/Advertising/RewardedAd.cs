
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

namespace Services.Advertising
{
	public class RewardedAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
	{
        [SerializeField] private Button _showAdButton;
        [SerializeField] private string _androidAdUnitId = "Rewarded_Android";
        [SerializeField] private string _iOSAdUnitId = "Rewarded_iOS";
        
        public event Action AdShowComplete;
    
        private string _adUnitId;
 
        private void Awake()
        {   

            #if UNITY_IOS
                _adUnitId = _iOSAdUnitId;
            #elif UNITY_ANDROID || UNITY_EDITOR
                _adUnitId = _androidAdUnitId;
            #endif
            
            _showAdButton.interactable = false;
        }
        
        public void LoadAd()
        {
            Advertisement.Load(_adUnitId, this);
        }
        
        public void OnUnityAdsAdLoaded(string adUnitId)
        {
            if (adUnitId.Equals(_adUnitId))
            {
                _showAdButton.onClick.AddListener(ShowAd);
                _showAdButton.interactable = true;
            }
        }

        private void ShowAd()
        {
            _showAdButton.interactable = false;
            Advertisement.Show(_adUnitId, this);
        }
        
        public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
        {
            if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
            {
                AdShowComplete?.Invoke();
                Advertisement.Load(_adUnitId, this);
            }
        }
        
        public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
        {
            Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        }
     
        public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
        {
            Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        }
     
        public void OnUnityAdsShowStart(string adUnitId) { }
        public void OnUnityAdsShowClick(string adUnitId) { }
    }
}