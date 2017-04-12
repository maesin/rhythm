using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// ダウンロードマネージャ.
/// </summary>
public class DownloadManager : MonoBehaviour
{
    /// <summary>
    /// 進捗バー.
    /// </summary>
    public Slider Slider;

    /// <summary>
    /// 進捗率.
    /// </summary>
    public Text Label;

    /// <summary>
    /// リソースマネージャ.
    /// </summary>
    public ResourceManager ResourceManager;

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Start()
    {
        ResourceManager.Download(progress =>
        {
            Slider.value = Slider.maxValue * progress;
            Label.text = string.Format("{0}%", Mathf.Round(progress * 100));

            if (progress == 1)
            {
                StartCoroutine(SceneUtils.LoadSettingsWithDelay(1));
            }
        });
    }
}
