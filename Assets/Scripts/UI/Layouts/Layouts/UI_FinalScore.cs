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

        ShowShootSpeed(maxShootSpeed);
        fieldRanking.SetText("");

        API.Request("/api/playerdata/" + GameRoundController.Instance.playerId, 
            onSuccess: (result) => {
                var jsonData = JSON.Parse(result);
                SetPlayerRankPosition(jsonData["player"]["position"].Value);
            },
            onError: (error) => {
                Debug.LogError("API Error: " + error);
            }
        );
    }

    private void ShowShootSpeed(float speed)
    {
        DOTween.To(() => 0f, x => ApplySpeedText(x), speed, 0.8f).SetEase(Ease.Linear);
    }

    void ApplySpeedText(float speed)
    {
        fieldScore.SetText(speed.ToString("0") + "<size=40%>km/h</size>");
    }

    void SetPlayerRankPosition(string position)
    {
        fieldRanking.SetText(position + "º");
    }


}
