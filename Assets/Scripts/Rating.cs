using UnityEngine;

/// <summary>
/// 打鍵の正確さを表す評価.
/// </summary>
[CreateAssetMenu]
public class Rating : ScriptableObject
{
    /// <summary>
    /// スコア.
    /// </summary>
    public int Score;

    /// <summary>
    /// 判定幅を表すフレーム数.
    /// </summary>
    public int Accuracy;

    /// <summary>
    /// 画像.
    /// </summary>
    public Sprite Sprite;

    /// <summary>
    /// サウンド.
    /// </summary>
    public AudioClip HitSound;
}
