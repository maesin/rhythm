using GoogleMobileAds.Api;
using UnityEngine;

/// <summary>
/// プロモーションマネージャ.
/// </summary>
public class PromotionManager : MonoBehaviour
{
    /// <summary>
    /// グローバル設定.
    /// </summary>
    public GlobalSettings Globals;

    /// <summary>
    /// AdMob Banner View.
    /// </summary>
    BannerView AdMobBanner;

    /// <summary>
    /// AdMob Interstitial View.
    /// </summary>
    InterstitialAd AdMobInterstitial;

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Start()
    {
        // AdMob.
        if (!string.IsNullOrEmpty(Globals.AdMobBannerUnitId))
        {
            AdMobBanner = new BannerView(Globals.AdMobBannerUnitId, AdSize.SmartBanner, AdPosition.Bottom);
        }

        if (!string.IsNullOrEmpty(Globals.AdMobInterstitialUnitId))
        {
            AdMobInterstitial = new InterstitialAd(Globals.AdMobInterstitialUnitId);
        }

        if (AdMobBanner != null || AdMobInterstitial != null)
        {

            var builder = new AdRequest.Builder();

#if DEVELOPMENT_BUILD
            builder.AddTestDevice (AdRequest.TestDeviceSimulator);

            foreach (var id in Globals.AdMobTestDeviceIds) {
                builder.AddTestDevice (id);
            }
#endif

            AdRequest request = builder.Build();

            if (AdMobBanner != null)
            {
                AdMobBanner.LoadAd(request);
                AdMobBanner.Hide();
            }

            if (AdMobInterstitial != null)
            {
                AdMobInterstitial.LoadAd(request);
            }
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
		
    }

    /// <summary>
    /// バナー表示コールバック.
    /// </summary>
    public void OnBannerShow()
    {
        if (AdMobBanner != null)
        {
            AdMobBanner.Show();
        }
    }

    /// <summary>
    /// バナー非表示コールバック.
    /// </summary>
    public void OnBannerHide()
    {
        if (AdMobBanner != null)
        {
            AdMobBanner.Hide();
        }
    }

    /// <summary>
    /// インタースティシャル表示コールバック.
    /// </summary>
    public void OnInterstitialShow()
    {
        if (AdMobInterstitial != null)
        {
            AdMobInterstitial.Show();
        }
    }
}
