using System;
using Kasulo.Animations.UI;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using SimpleJSON;

public class UI_GameHUD : UI
{
    [Header("Components")]
    public TMP_Text fieldGameScore;
    public TMP_Text fieldGameTime;
    public TMP_Text fieldGameShootSpeed;
    public Transform toggleRoot;
    public GameObject togglePrefab;
    public CanvasGroup fadeCanvasGroup;
    private Toggle[] toggleList;
    public UIAnimationModule uiAnimSpeed;
    bool isRecording = false;
    public bool debug_disableRecord = false;

    void OnEnable()
    {
        EventManager.Game.OnGameScoreUpdateEvent += OnGameScore;
        EventManager.Game.OnGameTimeChangeEvent += OnGameTimeChange;
        EventManager.Game.OnGameStartEvent += OnGameStart;
        EventManager.Game.OnGameEndEvent += OnGameEnd;
        EventManager.Game.OnShootSpeedEvent += OnShootSpeed;
        EventManager.Game.OnShootResultEvent += OnShootResult;
        EventManager.Game.OnNextRoundEvent += OnNextRound;
    }

    void OnDisable()
    {
        EventManager.Game.OnGameScoreUpdateEvent -= OnGameScore;
        EventManager.Game.OnGameTimeChangeEvent -= OnGameTimeChange;
        EventManager.Game.OnGameStartEvent -= OnGameStart;
        EventManager.Game.OnGameEndEvent -= OnGameEnd;
        EventManager.Game.OnShootSpeedEvent -= OnShootSpeed;
        EventManager.Game.OnShootResultEvent -= OnShootResult;
        EventManager.Game.OnNextRoundEvent -= OnNextRound;
    }

    internal override void OnShow(object data, Action<object> callback)
    {
        base.OnShow(data, callback);
        isRecording = false;
    }

    private void OnNextRound()
    {
        FadeInOut();
        if (GameRoundController.Instance.gameScoreList.Count == 1)
        {
            if(debug_disableRecord && Application.isEditor) return;
            StartRecording();
        }
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

        if (isRecording) StopRecording();
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
            toggleList[i].isOn = (roundList.Count > i);
        }
    }

    void FadeInOut()
    {
        fadeCanvasGroup.DOKill();
        var sequence = DOTween.Sequence();
        fadeCanvasGroup.alpha = 0f;
        sequence.Append(fadeCanvasGroup.DOFade(1f, 0.5f))
               .Append(fadeCanvasGroup.DOFade(0f, 0.5f));
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

    void StartRecording()
    {
        isRecording = true;
        Debug.Log("<<Recording Started>>");
        API.Request("/obs/recording/start",
           onSuccess: (result) =>
           {
               var jsonData = JSON.Parse(result);
               Debug.Log(jsonData["message"].Value);
           },
           onError: (error) =>
           {
               Debug.LogError("API Error: " + error);
           }
       );
    }

    void StopRecording()
    {
        isRecording = false;
        Debug.Log("<<Recording STOPPED>>");
        API.Request("/obs/recording/stop",
            onSuccess: (result) =>
            {
                var jsonData = JSON.Parse(result);
                Debug.Log(jsonData["message"].Value);
            },
            onError: (error) =>
            {
                Debug.LogError("API Error: " + error);
            }
        );
    }
}
