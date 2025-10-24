using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GoalKeeperController : MonoBehaviour
{
    public Animator animator;

    public float turnValue = 1f;
    public float heightValue = 1f;
    public float offsetFactor = 1f;

    void OnEnable()
    {
        EventManager.Game.OnGameStartEvent += OnGameStart;
        EventManager.Game.OnGameEndEvent += OnGameEnd;
        EventManager.Game.OnShootSpeedEvent += OnShootSpeed;
        EventManager.Game.OnShootResultEvent += OnShootResult;
        EventManager.Game.OnNextRoundEvent += OnNextRound;
    }

    void OnDisable()
    {
        EventManager.Game.OnGameStartEvent -= OnGameStart;
        EventManager.Game.OnGameEndEvent -= OnGameEnd;
        EventManager.Game.OnShootSpeedEvent -= OnShootSpeed;
        EventManager.Game.OnShootResultEvent -= OnShootResult;
        EventManager.Game.OnNextRoundEvent -= OnNextRound;
    }

    private void OnGameStart()
    {
        ResetCharacter();
    }

    private void OnGameEnd()
    {
    }

    private void OnShootResult(bool isGoal, string info)
    {
        // ResetCharacter();
    }

    private void OnShootSpeed(float speed)
    {
        Action();
    }

    private void OnNextRound()
    {
        Invoke(nameof(ResetCharacter), 0.5f);
    }

    void Action()
    {
        var mx = Random.Range(-1f, 1f) * turnValue;
        var my = Random.Range(0f, 1f) * heightValue;
        animator.SetFloat("Turn", mx);
        animator.SetFloat("Height", my);
        animator.SetTrigger("Dive");
        animator.transform.DOLocalMove(new Vector3(mx * offsetFactor, 0f, 0f), 1f).SetEase(Ease.InOutExpo);
    }

    void ResetCharacter()
    {
        animator.transform.localPosition = Vector3.zero;
        animator.transform.localRotation = Quaternion.identity;
        animator.Rebind();
        animator.SetFloat("Turn", 0);
        animator.SetFloat("Height", 0);
        animator.ResetTrigger("Dive");
        animator.ResetTrigger("Idle");
        animator.SetTrigger("Idle");
    }



}
