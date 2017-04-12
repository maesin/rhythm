using UnityEngine;

/// <summary>
/// プレイヤー設定.
/// </summary>
public class Settings
{
    /// <summary>
    /// 設定キーのプレフィックス.
    /// </summary>
    const string KeyPrefix = "Settings";

    /// <summary>
    /// 整数値を取得.
    /// </summary>
    static int GetInt(string key, int defaultValue)
    {
        return PlayerPrefs.GetInt(KeyPrefix + "." + key, defaultValue);
    }

    /// <summary>
    /// 整数値を設定.
    /// </summary>
    static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(KeyPrefix + "." + key, value);
    }

    /// <summary>
    /// 文字列値を取得.
    /// </summary>
    static string GetString(string key, string defaultValue)
    {
        return PlayerPrefs.GetString(KeyPrefix + "." + key, defaultValue);
    }

    /// <summary>
    /// 文字列値を設定.
    /// </summary>
    static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(KeyPrefix + "." + key, value);
    }

    /// <summary>
    /// 譜面 ID を取得.
    /// </summary>
    public static string MusicScoreId
    {
        get
        {
            return GetString("MusicScoreId", null);
        }
        set
        {
            SetString("MusicScoreId", value);
        }
    }

    /// <summary>
    /// Beats Per Minute を取得.
    /// </summary>
    public static int BPM
    {
        get
        {
            return GetInt("BPM", 150);
        }
        set
        {
            SetInt("BPM", value);
        }
    }

    /// <summary>
    /// スピードを取得.
    /// </summary>
    public static int Speed
    {
        get
        {
            return GetInt("Speed", 1);
        }
        set
        {
            SetInt("Speed", value);
        }
    }

    /// <summary>
    /// ミラーなら true.
    /// </summary>
    public static bool Mirror
    {
        get
        {
            return GetInt("Mirror", 0) > 0;
        }
        set
        {
            SetInt("Mirror", value ? 1 : 0);
        }
    }

    /// <summary>
    /// 自動プレイなら true.
    /// </summary>
    public static bool AutoPlay
    {
        get
        {
            return GetInt("AutoPlay", 0) > 0;
        }
        set
        {
            SetInt("AutoPlay", value ? 1 : 0);
        }
    }

    /// <summary>
    /// 保存.
    /// </summary>
    public static void Save()
    {
        PlayerPrefs.Save();
    }
}
