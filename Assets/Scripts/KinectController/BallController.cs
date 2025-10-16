using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class BallController : MonoBehaviour
{
    [Header("Referências")]
    public GameObject[] ballPrefab;
    public ProjectileBall currentBallInstance = null;
    public int currentBallStyle = 0;

    public Transform pinnedPosition;
    public bool isPlaying = false;

    void OnEnable()
    {
        EventManager.Game.OnGameStartEvent += OnGameStart;
        EventManager.Game.OnGameEndEvent += OnGameEnd;
        EventManager.Game.OnBallStyleEvent += OnBallStyleChange;
        EventManager.Game.OnNextRoundEvent += OnNextRound;

    }

    void OnDisable()
    {
        EventManager.Game.OnGameStartEvent -= OnGameStart;
        EventManager.Game.OnGameEndEvent -= OnGameEnd;
        EventManager.Game.OnBallStyleEvent -= OnBallStyleChange;
        EventManager.Game.OnNextRoundEvent -= OnNextRound;
    }

    private void OnNextRound()
    {
        Invoke(nameof(GenerateBall), 0.5f);
    }

    private void OnBallStyleChange(int styleId)
    {
        // currentBallStyle = styleId;
        // if (currentBallInstance != null)
        // {
        //     Destroy(currentBallInstance.gameObject);
        //     currentBallInstance = null;
        // }

        // GenerateBall();

        // if (currentBallStyle != 0)
        // {
        //     Utils.DelayAction(4f, () =>
        //     {
        //         EventManager.Game.SetBallStyle(0);
        //     });
        // }
    }

    private void OnGameStart()
    {
        isPlaying = true;
        currentBallStyle = 0;
        Invoke(nameof(GenerateBall), 3f);
    }

    private void OnGameEnd()
    {
        isPlaying = false;
        if (currentBallInstance != null)
        {
            Destroy(currentBallInstance.gameObject);
            currentBallInstance = null;
        }
    }

    public void ShootBall(Vector3 direction, float velocity)
    {
        var limitRange = Config.Instance.configData["game"]["ballSpeedLimitRange"];
        velocity += Config.Instance.configData["game"]["ballPower"].AsFloat;
        velocity = Mathf.Clamp(velocity, limitRange[0].AsFloat, limitRange[1].AsFloat);
        var upRotation = Quaternion.AngleAxis(-20f, Vector3.right);
        var adjustedDirection = upRotation * direction;
        currentBallInstance?.Play(adjustedDirection, velocity);
        currentBallInstance = null;
    }

    public void GenerateBall()
    {
        if (!isPlaying) return;
        if (currentBallInstance != null) return;
        var currentBall = Instantiate(ballPrefab[currentBallStyle], pinnedPosition.position, Quaternion.identity);
        currentBall.transform.localScale = Vector3.zero;
        currentBall.transform.position = pinnedPosition.position + new Vector3(0f, 1f, 0f);
        currentBall.transform.DOMoveY(pinnedPosition.position.y, 0.8f).SetEase(Ease.OutBounce);
        currentBall.transform.DOScale(new Vector3(1f, 1f, 1f), 0.32f).SetEase(Ease.OutBack);
        currentBallInstance = currentBall.GetComponent<ProjectileBall>();
        EventManager.Game.BallCreated(currentBall);
    }

}
