using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// プレイマネージャ.
/// </summary>
/// <remarks>
/// 譜面のロード、判定、エフェクトなどを管理する.
/// </remarks>
public class PlayManager : MonoBehaviour
{
    /// <summary>
    /// リズムアイコン.
    /// </summary>
    public Icon Icon;

    /// <summary>
    /// リズムアイコンのスピードと到着までの所要時間.
    /// </summary>
    public float[] IconSpeedSeconds;

    /// <summary>
    /// リボン.
    /// </summary>
    public Ribbon Ribbon;

    /// <summary>
    /// レーン.
    /// </summary>
    public GameObject[] Lanes;

    /// <summary>
    /// 判定結果.
    /// </summary>
    public SpriteRenderer Judged;

    /// <summary>
    /// 判定結果の生成から表示までの所要時間.
    /// </summary>
    public float JudgedRequired = 0.1f;

    /// <summary>
    /// 判定結果の表示から削除までの持続時間.
    /// </summary>
    public float JudgedDuration = 0.2f;

    /// <summary>
    /// 打鍵の正確さを表すレーティング.
    /// </summary>
    public Rating[] Ratings;

    /// <summary>
    /// タッチサウンド.
    /// </summary>
    public AudioClip TouchSound;

    /// <summary>
    /// 一時停止で表示するメニュー.
    /// </summary>
    public GameObject Menu;

    /// <summary>
    /// BPM ラベル.
    /// </summary>
    public Text LabelBpm;

    /// <summary>
    /// 時間ラベル.
    /// </summary>
    public Text LabelTime;

    /// <summary>
    /// スコアラベル.
    /// </summary>
    public Text LabelScore;

    /// <summary>
    /// タイムマネージャ.
    /// </summary>
    public TimeManager TimeManager;

    /// <summary>
    /// ビートマネージャ.
    /// </summary>
    public BeatManager BeatManager;

    /// <summary>
    /// リソースマネージャ.
    /// </summary>
    public ResourceManager ResourceManager;

    /// <summary>
    /// リズムアイコンの生成から到着までの所要時間.
    /// </summary>
    float IconRequired;

    /// <summary>
    /// リズムアイコンのキャッシュ.
    /// </summary>
    List<Icon> Icons;

    /// <summary>
    /// リボンのキャッシュ.
    /// </summary>
    List<Ribbon> Ribbons;

    /// <summary>
    /// ロードしたノート.
    /// </summary>
    List<Note> Notes;

    /// <summary>
    /// サウンドエフェクト.
    /// </summary>
    Dictionary<AudioClip, AudioSource> SoundEffects;

    /// <summary>
    /// 現在のスコア.
    /// </summary>
    int Score;

    /// <summary>
    /// 判定結果を表示する時間.
    /// </summary>
    float JudgedTime;

    /// <summary>
    /// 判定結果を表示する時のスケール.
    /// </summary>
    Vector3 JudgedScale;

    /// <summary>
    /// 終了なら true.
    /// </summary>
    float Finished;

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Start()
    {
        // スピードからアイコンが到着するまでの時間を取得.
        IconRequired = IconSpeedSeconds[Settings.Speed - 1];

        // アイコンをキャッシュ.
        Icons = new List<Icon>();

        for (var i = 0; i < Lanes.Length * 10; i++)
        {
            var icon = Instantiate(Icon) as Icon;
            icon.Start(0);
            Icons.Add(icon);
        }

        // リボンをキャッシュ.
        Ribbons = new List<Ribbon>();

        for (var i = 0; i < Lanes.Length * 5; i++)
        {
            var ribbon = Instantiate(Ribbon) as Ribbon;
            ribbon.Start(0);
            Ribbons.Add(ribbon);
        }

        // サウンドエフェクトをキャッシュ.
        SoundEffects = new Dictionary<AudioClip, AudioSource>();

        var clips = new List<AudioClip>();
        clips.Add(TouchSound);

        foreach (var rating in Ratings)
        {
            clips.Add(rating.HitSound);
        }

        foreach (var clip in clips)
        {
            if (clip != null && !SoundEffects.ContainsKey(clip))
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.clip = clip;
                SoundEffects.Add(source.clip, source);
            }
        }

        // 判定結果のスケールを保持.
        JudgedScale = Judged.transform.localScale;

        // ロード.
        Load();
    }

    /// <summary>
    /// ロード.
    /// </summary>
    /// <remarks>
    /// 譜面やスコア、画面の初期化などを行う.
    /// </remarks>
    void Load()
    {
        // 譜面をロード.
        var loaded = ResourceManager.LoadMusicScore(Settings.MusicScoreId);

        // ノート初期化.
        Notes = loaded.Notes;

        if (Settings.Mirror)
        {
            foreach (var note in Notes)
            {
                note.Lane = Lanes.Length + 1 - note.Lane;
            }
        }



        // アイコン初期化.
        foreach (var icon in Icons)
        {
            icon.Note = null;
        }

        // リボン初期化.
        foreach (var ribbon in Ribbons)
        {
            ribbon.Head = null;
            ribbon.Tail = null;
        }

        // スコア初期化.
        Score = 0;

        // レーティング初期化.
        JudgedTime = -1;

        // ラベル初期化.
        LabelBpm.text = Settings.BPM.ToString();
        LabelScore.text = Score.ToString();

        // ビートマネージャ初期化.
        BeatManager.Count = 0;

        // タイムマネージャ初期化.
        TimeManager.Time = 0;
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        // 新しいノートをアイコンに設定.
        if (Notes.Count > 0)
        {
            var first = Notes[0];

            if (first.Time <= TimeManager.Time + IconRequired)
            {
                var icon = Icons.Find(a => !a.IsAlive());

                if (icon == null)
                {
                    Debug.LogError("Icons was depleted.");
                    return;
                }

                var lane = Lanes[first.Lane - 1];

                icon.Note = first;
                icon.Required = IconRequired;
                icon.Lane = lane;

                if (first.Type == NoteType.Long)
                {
                    var alive = Ribbons.Find(a => a.IsAlive() && a.Lane == lane);

                    if (alive == null)
                    {
                        // ロングノートの場合.
                        var reborn = Ribbons.Find(a => !a.IsAlive());

                        if (reborn != null)
                        {
                            reborn.Head = first;
                            reborn.Tail = Notes.Find(a => a != first && a.Lane == first.Lane);
                            reborn.Required = IconRequired;
                            reborn.Lane = lane;
                        }
                    }
                }

                Notes.RemoveAt(0);
            }
        }
        else if (Icons.FindAll(a => a.IsAlive()).Count == 0)
        {
            // 終了したらリロード.
            if (Finished == 0)
            {
                Finished = TimeManager.Time;
            }
            else if (Finished + 2 <= TimeManager.Time)
            {
                Load();
                Finished = 0;
            }
        }

        // アイコンを更新.
        foreach (var icon in Icons)
        {
            icon.Update(TimeManager.Time);

            if (icon.IsAlive())
            {
                // 自動判定.
                Rating judged = null;

                if (Settings.AutoPlay)
                {
                    // 自動プレイ.
                    var rating = Judge(icon.Note.Time);

                    if (rating == Ratings[0])
                    {
                        judged = rating;
                    }
                }
                else if (icon.Note.Time < TimeManager.Time)
                {
                    // Miss 判定.
                    var rating = Judge(icon.Note.Time);

                    if (rating == Ratings[Ratings.Length - 1])
                    {
                        judged = rating;

                        if (icon.Note.Type == NoteType.Long)
                        {
                            // ロングノートの場合、相方を道連れにする.
                            var ribbon = Ribbons.Find(a =>
                            {
                                return a.Lane == icon.Lane &&
                                a.IsAlive() &&
                                a.Head == icon.Note;
                            });

                            ribbon.Tail.Rating = rating;
                        }
                    }
                }

                if (judged != null)
                {
                    icon.Note.Rating = judged;
                    AddScore(judged.Score);
                    ShowJudged(judged);
                    PlaySoundEffect(judged.HitSound);
                }
            }
        }

        // リボンを更新.
        foreach (var ribbon in Ribbons)
        {
            ribbon.Update(TimeManager.Time);
        }

        // 判定結果を更新.
        if (TimeManager.Time <= JudgedTime)
        {
            float scale = 1 - (JudgedTime - TimeManager.Time) / JudgedRequired;

            Judged.transform.localScale = new Vector3(
                JudgedScale.x * scale,
                JudgedScale.y * scale,
                JudgedScale.z * scale);
        }
        else if (TimeManager.Time <= JudgedTime + JudgedDuration)
        {
            // Floating.
        }
        else
        {
            Judged.transform.localScale = Vector3.zero;
        }

        // 時間表示を更新.
        LabelTime.text = TimeManager.Time.ToString("N3");
    }

    /// <summary>
    /// 判定.
    /// </summary>
    /// <remarks>
    /// 現在の時間と比較して正確さを判定する.
    /// </remarks>
    Rating Judge(float arrival)
    {
        float time = TimeManager.Time;
        float since = arrival;
        float until = arrival;

        foreach (var rating in Ratings)
        {
            var accuracy = 0.0166f * rating.Accuracy;

            since -= accuracy;
            until += accuracy;

            if (since <= time && time <= until)
            {
                return rating;
            }
        }

        // 全て当てはまらなければ、末尾を返却.
        return Ratings[Ratings.Length - 1];
    }

    /// <summary>
    /// スコアを追加.
    /// </summary>
    /// <remarks>
    /// 値の加算とラベルの更新を行う.
    /// </remarks>
    void AddScore(int score)
    {
        Score += score;
        LabelScore.text = Score.ToString();
    }

    /// <summary>
    /// 判定結果を表示.
    /// </summary>
    /// <remarks>
    /// 判定されたレーティングを表示する.
    /// </remarks>
    void ShowJudged(Rating rating)
    {
        Judged.sprite = rating.Sprite;
        JudgedTime = TimeManager.Time + JudgedRequired;
    }

    /// <summary>
    /// サウンドエフェクトを再生.
    /// </summary>
    /// <remarks>
    /// サウンドエフェクトを再生する.
    /// </remarks>
    void PlaySoundEffect(AudioClip sound)
    {
        if (sound != null)
        {
            if (SoundEffects.ContainsKey(sound))
            {
                SoundEffects[sound].clip = sound;
                SoundEffects[sound].Play();
            }
            else
            {
                Debug.LogWarningFormat("Uncached {0} can not be played.", sound.name);
            }
        }
    }

    /// <summary>
    /// 一時停止コールバック.
    /// </summary>
    /// <remarks>
    /// タイマーの停止とメニューの表示を行う.
    /// </remarks>
    public void OnPause()
    {
        TimeManager.Running = false;
        Menu.SetActive(true);
    }

    /// <summary>
    /// レーンタッチコールバック.
    /// </summary>
    /// <remarks>
    /// 打鍵を判定する.
    /// </remarks>
    public void OnLaneTouch(BaseEventData data)
    {
        var lane = ((PointerEventData) data).pointerEnter;
        var sound = TouchSound;
        var holded = Ribbons.Find(a =>
        {
            return a.IsAlive() &&
            a.Lane == lane;
        });

        foreach (var icon in Icons)
        {
            if (icon.IsAlive() &&
                icon.Lane == lane &&
                (holded == null || icon.Note != holded.Tail))
            {
                var rating = Judge(icon.Note.Time);

                if (rating.Score > 0)
                {
                    icon.Note.Rating = rating;
                    AddScore(rating.Score);
                    ShowJudged(rating);

                    if (rating.HitSound != null)
                    {
                        sound = rating.HitSound;
                    }

                    break;
                }
            }
        }

        PlaySoundEffect(sound);
    }

    /// <summary>
    /// レーンリリースコールバック.
    /// </summary>
    /// <remarks>
    /// 打鍵を判定する.
    /// </remarks>
    public void OnLaneRelease(BaseEventData data)
    {
        var lane = ((PointerEventData) data).pointerPress;
        var holded = Ribbons.Find(a =>
        {
            return a.IsAlive() &&
            a.Head.Rating != null &&
            a.Lane == lane;
        });

        if (holded == null)
        {
            return;
        }

        var rating = Judge(holded.Tail.Time);
        holded.Tail.Rating = rating;
        AddScore(rating.Score);
        ShowJudged(rating);
        PlaySoundEffect(rating.HitSound);
    }

    /// <summary>
    /// 再開コールバック.
    /// </summary>
    /// <remarks>
    /// 一時停止を解除する.
    /// </remarks>
    public void OnRestart()
    {
        Menu.SetActive(false);
        TimeManager.Running = true;
    }

    /// <summary>
    /// 再試行コールバック.
    /// </summary>
    /// <remarks>
    /// 設定画面に戻る.
    /// </remarks>
    public void OnRetry()
    {
        SceneUtils.LoadSettings();
    }
}
