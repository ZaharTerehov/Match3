
using System;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Services.Advertising
{
	public class AdvertisingManager : MonoBehaviour, IUnityAdsInitializationListener
	{
		[SerializeField] private string _androidAdUnitId = "4911517";
		[SerializeField] private string _iOsAdUnitId = "4911516";
		
		[SerializeField] private bool _testMode = true;

		[Space]
		[SerializeField] private RewardedAd _rewardedAd; 
		[SerializeField] private InterstitialAd _interstitialAd; 
		[SerializeField] private BannerAd _bannerAd;

		private static AdvertisingManager _instance;
		
		private string _adUnitId;
		
		private void Awake()
		{
			_instance = this;
			
			InitializeAds();
		}

		public static void ShowBannerAd()
		{
			_instance._bannerAd.OnShowBannerAd();
		}

		public static void HideBannerAd()
		{
			_instance._bannerAd.OnHideBannerAd();
		}

		public static void ShowInterstitialAd()
		{
			_instance._interstitialAd.OnShowAd();
		}

		public static void ShowRewardedAd(Action callback)
		{
			_instance._rewardedAd.AdShowComplete += callback;
		}
 
		private void InitializeAds()
		{
			#if UNITY_IOS
				_adUnitId = _iOsAdUnitId;
			#elif UNITY_ANDROID || UNITY_EDITOR
				_adUnitId = _androidAdUnitId;
			#endif
			
			Advertisement.Initialize(_adUnitId, _testMode, this);
		}
 
		public void OnInitializationComplete()
		{
			_rewardedAd.LoadAd();
			_interstitialAd.LoadAd();
			_bannerAd.LoadBanner();
		}
 
		public void OnInitializationFailed(UnityAdsInitializationError error, string message)
		{
			Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
		}
	}
}