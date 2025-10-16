using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoundController : MonoBehaviour
{

    [Serializable]
    public class RoundResult
    {
        public bool isGoal;
        public string info;
        public float speed;
    }

    public static GameRoundController Instance;
    public static bool IsGameRunning = false;
    Coroutine gameplayRoundTimeRoutine;

    public List<RoundResult> gameScoreList = new List<RoundResult>();
    [HideInInspector] public float currentShootSpeed = 0f;

    [SerializeField]
    [HideInInspector] private float _currentGameTime = 0f;
    public float CurrentGameTime
    {
        get => _currentGameTime;
        set
        {
            _currentGameTime = Mathf.Clamp(value, 0f, 9999f);
            EventManager.Game.SetGameTime(_currentGameTime);
        }
    }

    [SerializeField]
    [HideInInspector] private int _currentGameScore = 0;
    public int CurrentGameScore
    {
        get => _currentGameScore;
        set
        {
            _currentGameScore = Mathf.Clamp(value, 0, 9999);
            EventManager.Game.UpdateGameScore(_currentGameScore);
        }
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ResetGame();
        AudioController.PlayBGS(AudioTypes.BGS_crowd, 1f);
    }

    void OnEnable()
    {
        EventManager.Game.OnGameScoreEvent += OnGameScore;
        EventManager.Game.OnGameStartEvent += OnGameStart;
        EventManager.Game.OnGameEndEvent += OnGameEnd;
        EventManager.Game.OnShootResultEvent += OnShootResult;
        EventManager.Game.OnShootSpeedEvent += OnShootSpeed;
    }

    void OnDisable()
    {
        EventManager.Game.OnGameScoreEvent -= OnGameScore;
        EventManager.Game.OnGameStartEvent -= OnGameStart;
        EventManager.Game.OnGameEndEvent -= OnGameEnd;
        EventManager.Game.OnShootResultEvent -= OnShootResult;
        EventManager.Game.OnShootSpeedEvent -= OnShootSpeed;
    }

    private void OnShootSpeed(float speed)
    {
        currentShootSpeed = speed;
    }

    private void OnShootResult(bool isGoal, string info)
    {
        gameScoreList.Add(new RoundResult() { isGoal = isGoal, info = info, speed = currentShootSpeed });
        EventManager.Game.SetGameScore(1);

        if (gameScoreList.Count >= Config.Instance.configData["game"]["roundCount"].AsInt)
        {
            EndGame();
        }
        else
        {
            EventManager.Game.NextRound();
        }
    }

    private void OnGameStart()
    {
        PlayGame();
    }

    private void OnGameEnd()
    {
        Utils.DelayAction(4f, () =>
        {
            EventManager.Section.SetSection(SectionTypes.FinalScore);
        });
    }

    void PlayGame()
    {
        ResetGame();
        if (gameplayRoundTimeRoutine != null) StopCoroutine(gameplayRoundTimeRoutine);
        gameplayRoundTimeRoutine = StartCoroutine(GameplayRoundTime());
    }

    IEnumerator GameplayRoundTime()
    {
        IsGameRunning = true;
        while (CurrentGameTime > 0f)
        {
            yield return new WaitForSeconds(1f);
            CurrentGameTime -= 1f;
            EventManager.Game.SetGameTime(CurrentGameTime);
        }
        EndGame();
        yield return null;
    }

    private void OnGameScore(int score)
    {
        CurrentGameScore += score;
    }

    private void ResetGame()
    {
        CurrentGameScore = 0;
        gameScoreList.Clear();
        CurrentGameTime = Config.Instance.configData["game"]["duration"].AsInt;
    }

    void EndGame()
    {
        CancelInvoke();
        if (gameplayRoundTimeRoutine != null) StopCoroutine(gameplayRoundTimeRoutine);
        gameplayRoundTimeRoutine = null;
        IsGameRunning = false;
        EventManager.Game.SetGameTime(CurrentGameTime);
        EventManager.Game.UpdateGameScore(CurrentGameScore);
        EventManager.Game.GameEnd();
    }

}
