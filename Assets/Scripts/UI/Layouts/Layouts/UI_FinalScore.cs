using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Linq;

public class UI_FinalScore : UI
{
    public TMP_Text fieldScore;

    internal override void Awake()
    {
        base.Awake();
    }

    internal override void OnShow(object data, Action<object> callback)
    {
        base.OnShow(data, callback);
        try
        {
            var maxShootSpeed = GameRoundController.Instance.gameScoreList.Max(x => x.speed);
            ShowShootSpeed(maxShootSpeed);
        }
        catch (Exception)
        {
        }
        // EventManager.Log.SetLog(LogTypeInfo.INFO, "Game Score: " + GameRoundController.Instance.CurrentGameScore.ToString());
        // EventManager.Log.SetLog(LogTypeInfo.GAME_END);
    }
    
    private void ShowShootSpeed(float speed)
    {
        DOTween.To(() => 0f , x => ApplySpeedText(x), speed, 0.8f).SetEase(Ease.Linear);
    }

    void ApplySpeedText(float speed)
    {
        fieldScore.text = speed.ToString("0") + " <size=60%>km/h</size>";        
    }
}
