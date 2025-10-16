using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System.IO;

public class Config : MonoBehaviour
{

    public static Config Instance;
    public JSONNode configData;

    public static string GetStreamingAssets(string relativePath)
    {
        relativePath = relativePath.Replace("\\", "/").TrimStart('/');
        return $"{Application.streamingAssetsPath}/{relativePath}";
    }

    void Awake()
    {
        Instance = this;
        LoadConfig();
    }

    private void LoadConfig()
    {
        var configPath = Path.Combine(Application.streamingAssetsPath, "config.json");
        Utils.LoadText(configPath, (json) =>
        {
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    Instance.configData = JSON.Parse(json);
                    Debug.Log("Config loaded successfully." + Instance.configData.ToString());
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error parsing config JSON: {ex.Message}");
                }
            }
            else
            {
                Debug.LogError("Config file is empty or could not be loaded.");
            }
        });
    }

    // static public async Task<Texture2D> LoadTextureAsync(string url, CancellationToken token)
    // {
    //     using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
    //     {
    //         var asyncOp = webRequest.SendWebRequest();

    //         while (!asyncOp.isDone)
    //         {
    //             token.ThrowIfCancellationRequested();
    //             await Task.Yield();
    //         }

    //         if (webRequest.result != UnityWebRequest.Result.Success)
    //         {
    //             Debug.LogError($"Erro ao carregar a imagem: {webRequest.error}");
    //             return null;
    //         }
    //         else
    //         {
    //             return DownloadHandlerTexture.GetContent(webRequest);
    //         }
    //     }
    // }


}
