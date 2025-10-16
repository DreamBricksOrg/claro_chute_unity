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
        DelayActionAsync(delayTime, callback);
    }

    private static async void DelayActionAsync(float delayTime, Action callback)
    {
        int delayMilliseconds = (int)(delayTime * 1000);
        await Task.Delay(delayMilliseconds);
        callback?.Invoke();
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
