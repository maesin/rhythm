using UnityEngine;

/// <summary>
/// スタートマネージャ.
/// </summary>
public class StartManager : MonoBehaviour
{
    /// <summary>
    /// グローバル設定.
    /// </summary>
    public GlobalSettings Globals;

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Start()
    {
        var unknown = string.IsNullOrEmpty(Globals.RecommendedMusicScores);
        var unreachable = Application.internetReachability == NetworkReachability.NotReachable;

        if (unknown || unreachable)
        {
            SceneUtils.LoadSettings();
        }
        else
        {
            SceneUtils.LoadDownload();
        }
    }
}
