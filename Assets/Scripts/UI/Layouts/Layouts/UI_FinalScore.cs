using System;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Linq;
using SimpleJSON;

public class UI_FinalScore : UI
{
    public TMP_Text fieldScore;
    public TMP_Text fieldRanking;

    internal override void OnShow(object data, Action<object> callback)
    {
        base.OnShow(data, callback);
        var maxShootSpeed = 0f;
        try
        {
            maxShootSpeed = GameRoundController.Instance.gameScoreList.Max(x => x.speed);
        }
        catch (Exception)
        {
        }

        fieldRanking.SetText("");
        fieldScore.SetText("");

        var jsonData = new JSONObject();
        jsonData["score"] = maxShootSpeed;

        API.Request("/api/getrankingposition",
            onSuccess: (result) =>
            {
                var jsonData = JSON.Parse(result);
                GameRoundController.Instance.playerId = jsonData["player"]["id"].Value;

                var score = jsonData["player"]["score"].AsFloat;
                var position = jsonData["player"]["position"].AsInt;

                SetPlayerRankPosition(position);
                ShowShootSpeed(score);

                
                //                 {
                //                     "status": "success",
                //   "player": {
                //                         "id": "1f6503ac-c452-404e-9c44-f7286132f21b",
                //     "score": 86,
                //     "position": 7,
                //     "created_at": "2025-10-23T21:09:07.743857"
                //   },
                //   "message": "Player saved successfully",
                //   "video": {
                //                         "success": false,
                //     "message": "Erro interno: [WinError 32] O arquivo já está sendo usado por outro processo: 'C:\\\\Users\\\\db\\\\Documents\\\\db\\\\prj\\\\claro_tvbox\\\\claro_tvbox_server\\\\app\\\\recordings\\\\2025-10-23 18-08-08.mp4' -> 'C:\\\\Users\\\\db\\\\Documents\\\\db\\\\prj\\\\claro_tvbox\\\\claro_tvbox_server\\\\app\\\\recordings\\\\1f6503ac-c452-404e-9c44-f7286132f21b_claro_tvbox.mp4'"
                //   }
                //                 }

                // "video": {
                //         "success": true,
                //         "message": "Vídeo encontrado e ID atribuído com sucesso",
                //         "filename": "62aaad46-b0c2-45fc-b359-8615d4297dcc_claro_tvbox.mp4",
                //         "renamed": true
                //     }

            },
            onError: (error) =>
            {
                Debug.LogError("API Error: " + error);
            },
            API.APIMethod.POST,
            jsonData.ToString()
        );

        // API.Request("/api/playerdata/" + GameRoundController.Instance.playerId, 
        //     onSuccess: (result) => {
        //         var jsonData = JSON.Parse(result);
        //         SetPlayerRankPosition(jsonData["player"]["position"].Value);
        //     },
        //     onError: (error) => {
        //         Debug.LogError("API Error: " + error);
        //     }
        // );
    }

    private void ShowShootSpeed(float speed)
    {
        DOTween.To(() => 0f, x => ApplySpeedText(x), speed, 0.8f).SetEase(Ease.Linear);
    }

    void ApplySpeedText(float speed)
    {
        fieldScore.SetText(speed.ToString("0") + "<size=40%>km/h</size>");
    }

    void SetPlayerRankPosition(int position)
    {
        fieldRanking.SetText(position + "º");
    }


}
