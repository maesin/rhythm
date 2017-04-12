using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーンユーティリティ.
/// </summary>
public class SceneUtils
{
    /// <summary>
    /// シーンをロード.
    /// </summary>
    static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Download シーンをロード.
    /// </summary>
    public static void LoadDownload()
    {
        LoadScene("Download");
    }

    /// <summary>
    /// Settings シーンをロード.
    /// </summary>
    public static void LoadSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    /// <summary>
    /// Settings シーンを遅延ロード.
    /// </summary>
    public static IEnumerator LoadSettingsWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadSettings();
    }

    /// <summary>
    /// Play シーンをロード.
    /// </summary>
    public static void LoadPlay()
    {
        SceneManager.LoadScene("Play");
    }
}
