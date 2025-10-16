using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;
using System.IO;

public class API : MonoBehaviour
{
    public enum APIMethod
    {
        GET,
        POST
    }

    public async Task<string> Request(string endpoint, APIMethod method = APIMethod.GET, string jsonData = null)
    {
        var url = Path.Combine(Config.Instance.configData["api"]["baseUrl"].Value, endpoint);

        switch (method)
        {
            case APIMethod.GET:
                return await Get(url);
            case APIMethod.POST:
                return await Post(url, jsonData);
            default:
                throw new ArgumentException("Invalid API method");
        }
    }

    private async Task<string> Get(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (www.result == UnityWebRequest.Result.Success)
            {
                return www.downloadHandler.text;
            }
            else
            {
                Debug.LogError($"Error: {www.error}");
                throw new Exception($"GET request failed: {www.error}");
            }
        }
    }

    private async Task<string> Post(string url, string jsonData)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(url, jsonData))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            var operation = www.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (www.result == UnityWebRequest.Result.Success)
            {
                return www.downloadHandler.text;
            }
            else
            {
                Debug.LogError($"Error: {www.error}");
                throw new Exception($"POST request failed: {www.error}");
            }
        }
    }
}
