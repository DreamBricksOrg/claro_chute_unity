using System;
using Kasulo.Animations.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;

public class UI_GameHUD : UI
{
    [Header("Components")]
    public TMP_Text fieldGameScore;
    public TMP_Text fieldGameTime;
    public TMP_Text fieldGameShootSpeed;
    public UIAnimationModule uIAnimationModule;
    public UIAnimationModule uiAnimSpeed;

    void OnEnable()
    {
        EventManager.Game.OnGameScoreUpdateEvent += OnGameScore;
        EventManager.Game.OnGameTimeChangeEvent += OnGameTimeChange;
        EventManager.Game.OnGameStartEvent += OnGameStart;
        EventManager.Game.OnGameEndEvent += OnGameEnd;
        EventManager.Game.OnShootSpeedEvent += OnShootSpeed;
        EventManager.Game.OnShootResultEvent += OnShootResult;
    }

    void OnDisable()
    {
        EventManager.Game.OnGameScoreUpdateEvent -= OnGameScore;
        EventManager.Game.OnGameTimeChangeEvent -= OnGameTimeChange;
        EventManager.Game.OnGameStartEvent -= OnGameStart;
        EventManager.Game.OnGameEndEvent -= OnGameEnd;
        EventManager.Game.OnShootSpeedEvent -= OnShootSpeed;
        EventManager.Game.OnShootResultEvent -= OnShootResult;
    }

    private void OnShootResult(bool isGoal, string info)
    {
        fieldGameShootSpeed.text = "";
        uiAnimSpeed.Out();
    }

    private void OnShootSpeed(float speed)
    {
        uiAnimSpeed.In();
        DOTween.To(() => 0f, x => ApplySpeedText(x), speed, 0.8f).SetEase(Ease.Linear);
    }

    void ApplySpeedText(float speed)
    {
        fieldGameShootSpeed.text = speed.ToString("0") + " <size=60%>km/h</size>";        
    }

    internal override void Start()
    {
        uIAnimationModule.gameObject.SetActive(false);
        uIAnimationModule.Out();
    }

    private void OnGameStart()
    {
        uIAnimationModule.gameObject.SetActive(false);
        uIAnimationModule.Out();
    }

    private void OnGameEnd()
    {
        uIAnimationModule.gameObject.SetActive(true);
        uIAnimationModule.In();
    }

    private void OnGameTimeChange(float time)
    {
        fieldGameTime.text = time.ToString("00");
    }

    private void OnGameScore(int globalScore)
    {
        fieldGameScore.text = globalScore.ToString("0");
    }
}
