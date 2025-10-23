using System;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class UI_QRCode : UI
{
	public RawImage qrCodeImage;

	internal override void Awake()
	{
		base.Awake();
	}

    internal override void OnShow(object data, Action<object> callback)
    {
		base.OnShow(data, callback);
		var jsonData = new JSONObject();
		jsonData["player_id"] = GameRoundController.Instance.playerId;

        API.Request("/api/getqrcode", 
            onSuccess: (result) => {
				var jsonData = JSON.Parse(result);
                var qrTexture = Base64ToTexture(jsonData["qr_code"].Value);
				qrCodeImage.texture = qrTexture;
                // LayoutRebuilder.ForceRebuildLayoutImmediate(qrCodeImage.transform.parent as RectTransform);
                // LayoutRebuilder.MarkLayoutForRebuild(qrCodeImage.transform.parent as RectTransform);
            },
            onError: (error) => {
                Debug.LogError("API Error: " + error);
            }, API.APIMethod.POST, jsonData.ToString()
        );
    }

    private Texture2D Base64ToTexture(string base64String)
    {
        try
        {
            var base64Data = base64String.Contains(",") ? 
                base64String.Substring(base64String.IndexOf(",") + 1) : 
                base64String;

            byte[] imageData = Convert.FromBase64String(base64Data);
			var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
			texture.LoadImage(imageData);
			return texture;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error converting base64 to texture: {e.Message}");
            return null;
        }
    }

}
