using System;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class UI_QRCode : UI
{
    [Header("QR Code UI")]
    public RawImage qrCodeImage;

    private Texture2D currentQRTexture;
    private bool isDestroyed = false;

    internal override void OnShow(object data, Action<object> callback)
    {
        base.OnShow(data, callback);
        isDestroyed = false;

        var jsonData = new JSONObject();
        jsonData["player_id"] = GameRoundController.Instance.playerId;

        DisposeCurrentTexture();

        API.Request("/api/getqrcode",
            onSuccess: (result) =>
            {
                if (isDestroyed) return;
                try
                {
                    var response = JSON.Parse(result);
                    string base64 = response["qr_code"].Value;

                    var newTexture = Base64ToTexture(base64);
                    if (newTexture != null)
                    {
                        DisposeCurrentTexture();
                        currentQRTexture = newTexture;
                        qrCodeImage.texture = currentQRTexture;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"QR Parse Error: {e}");
                }
            },
            onError: (error) =>
            {
                if (!isDestroyed)
                    Debug.LogError("API Error: " + error);
            },
            API.APIMethod.POST,
            jsonData.ToString()
        );
    }

    internal override void OnHide(object data, Action<object> callback)
    {
        base.OnHide(data, callback);
        DisposeCurrentTexture();
    }

    internal override void OnDestroy()
    {
        isDestroyed = true;
        DisposeCurrentTexture();
        base.OnDestroy();
    }

    private void DisposeCurrentTexture()
    {
        if (currentQRTexture != null)
        {
            Destroy(currentQRTexture);
            currentQRTexture = null;
            if (qrCodeImage != null)
                qrCodeImage.texture = null;
        }
    }

    private Texture2D Base64ToTexture(string base64String)
    {
        try
        {
            var base64Data = base64String.Contains(",")
                ? base64String.Substring(base64String.IndexOf(",") + 1)
                : base64String;

            byte[] imageData = Convert.FromBase64String(base64Data);

            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (texture.LoadImage(imageData))
                return texture;

            Destroy(texture);
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error converting base64 to texture: {e.Message}");
            return null;
        }
    }
}
