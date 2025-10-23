using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class API : MonoBehaviour
{
    private static API instance;
    
    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public enum APIMethod
    {
        GET,
        POST
    }

    public static void Request(string endpoint, Action<string> onSuccess, Action<string> onError = null, APIMethod method = APIMethod.GET, string jsonData = null)
    {
        var url = Config.Instance.configData["api"]["baseUrl"].Value + endpoint;
        switch (method)
        {
            case APIMethod.GET:
                instance.StartCoroutine(Get(url, onSuccess, onError));
                break;
            case APIMethod.POST:
                instance.StartCoroutine(Post(url, jsonData, onSuccess, onError));
                break;
            default:
                if (onError != null)
                    onError("Invalid API method");
                break;
        }
    }

    private static IEnumerator Get(string url, System.Action<string> onSuccess, System.Action<string> onError)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("accept", "application/json");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(www.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"Error: {www.error}");
                onError?.Invoke(www.error);
            }
        }
    }

    private static IEnumerator Post(string url, string jsonData, System.Action<string> onSuccess, System.Action<string> onError)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(url, jsonData))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("accept", "application/json");
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(www.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"Error: {www.error}");
                onError?.Invoke(www.error);
            }
        }
    }
}
