using UnityEngine;

/// <summary>
/// プレイユーティリティ.
/// </summary>
public class PlayUtils
{
    /// <summary>
    /// 現在の位置.
    /// </summary>
    public static Vector3 CurrentPosition(
        Vector3 start,
        Vector3 end,
        float required,
        float arrival,
        float time)
    {
        float x = end.x;
        float y = (arrival - time) / required * start.y + end.y;
        float z = end.z;
        return new Vector3(x, y, z);
    }
}
