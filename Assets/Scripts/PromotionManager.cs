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
        AdMobBanner = new BannerView(Globals.AdMobBannerUnitId, AdSize.SmartBanner, AdPosition.Bottom);
        AdMobInterstitial = new InterstitialAd(Globals.AdMobInterstitialUnitId);

        var builder = new AdRequest.Builder();

#if DEVELOPMENT_BUILD
        builder.AddTestDevice (AdRequest.TestDeviceSimulator);

        foreach (var id in Globals.AdMobTestDeviceIds) {
            builder.AddTestDevice (id);
        }
#endif

        AdRequest request = builder.Build();

        AdMobBanner.LoadAd(request);
        AdMobInterstitial.LoadAd(request);

        OnBannerHide();
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
        AdMobBanner.Show();
    }

    /// <summary>
    /// バナー非表示コールバック.
    /// </summary>
    public void OnBannerHide()
    {
        AdMobBanner.Hide();
    }

    /// <summary>
    /// インタースティシャル表示コールバック.
    /// </summary>
    public void OnInterstitialShow()
    {
        AdMobInterstitial.Show();
    }
}
