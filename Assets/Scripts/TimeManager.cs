using UnityEngine;

/// <summary>
/// タイムマネージャ.
/// </summary>
public class TimeManager : MonoBehaviour
{
    /// <summary>
    /// 秒で表される時間.
    /// </summary>
    public float Time;

    /// <summary>
    /// 計測中なら true.
    /// </summary>
    public bool Running;

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Start()
    {
        Running = true;
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        if (Running)
        {
            Time += UnityEngine.Time.deltaTime;
        }
    }
}
