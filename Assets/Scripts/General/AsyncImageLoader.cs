using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class AsyncImageLoader : MonoBehaviour
{
	public static async Task<Texture2D> LoadTextureAsync(string url, CancellationToken token, IProgress<float> progress = null)
	{
		using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
		{
			var asyncOp = webRequest.SendWebRequest();

			while (!asyncOp.isDone)
			{
				token.ThrowIfCancellationRequested();
				progress?.Report(asyncOp.progress);
				await Task.Yield();  // Permite que outras tarefas continuem na thread principal
			}

			if (webRequest.result == UnityWebRequest.Result.Success)
			{
				var downloadedTexture = DownloadHandlerTexture.GetContent(webRequest);
				return await Task.FromResult(downloadedTexture);
			}
			else
			{
				// Enable mipmap
				//Texture2D mipmapTexture = new Texture2D(downloadedTexture.width, downloadedTexture.height, TextureFormat.RGBA32, true);
				//mipmapTexture.SetPixels(downloadedTexture.GetPixels());
				//mipmapTexture.Apply();

			}
			return null;
		}
	}


	//public static async Task<Texture2D> LoadTextureAsync(string url, CancellationToken token, IProgress<float> progress = null)
	//{
	//    using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
	//    {
	//        var asyncOp = webRequest.SendWebRequest();

	//        while (!asyncOp.isDone)
	//        {
	//            token.ThrowIfCancellationRequested();
	//            progress?.Report(asyncOp.progress);
	//            await Task.Yield();
	//        }

	//        if (webRequest.result != UnityWebRequest.Result.Success)
	//        {
	//            Debug.LogError($"Erro ao carregar a imagem: {webRequest.error}");
	//            return null;
	//        }
	//        else
	//        {
	//            return DownloadHandlerTexture.GetContent(webRequest);
	//        }
	//    }
	//}

	//public async Task StartLoadingImage(string url)
	//{
	//    var cts = new CancellationTokenSource();

	//    try
	//    {
	//        var texture = await LoadTextureAsync(url, cts.Token, new Progress<float>(p => Debug.Log($"Progresso: {p * 100}%")));
	//        if (texture != null)
	//        {
	//            Debug.Log("Imagem carregada com sucesso!");
	//            // Fa�a algo com a textura (por exemplo, aplicar em um objeto)
	//            // GetComponent<Renderer>().material.mainTexture = texture;
	//        }
	//    }
	//    catch (OperationCanceledException)
	//    {
	//        Debug.Log("Opera��o cancelada!");
	//    }
	//}

	public void CancelLoading(CancellationTokenSource cts)
	{
		if (cts != null)
		{
			cts.Cancel();
		}
	}
}
