using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IInteract, IElementState
{

    public Transform parentTransform;
    public Collider triggerCollider;
    public ParticleSystem fxParticle;
    public GameObject worldScorePrefab;
    public Transform pivotStageScore;
    [HideInInspector] public int score = 0;

    public void OnBegin()
    {
    }

    public void OnHit()
    {
        OnPlay();
    }

    void SetScore(int score)
    {
        EventManager.Game.SetGameScore(score);
        var worldScore = Instantiate(worldScorePrefab);
        worldScore.transform.SetParent(null);
        worldScore.transform.position = pivotStageScore.transform.position;
        worldScore.transform.rotation = pivotStageScore.transform.rotation;
        worldScore.transform.localScale = pivotStageScore.transform.localScale * 0.01f;
        var comp = worldScore.GetComponent<WorldScoreComponent>();
        comp?.SetValue(score);
    }

    void SetColliderActive(bool isActive)
    {
        triggerCollider.enabled = isActive;
    }

    public void OnPrepare()
    {
        SetColliderActive(true);
    }

    public virtual void OnPlay()
    {
        SetColliderActive(false);
        fxParticle?.Play();
        SetScore(score);
        parentTransform?.GetComponent<IElementState>()?.OnReset();
    }

    public void OnReset()
    {
        SetColliderActive(false);
    }

    public void OnCancel()
    {
    }
}

public enum EntityType
{
    Ally,
    Enemy,
    Neutral,
    Bonus,
    Special
}
