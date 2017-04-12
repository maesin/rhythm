using UnityEngine;

/// <summary>
/// リボン.
/// </summary>
public class Ribbon : MonoBehaviour
{
    /// <summary>
    /// 先頭ノート.
    /// </summary>
    /// <remarks>
    /// 紐付けられた譜面情報.
    /// </remarks>
    public Note Head;

    /// <summary>
    /// 末尾ノート.
    /// </summary>
    /// <remarks>
    /// 紐付けられた譜面情報.
    /// </remarks>
    public Note Tail;

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
    /// ライン.
    /// </summary>
    /// <remarks>
    /// Head と Tail を繋ぐライン.
    /// </remarks>
    LineRenderer Line;

    /// <summary>
    /// Y 座標の開始地点.
    /// </summary>
    /// <remarks>
    /// 到着が 0 固定なので移動距離になる.
    /// </remarks>
    float StartY;

    // Use this for initialization
    void Start()
    {
        // Unused.
    }
	
    // Update is called once per frame
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
        Line = GetComponent<LineRenderer>();
        StartY = Line.GetPosition(0).y;
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
            if (Head.Rating == null)
            {
                Line.SetPosition(1, CurrentPosition(Head, time));
            }
            else if (Head.Rating.Score > 0)
            {
                Line.SetPosition(1, Lane.transform.position);
            }

            if (Tail == null)
            {
                Line.SetPosition(0, new Vector3(Lane.transform.position.x, StartY, transform.position.z));
            }
            else if (Tail.Rating == null)
            {
                Line.SetPosition(0, CurrentPosition(Tail, time));
            }
        }
        else
        {
            Line.SetPosition(0, Vector3.zero);
            Line.SetPosition(1, Vector3.zero);
        }
    }

    /// <summary>
    /// 生存なら true.
    /// </summary>
    /// <remarks>
    /// 先頭ノートが設定されており、ミス以外の判定かつ、末尾ノートが判定されてなければ true.
    /// </remarks>
    public bool IsAlive()
    {
        return Head != null && (Head.Rating == null || Head.Rating.Score > 0) && Tail.Rating == null;
    }

    /// <summary>
    /// 現在の位置.
    /// </summary>
    Vector3 CurrentPosition(Note note, float time)
    {
        var start = new Vector3(
                        Lane.transform.position.x,
                        StartY,
                        transform.position.z);

        return PlayUtils.CurrentPosition(
            start,
            Lane.transform.position,
            Required,
            note.Time,
            time);
    }
}
