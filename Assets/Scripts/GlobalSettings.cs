using UnityEngine;

/// <summary>
/// グローバル設定.
/// </summary>
[CreateAssetMenu]
public class GlobalSettings : ScriptableObject
{
    /// <summary>
    /// おすすめ譜面 URL.
    /// </summary>
    public string RecommendedMusicScores;

    /// <summary>
    /// AdMob Banner Unit ID for Android.
    /// </summary>
    public string AdMobBannerUnitIdForAndroid;

    /// <summary>
    /// AdMob Banner Unit ID for iOS.
    /// </summary>
    public string AdMobBannerUnitIdForIOS;

    /// <summary>
    /// AdMob Interstitial Unit ID for Android.
    /// </summary>
    public string AdMobInterstitialUnitIdForAndroid;

    /// <summary>
    /// AdMob Interstitial Unit ID for iOS.
    /// </summary>
    public string AdMobInterstitialUnitIdForIOS;

    /// <summary>
    /// AdMob Test Device IDs.
    /// </summary>
    public string[] AdMobTestDeviceIds;

    /// <summary>
    /// AdMob Banner Unit ID.
    /// </summary>
    public string AdMobBannerUnitId
    {
        get
        {
#if UNITY_ANDROID
            return AdMobBannerUnitIdForAndroid;
#elif UNITY_IOS
            return AdMobBannerUnitIdForIOS;
#else
            return null;
#endif
        }
    }

    /// <summary>
    /// AdMob Interstitial Unit ID.
    /// </summary>
    public string AdMobInterstitialUnitId
    {
        get
        {
#if UNITY_ANDROID
            return AdMobInterstitialUnitIdForAndroid;
#elif UNITY_IOS
            return AdMobInterstitialUnitIdForIOS;
#else
            return null;
#endif
        }
    }
}
