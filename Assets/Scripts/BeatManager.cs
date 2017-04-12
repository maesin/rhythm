using UnityEngine;

/// <summary>
/// ビートマネージャ.
/// </summary>
public class BeatManager : MonoBehaviour
{
    /// <summary>
    /// サウンド.
    /// </summary>
    public AudioSource TickSound;

    /// <summary>
    /// 拍数.
    /// </summary>
    public int Count;

    /// <summary>
    /// タイムマネージャ.
    /// </summary>
    public TimeManager TimeManager;

    /// <summary>
    /// Beats Per Second.
    /// </summary>
    float BPS;

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Start()
    {
        BPS = 60f / Settings.BPM;
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        if ((int) (TimeManager.Time / BPS) >= Count)
        {
            TickSound.Play();
            Count++;
        }
    }
}
