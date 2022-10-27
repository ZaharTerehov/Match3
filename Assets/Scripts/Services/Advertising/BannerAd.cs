
using UnityEngine;
using UnityEngine.Advertisements;

namespace Services.Advertising
{
	public class BannerAd : MonoBehaviour
	{
        [SerializeField] private BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;
 
        [Space]
        [SerializeField] private string _androidAdUnitId = "Banner_Android";
        [SerializeField] private string _iOSAdUnitId = "Banner_iOS";
        
        private string _adUnitId;
     
        private void Start()
        {
            #if UNITY_IOS
                _adUnitId = _iOSAdUnitId;
            #elif UNITY_ANDROID || UNITY_EDITOR
                _adUnitId = _androidAdUnitId;
            #endif

            Advertisement.Banner.SetPosition(_bannerPosition);
        }
        
        public void LoadBanner()
        {
            var options = new BannerLoadOptions
            {
                loadCallback = OnBannerLoaded,
                errorCallback = OnBannerError
            };

            Advertisement.Banner.Load(_adUnitId, options);
        }

        private void OnBannerLoaded() { }
        
        private void OnBannerError(string message)
        {
            Debug.Log($"Banner Error: {message}");
        }
        
        public void OnShowBannerAd()
        {
            var options = new BannerOptions
            {
                clickCallback = OnBannerClicked,
                hideCallback = OnBannerHidden,
                showCallback = OnBannerShown
            };

            Advertisement.Banner.Show(_adUnitId, options);
        }

        public void OnHideBannerAd()
        {
            Advertisement.Banner.Hide();
        }
     
        private void OnBannerClicked() { }
        private void OnBannerShown() { }
        private void OnBannerHidden() { }
    }
}