using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// リソースマネージャ.
/// </summary>
public class ResourceManager : MonoBehaviour
{
    /// <summary>
    /// 譜面リスト名.
    /// </summary>
    const string MusicScores = "MusicScores";

    /// <summary>
    /// グローバル設定.
    /// </summary>
    public GlobalSettings Globals;

    /// <summary>
    /// 譜面をダウンロード.
    /// </summary>
    public void Download(Action<float> progressed)
    {
        // 譜面リスト.
        var list = MusicScores + ".txt";

        // ダウンロード.
        StartCoroutine(
            Get(
                Globals.RecommendedMusicScores,
                timedOut: () => Load(list, progressed),
                unexpired: process =>
                {
                    Load(list, progressed);
                    process.Finished = true;
                },
                downloaded: request =>
                {
                    if (request.responseCode == 200)
                    {
                        var type = request.GetResponseHeader("Content-Type");

                        if (type != null && type.StartsWith("text/"))
                        {
                            // リストの書き込み.
                            var path = Path.Combine(Application.persistentDataPath, list);

                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                using (var writer = new BinaryWriter(stream))
                                {
                                    writer.Write(request.downloadHandler.data);
                                }
                            }

                            Download(request.downloadHandler.text.Split('\n'), progressed);
                        }
                    }
                    else if (request.responseCode == 304)
                    {
                        Load(list, progressed);
                    }
                }
            )
        );
    }

    /// <summary>
    /// 譜面リストのロードと譜面をダウンロード.
    /// </summary>
    void Load(string list, Action<float> progress)
    {
        var path = Path.Combine(Application.persistentDataPath, list);

        if (File.Exists(path))
        {
            using (var reader = new StreamReader(path))
            {
                Download(reader.ReadToEnd().Split('\n'), progress);
            }
        }
        else
        {
            progress(1);
        }
    }

    /// <summary>
    /// 譜面をダウンロード.
    /// </summary>
    void Download(string[] urls, Action<float> progressed)
    {
        var valid = new List<string>();

        foreach (var url in urls)
        {
            if (Valid(url))
            {
                valid.Add(url);
            }
        }

        if (valid.Count == 0)
        {
            progressed(1);
        }
        else
        {
            var progresses = new Dictionary<string, float>();

            foreach (var url in valid)
            {
                progresses.Add(url, 0);

                StartCoroutine(
                    Get(
                        url,
                        progressed: (progress) =>
                        {
                            progresses[url] = progress;

                            var sum = 0f;

                            foreach (var p in progresses.Values)
                            {
                                sum += p;
                            }

                            progressed(sum / valid.Count);
                        },
                        unexpired: process =>
                        {
                            process.Finished = true;
                        },
                        downloaded: request =>
                        {
                            if (request.responseCode == 200)
                            {
                                var type = request.GetResponseHeader("Content-Type");

                                if (type != null && type.StartsWith("text/"))
                                {

                                    var file = Path.ChangeExtension(Path.GetFileName(url), "txt");
                                    var dir = Path.Combine(Application.persistentDataPath, MusicScores);
                                    var path = Path.Combine(dir, file);

                                    if (!Directory.Exists(dir))
                                    {
                                        Directory.CreateDirectory(dir);
                                    }

                                    using (var stream = new FileStream(path, FileMode.Create))
                                    {
                                        using (var writer = new BinaryWriter(stream))
                                        {
                                            writer.Write(request.downloadHandler.data);
                                        }
                                    }
                                }
                            }
                        }
                    )
                );
            }
        }
    }

    /// <summary>
    /// URI を検証.
    /// </summary>
    bool Valid(string uri)
    {
        Uri created;

        if (Uri.TryCreate(uri, UriKind.Absolute, out created))
        {
            return created.Scheme == Uri.UriSchemeHttp || created.Scheme == Uri.UriSchemeHttps;
        }

        return false;
    }

    /// <summary>
    /// HTTP GET.
    /// </summary>
    public IEnumerator Get(
        string url,
        float timeout = 3,
        Action<float> progressed = null,
        Action timedOut = null,
        Action<DownloadProcess> expired = null,
        Action<DownloadProcess> unexpired = null,
        Action<UnityWebRequest> downloaded = null)
    {
        Debug.LogFormat("Get {0}.", url);

        var bytes = Encoding.Default.GetBytes(url);
        var hash = MD5.Create().ComputeHash(bytes);
        var hex = new StringBuilder(hash.Length * 2);

        for (var i = 0; i < hash.Length; i++)
        {
            hex.Append(hash[i].ToString("x2"));
        }

        var cache = Path.Combine(Application.temporaryCachePath, hex.ToString());
        var cached = new Dictionary<string, string>();

        if (File.Exists(cache) && (expired != null || unexpired != null))
        {
            // キャッシュの読み込み.
            using (var reader = new StreamReader(cache))
            {
                foreach (var line in reader.ReadToEnd ().Split ('\n'))
                {
                    var header = line.Split(new [] { ": " }, StringSplitOptions.None);

                    if (header.Length == 2)
                    {
                        if (cached.ContainsKey(header[0]))
                        {
                            cached[header[0]] = header[1];
                        }
                        else
                        {
                            cached.Add(header[0], header[1]);
                        }
                    }
                }
            }

            var process = new DownloadProcess();

            if (cached.ContainsKey("Expires"))
            {
                try
                {
                    var expires = DateTime.ParseExact(cached["Expires"],
                                      CultureInfo.CurrentCulture.DateTimeFormat.RFC1123Pattern,
                                      CultureInfo.InvariantCulture);

                    if (expires > DateTime.Now)
                    {
                        Debug.LogFormat("{0} is unexpired.", url);

                        if (unexpired != null)
                        {
                            unexpired(process);
                        }
                    }
                    else
                    {
                        Debug.LogFormat("{0} is expired.", url);

                        if (expired != null)
                        {
                            expired(process);
                        }
                    }
                }
                catch
                {
                }
            }
            else if (cached.ContainsKey("Cache-Control") && cached.ContainsKey("Date"))
            {
                var match = Regex.Match(cached["Cache-Control"], @"max-age=(\d+)");

                if (match.Success)
                {
                    try
                    {
                        var date = DateTime.ParseExact(cached["Date"],
                                       CultureInfo.CurrentCulture.DateTimeFormat.RFC1123Pattern,
                                       CultureInfo.InvariantCulture);

                        var expires = date.AddSeconds(int.Parse(match.Groups[1].Value));

                        if (expires > DateTime.Now)
                        {
                            Debug.LogFormat("{0} is unexpired.", url);

                            if (unexpired != null)
                            {
                                unexpired(process);
                            }
                        }
                        else
                        {
                            Debug.LogFormat("{0} is expired.", url);

                            if (expired != null)
                            {
                                expired(process);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            if (process.Finished)
            {
                if (progressed != null)
                {
                    progressed(1);
                }

                yield break;
            }
        }

        var request = UnityWebRequest.Get(url);

        if (cached.ContainsKey("Last-Modified"))
        {
            request.SetRequestHeader("If-Modified-Since", cached["Last-Modified"]);
        }

        if (cached.ContainsKey("ETag"))
        {
            request.SetRequestHeader("If-None-Match", cached["ETag"]);
        }

        var operation = request.Send();

        var since = Time.time;

        while (!operation.isDone)
        {
            yield return null;

            if (progressed != null)
            {
                progressed(request.downloadProgress);
            }

            if (timeout <= Time.time - since)
            {
                Debug.LogErrorFormat("Request to {0} timed out. Timeout is {1} sec.", url, timeout);
                timedOut();
                yield break;
            }
        }

        if (request.isError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.LogFormat("Response code from {0} is {1}.", url, request.responseCode);

            if (200 <= request.responseCode && request.responseCode < 300)
            {
                // キャッシュの書き込み.
                using (var stream = new FileStream(cache, FileMode.Create))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        foreach (var header in request.GetResponseHeaders ())
                        {
                            writer.WriteLine(header.Key + ": " + header.Value);
                        }
                    }
                }
            }

            if (downloaded != null)
            {
                downloaded(request);
            }
        }

        if (progressed != null)
        {
            progressed(1);
        }
    }

    /// <summary>
    /// 全ての譜面をロード.
    /// </summary>
    public List<MusicScore> LoadMusicScores()
    {
        List<MusicScore> result = new List<MusicScore>();

        var cached = new Dictionary<string, MusicScore>();

        var persistent = Path.Combine(Application.persistentDataPath, MusicScores);

        if (Directory.Exists(persistent))
        {
            foreach (var file in Directory.GetFiles (persistent))
            {
                var id = Path.GetFileNameWithoutExtension(file);

                using (var reader = new StreamReader(file))
                {
                    cached.Add(id, new MusicScore(id, reader.ReadToEnd()));
                }
            }
        }

        foreach (var res in Resources.LoadAll<TextAsset> (MusicScores))
        {
            if (!cached.ContainsKey(res.name))
            {
                result.Add(new MusicScore(res.name, res.text));
            }
        }

        result.AddRange(cached.Values);

        result.Sort((a, b) => b.Priority - a.Priority);

        return result;
    }

    /// <summary>
    /// 譜面をロード.
    /// </summary>
    public static MusicScore LoadMusicScore(string id)
    {
        MusicScore result = null;

        var name = Path.Combine(MusicScores, id.ToString());

        var persistent = Path.Combine(Application.persistentDataPath, name + ".txt");

        if (File.Exists(persistent))
        {
            using (var reader = new StreamReader(persistent))
            {
                result = new MusicScore(id, reader.ReadToEnd());
            }
        }
        else
        {
            var res = Resources.Load(name) as TextAsset;

            if (res != null)
            {
                result = new MusicScore(id, res.text);
            }
        }

        return result;
    }
}
