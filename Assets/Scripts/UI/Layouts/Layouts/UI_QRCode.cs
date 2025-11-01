using System;
using System.Collections;
using DG.Tweening;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class UI_QRCode : UI
{
    [Header("QR Code UI")]
    public RawImage qrCodeImage;
    private Texture2D currentQRTexture;
    private bool videoIsProcessed = false;
    // public int currentTimeout = 0;
    public CanvasGroup qrcodeCanvasGroup;
    // private string currentUrl = "";

    internal override void Start()
    {
        base.Start();
        EventManager.VideoReplay.OnVideoReplayProcessEvent += OnVideoReplayProcess;
    }

    internal override void OnDestroy()
    {
        base.OnDestroy();
        EventManager.VideoReplay.OnVideoReplayProcessEvent -= OnVideoReplayProcess;
        DisposeCurrentTexture();
    }

    internal override void OnShow(object data, Action<object> callback)
    {
        base.OnShow(data, callback);

        qrcodeCanvasGroup.alpha = 0;

        var jsonData = new JSONObject();
        jsonData["player_id"] = GameRoundController.Instance.playerId;

        DisposeCurrentTexture();

        // videoIsProcessed = false;
        // currentUrl = "";

        // currentTimeout = Config.Instance.configData["timeout"]["qrcode"].AsInt;;
        // InvokeRepeating(nameof(CheckVideoProcessed), 1f, 1f);

        API.Request("/api/getqrcode",
            onSuccess: (result) =>
            {
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
                    
                    qrcodeCanvasGroup.DOFade(1, 0.3f);

                    // Process Video
                    EventManager.VideoReplay.VideoReplayProcess();
                }
                catch (Exception e)
                {
                    CancelInvoke();
				    EventManager.Section.SetSection(SectionTypes.Gameover);
                    Debug.LogError($"QR Parse Error: {e}");
                }
            },
            onError: (error) =>
            {
                CancelInvoke();
				EventManager.Section.SetSection(SectionTypes.Gameover);
                Debug.LogError("API Error: " + error);
            },
            API.APIMethod.POST,
            jsonData.ToString()
        );
    }

    internal override void OnHide(object data, Action<object> callback)
    {
        base.OnHide(data, callback);
        // currentUrl = "";
        CancelInvoke();
        qrcodeCanvasGroup.DOFade(0, 0.3f).OnComplete(() =>
        {
            DisposeCurrentTexture();
        });
    }

    private void OnVideoReplayProcess()
    {
        // currentUrl = "";
        API.Request("/api/process-video/" + GameRoundController.Instance.playerId,
            onSuccess: (result) =>
            {
                var jsonData = JSON.Parse(result);
                Debug.Log("<<Video Generation Response>>" + jsonData.ToString());
                // currentUrl = jsonData["video"]["download_url"].Value;
                EventManager.VideoReplay.VideoReplayCompleted(jsonData["video"]["download_url"].Value);

                // {
                //     "status": "success",
                //     "player": {
                //         "id": "180c5dbf-5386-49ae-97c0-9306a96716ea",
                //         "score": 201,
                //         "position": 7,
                //         "created_at": "2025-10-23T21:36:08.592000"
                //     },
                //     "video": {
                //         "original_filename": "180c5dbf-5386-49ae-97c0-9306a96716ea_claro_tvbox.mp4",
                //         "processed_filename": "180c5dbf-5386-49ae-97c0-9306a96716ea_claro_tvbox_processed.mp4",
                //         "path": "/api/video/180c5dbf-5386-49ae-97c0-9306a96716ea",
                //         "download_url": "https://clarotvboxchute.ngrok.app/api/video/180c5dbf-5386-49ae-97c0-9306a96716ea",
                //         "processed": true,
                //         "processing_time": 3.404806137084961,
                //         "output_file": "C:\\Users\\db\\Documents\\db\\prj\\claro_tvbox\\claro_tvbox_server\\app\\recordings\\180c5dbf-5386-49ae-97c0-9306a96716ea_claro_tvbox_processed.mp4"
                //     },
                //     "message": "Vídeo processado com sucesso com labels aplicados"
                // }

            },
            onError: (error) =>
            {
                EventManager.Section.SetSection(SectionTypes.Gameover);
                Debug.LogError("API Error: " + error);
            }
        );
    }
    
    // void CheckVideoProcessed()
    // {
    //     Debug.Log("QR Timeout: " + currentTimeout);
    //     currentTimeout--;
    //     if (currentTimeout <= 0 && currentUrl != "")
    //     {
    //         EventManager.VideoReplay.VideoReplayCompleted(currentUrl);
    //         CancelInvoke();
    //     }
    // }

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
