using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Linq;

public class UI_FinalScore : UI
{
    public Button btnConfirm;
    public TMP_Text fieldScore;

    internal override void Awake()
    {
        base.Awake();
        btnConfirm.onClick.AddListener(() =>
        {
            EventManager.Section.SetSection(SectionTypes.Intro);
        });
    }

    internal override void OnShow(object data, Action<object> callback)
    {
        base.OnShow(data, callback);
        var maxShootSpeed = GameRoundController.Instance.gameScoreList.Max(x => x.speed);
        ShowShootSpeed(maxShootSpeed);
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
