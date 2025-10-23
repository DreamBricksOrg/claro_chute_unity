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
        else if (instance != this)
            Destroy(gameObject);
    }

    public enum APIMethod
    {
        GET,
        POST
    }

    public static void Request(
        string endpoint,
        Action<string> onSuccess,
        Action<string> onError = null,
        APIMethod method = APIMethod.GET,
        string jsonData = null)
    {
        if (instance == null)
        {
            Debug.LogError("API instance is missing in the scene.");
            return;
        }

        string url = Config.Instance.configData["api"]["baseUrl"].Value + endpoint;

        switch (method)
        {
            case APIMethod.GET:
                instance.StartCoroutine(Get(url, onSuccess, onError));
                break;

            case APIMethod.POST:
                instance.StartCoroutine(Post(url, jsonData, onSuccess, onError));
                break;

            default:
                onError?.Invoke("Invalid API method");
                break;
        }
    }

    private static IEnumerator Get(string url, Action<string> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Accept", "application/json");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(www.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"[API GET] Error: {www.error}");
                onError?.Invoke(www.error);
            }

            www.downloadHandler?.Dispose();
            www.uploadHandler?.Dispose();
        }
    }

    private static IEnumerator Post(string url, string jsonData, Action<string> onSuccess, Action<string> onError)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData ?? "{}");
        using (UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();

            www.SetRequestHeader("Accept", "application/json");
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(www.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"[API POST] Error: {www.error}\nURL: {url}\nBody: {jsonData}");
                onError?.Invoke(www.error);
            }

            www.downloadHandler?.Dispose();
            www.uploadHandler?.Dispose();
        }
    }
}
