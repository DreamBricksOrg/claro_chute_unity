using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class Utils
{

    public static void DelayAction(float delayTime, Action callback)
    {
        // backward-compatible: fire-and-forget, returned CTS ignored
        DelayActionCancelable(delayTime, callback);
    }

    // New API: returns a CancellationTokenSource so callers can cancel the pending action
    public static CancellationTokenSource DelayActionCancelable(float delayTime, Action callback)
    {
        var cts = new CancellationTokenSource();
        // intentionally not awaited; we want fire-and-forget behavior
        _ = DelayActionAsync(delayTime, callback, cts.Token);
        return cts;
    }

    private static async Task DelayActionAsync(float delayTime, Action callback, CancellationToken token)
    {
        try
        {
            int delayMilliseconds = (int)(delayTime * 1000);
            await Task.Delay(delayMilliseconds, token);
            if (!token.IsCancellationRequested)
                callback?.Invoke();
        }
        catch (OperationCanceledException)
        {
            // canceled - nothing to do
        }
    }

    // Helper to cancel and dispose a CancellationTokenSource returned by DelayActionCancelable
    public static void CancelDelay(CancellationTokenSource cts)
    {
        if (cts == null) return;
        try
        {
            if (!cts.IsCancellationRequested) cts.Cancel();
        }
        finally
        {
            cts.Dispose();
        }
    }

    public static Color HexToColor(string hex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            return color;
        }
        else
        {
            //Debug.LogError("Falha ao converter a cor: " + hex);
            return Color.clear;
        }
    }

    public static string GetStreamingAssets(string relativePath)
    {
        relativePath = relativePath.Replace("\\", "/").TrimStart('/');

#if UNITY_ANDROID && !UNITY_EDITOR
        return $"jar:file://{Application.dataPath}!/assets/{relativePath}";
#else
        return $"{Application.streamingAssetsPath}/{relativePath}";
#endif
    }

    static public async Task<Texture2D> LoadTextureAsync(string url, CancellationToken token)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            var asyncOp = webRequest.SendWebRequest();

            while (!asyncOp.isDone)
            {
                token.ThrowIfCancellationRequested();
                await Task.Yield();
            }

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Erro ao carregar a imagem: {webRequest.error}");
                return null;
            }
            else
            {
                return DownloadHandlerTexture.GetContent(webRequest);
            }
        }
    }

    static public void LoadText(string url, Action<string> onComplete)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        var asyncOp = webRequest.SendWebRequest();

        asyncOp.completed += (op) =>
        {
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Erro ao carregar o arquivo de texto: {webRequest.error}");
                onComplete?.Invoke(null);
            }
            else
            {
                onComplete?.Invoke(webRequest.downloadHandler.text);
            }
            webRequest.Dispose();
        };
    }

}
