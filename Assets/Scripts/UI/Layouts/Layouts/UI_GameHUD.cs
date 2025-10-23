using System;
using Kasulo.Animations.UI;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UI_GameHUD : UI
{
    [Header("Components")]
    public TMP_Text fieldGameScore;
    public TMP_Text fieldGameTime;
    public TMP_Text fieldGameShootSpeed;
    // public UIAnimationModule uIAnimationModule;
    public Transform toggleRoot;
    public GameObject togglePrefab;
    public Toggle[] toggleList;
    public UIAnimationModule uiAnimSpeed;

    // ADD FADE
    // ADD chutes

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

    internal override void Start()
    {
        base.Start();
        ClearRoundToggles();
        CreateRoundToggles(Config.Instance.configData["game"]["roundCount"].AsInt);
        UpdateRound();
    } 

    void ClearRoundToggles()
    {
        foreach (Transform child in toggleRoot)
        {
            Destroy(child.gameObject);
        }
    }

    void CreateRoundToggles(int roundCount)
    {
        toggleList = new Toggle[roundCount];
        for (int i = 0; i < roundCount; i++)
        {
            var toggleObj = Instantiate(togglePrefab, toggleRoot);
            var toggleComp = toggleObj.GetComponent<Toggle>();
            toggleList[i] = toggleComp;
        }
    }

    private void OnShootResult(bool isGoal, string info)
    {
        fieldGameShootSpeed.text = "";
        uiAnimSpeed.Out();
        Invoke(nameof(UpdateRound), 0.1f);
    }

    private void OnShootSpeed(float speed)
    {
        uiAnimSpeed.In();
        DOTween.To(() => 0f, x => ApplySpeedText(x), speed, 0.8f).SetEase(Ease.Linear);
    }

    void ApplySpeedText(float speed)
    {       
        fieldGameShootSpeed.text = speed.ToString("0") + "<size=40%>km/h</size>";
    }

    void UpdateRound()
    {
        var roundList = GameRoundController.Instance.gameScoreList;
        for (int i = 0; i < toggleList.Length; i++)
        {
            Debug.Log(">>> Updating Round HUD: " + i + " toggle set to " + (i >= roundList.Count));
            toggleList[i].isOn = (roundList.Count > i);
        }
    }

    // internal override void Start()
    // {
    //     uIAnimationModule.gameObject.SetActive(false);
    //     uIAnimationModule.Out();
    // }

    private void OnGameStart()
    {
        UpdateRound();
        // uIAnimationModule.gameObject.SetActive(false);
        // uIAnimationModule.Out();
    }

    private void OnGameEnd()
    {
        // uIAnimationModule.gameObject.SetActive(true);
        // uIAnimationModule.In();
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
