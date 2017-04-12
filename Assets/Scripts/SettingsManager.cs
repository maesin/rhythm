using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 設定マネージャ.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    /// <summary>
    /// Beats Per Minute.
    /// </summary>
    public InputField BPM;

    /// <summary>
    /// 譜面.
    /// </summary>
    public Dropdown MusicScore;

    /// <summary>
    /// スピード.
    /// </summary>
    public Dropdown Speed;

    /// <summary>
    /// ミラーかどうか.
    /// </summary>
    public Toggle Mirror;

    /// <summary>
    /// 自動プレイかどうか.
    /// </summary>
    public Toggle AutoPlay;

    /// <summary>
    /// リソースマネージャ.
    /// </summary>
    public ResourceManager ResourceManager;

    /// <summary>
    /// ロードされた譜面.
    /// </summary>
    List<MusicScore> LoadedMusicScores;

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Start()
    {
        // BPM.
        BPM.text = Settings.BPM.ToString();

        // 譜面.
        var scoreTitles = new List<string>();
        var scoreDefault = -1;

        LoadedMusicScores = ResourceManager.LoadMusicScores();

        for (var i = 0; i < LoadedMusicScores.Count; i++)
        {
            if (LoadedMusicScores[i].Id.Equals(Settings.MusicScoreId))
            {
                scoreDefault = i;
            }

            scoreTitles.Add(LoadedMusicScores[i].Title);
        }

        if (scoreDefault == -1)
        {
            scoreDefault = 0;
            Settings.MusicScoreId = LoadedMusicScores[0].Id;
        }

        MusicScore.AddOptions(scoreTitles);
        MusicScore.value = scoreDefault;

        // スピード.
        Speed.value = Settings.Speed - 1;

        // ミラー.
        Mirror.isOn = Settings.Mirror;

        // 自動プレイ.
        AutoPlay.isOn = Settings.AutoPlay;
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
		
    }

    /// <summary>
    /// BPM 変更コールバック.
    /// </summary>
    public void OnBPMValueChanged(string bpm)
    {
        int parsed;

        if (int.TryParse(bpm, out parsed))
        {
            Settings.BPM = parsed;
        }
    }

    /// <summary>
    /// 譜面変更コールバック.
    /// </summary>
    public void OnMusicScoreValueChanged(int i)
    {
        Settings.MusicScoreId = LoadedMusicScores[i].Id;
    }

    /// <summary>
    /// スピード変更コールバック.
    /// </summary>
    public void OnSpeedValueChanged(int i)
    {
        Settings.Speed = i + 1;
    }

    /// <summary>
    /// ミラー変更コールバック.
    /// </summary>
    public void OnMirrorValueChanged(bool value)
    {
        Settings.Mirror = value;
    }

    /// <summary>
    /// 自動プレイ変更コールバック.
    /// </summary>
    public void OnAutoPlayValueChanged(bool value)
    {
        Settings.AutoPlay = value;
    }

    /// <summary>
    /// プレイクリックコールバック.
    /// </summary>
    public void OnPlayClick()
    {
        Settings.Save();
        SceneUtils.LoadPlay();
    }
}
