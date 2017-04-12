using UnityEngine;

/// <summary>
/// リズムアイコン.
/// </summary>
public class Icon : MonoBehaviour
{
    /// <summary>
    /// ノート.
    /// </summary>
    /// <remarks>
    /// 紐付けられた譜面情報.
    /// </remarks>
    public Note Note;

    /// <summary>
    /// 所要時間.
    /// </summary>
    /// <remarks>
    /// 生成から到着までの時間.
    /// </remarks>
    public float Required;

    /// <summary>
    /// レーン.
    /// </summary>
    /// <remarks>
    /// 到着する位置.
    /// </remarks>
    public GameObject Lane;

    /// <summary>
    /// Y 座標の開始地点.
    /// </summary>
    /// <remarks>
    /// 到着が 0 固定なので移動距離になる.
    /// </remarks>
    float StartY;

    /// <summary>
    /// 表示時のスケール.
    /// </summary>
    /// <remarks>
    /// 初期値を使う想定.
    /// </remarks>
    Vector3 VisibleScale;

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Start()
    {
        // Unused.
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        // Unused.
    }

    /// <summary>
    /// 開始.
    /// </summary>
    /// <remarks>
    /// マネージャの管理下にある時間を使うための開始処理.
    /// </remarks>
    public void Start(float time)
    {
        StartY = transform.position.y;
        VisibleScale = transform.localScale;
    }

    /// <summary>
    /// 更新.
    /// </summary>
    /// <remarks>
    /// マネージャの管理下にある時間を使うための更新処理.
    /// </remarks>
    public void Update(float time)
    {
        if (IsAlive())
        {
            transform.position = CurrentPosition(time);
            transform.localScale = VisibleScale;
        }
        else
        {
            transform.localScale = Vector3.zero;
        }
    }

    /// <summary>
    /// 生存なら true.
    /// </summary>
    /// <remarks>
    /// ノートが設定されており、判定されてなければ true.
    /// </remarks>
    public bool IsAlive()
    {
        return Note != null && Note.Rating == null;
    }

    /// <summary>
    /// 現在の位置.
    /// </summary>
    Vector3 CurrentPosition(float time)
    {
        var start = new Vector3(
                        Lane.transform.position.x,
                        StartY,
                        transform.position.z);

        return PlayUtils.CurrentPosition(
            start,
            Lane.transform.position,
            Required,
            Note.Time,
            time);
    }
}
