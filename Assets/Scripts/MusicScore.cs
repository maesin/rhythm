using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// 譜面.
/// </summary>
public class MusicScore
{
    /// <summary>
    /// 小節あたりの拍数.
    /// </summary>
    const int BeatPerMeasure = 4;

    /// <summary>
    /// ID.
    /// </summary>
    public string Id;

    /// <summary>
    /// タイトル.
    /// </summary>
    public string Title;

    /// <summary>
    /// 優先度.
    /// </summary>
    public int Priority;

    /// <summary>
    /// データ.
    /// </summary>
    string Data;

    /// <summary>
    /// ノート.
    /// </summary>
    List<Note> notes;

    /// <summary>
    /// インスタンスを生成.
    /// </summary>
    /// <remarks>
    /// データから必要最低限の情報をロードしてインスタンスを生成する.
    /// </remarks>
    public MusicScore(string id, string data)
    {
        Id = id;

        foreach (var line in data.Split ('\n'))
        {
            var trimed = line.Trim();

            Match match = Regex.Match(trimed, @"^#Title (.+)$");

            if (match.Success)
            {
                Title = match.Groups[1].Value;
            }

            match = Regex.Match(trimed, @"^#Priority (\d+)$");

            if (match.Success)
            {
                Priority = int.Parse(match.Groups[1].Value);
            }
        }

        Data = data;
    }

    /// <summary>
    /// ノート.
    /// </summary>
    /// <remarks>
    /// 初回呼び出し時にデータをロードする.
    /// </remarks>
    public List<Note> Notes
    {
        get
        {
            if (notes == null || notes.Count <= 0)
            {
                notes = new List<Note>();

                foreach (string line in Data.Split ('\n'))
                {
                    var trimed = line.Trim();

                    Match match = Regex.Match(trimed, @"^#(\d+):(\d+):(\d+)$");

                    if (match.Success)
                    {
                        var num = int.Parse(match.Groups[1].Value);
                        var beats = match.Groups[2].Value.ToCharArray();
                        var lanes = match.Groups[3].Value.ToCharArray();
                        var bps = 60f / Settings.BPM;
                        var time = num * bps * BeatPerMeasure;

                        for (var i = 0; i < beats.Length; i++)
                        {
                            var beat = beats[i] - '0';

                            if (beat > 0)
                            {
                                var note = new Note();
                                note.Time = time;
                                note.Type = (NoteType) beat;
                                note.Lane = lanes[i] - '0';
                                notes.Add(note);
                            }

                            time += bps * (float) BeatPerMeasure / beats.Length;
                        }
                    }
                }

                notes.Sort((a, b) =>
                {
                    var diff = a.Time - b.Time;
                    return diff > 0 ? 1 : diff < 0 ? -1 : 0;
                });
            }

            return notes;
        }
    }
}
