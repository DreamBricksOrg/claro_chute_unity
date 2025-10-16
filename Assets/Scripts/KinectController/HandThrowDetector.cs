using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class HandThrowDetector : MonoBehaviour
{
    [Header("Referências")]
    public GameObject[] ballPrefab;
    public ProjectileBall currentBallInstance = null;
    public int currentBallStyle = 0;

    [Header("Parâmetros de arremesso")]
    public float throwVelocityThreshold = 1.0f;       // velocidade mínima para começar a acumular
    public float throwDecelerationThreshold = 0.5f;   // velocidade abaixo da qual considera que o movimento parou
    public float forwardDotThreshold = 0.7f;
    public float cooldownTime = 1.0f;

    private Vector3 lastHandPos;
    private float currentVelocity = 0f;
    private float lastVelocity = 0f;
    private Vector3 lastDirection = Vector3.zero;

    private bool isAccumulating = false;
    private bool hasThrown = false;
    private float lastThrowDetectedTime = -10f;
    private float accumulatedVelocity = 0f;
    private bool isForward = false;

    private bool wasMoving = false;
    private float movementDistance = 0f;
    private Vector3 movementStartDirection = Vector3.zero;
    private float movementPeakVelocity = 0f;
    public bool isDebugMode = false;
    public bool isPlaying = false;

    void Start()
    {
        lastHandPos = transform.position;
    }

    void OnEnable()
    {
        EventManager.Game.OnGameStartEvent += OnGameStart;
        EventManager.Game.OnGameEndEvent += OnGameEnd;
        EventManager.Game.OnBallStyleEvent += OnBallStyleChange;
    }

    void OnDisable()
    {
        EventManager.Game.OnGameStartEvent -= OnGameStart;
        EventManager.Game.OnGameEndEvent -= OnGameEnd;
        EventManager.Game.OnBallStyleEvent -= OnBallStyleChange;
    }

    private void OnBallStyleChange(int styleId)
    {
        currentBallStyle = styleId;
        if (currentBallInstance != null)
        {
            Destroy(currentBallInstance.gameObject);
            currentBallInstance = null;
        }
        GenerateBall();

        if (currentBallStyle != 0)
        {
            Utils.DelayAction(4f, () =>
            {
                EventManager.Game.SetBallStyle(0);
            });
        }
    }

    private void OnGameStart()
    {
        isPlaying = true;
        currentBallStyle = 0; // Reset to default style
        Invoke(nameof(GenerateBall), 0.5f);
    }

    private void OnGameEnd()
    {
        isPlaying = false;
        if (currentBallInstance != null)
        {
            Destroy(currentBallInstance.gameObject);
            currentBallInstance = null;
        }
        lastThrowDetectedTime = -10f;
        hasThrown = false;
        wasMoving = false;
        movementDistance = 0f;
        accumulatedVelocity = 0f;
        movementPeakVelocity = 0f;
    }

    void Update()
    {
        if (!isPlaying || ballPrefab == null) return;

        Vector3 currentHandPos = transform.position;
        Vector3 displacement = currentHandPos - lastHandPos;
        currentVelocity = displacement.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        Vector3 movementDir = displacement.sqrMagnitude > 0.0001f ? displacement.normalized : lastDirection;
        lastDirection = movementDir;

        float dot = Vector3.Dot(movementDir, Vector3.forward);
        isForward = dot >= forwardDotThreshold;

        bool cooldown = (Time.time - lastThrowDetectedTime) > cooldownTime;
        bool isMovingFast = currentVelocity > throwVelocityThreshold && isForward && cooldown;
        bool isSlowingDown = currentVelocity < throwDecelerationThreshold;

        if (!wasMoving && isMovingFast)
        {
            wasMoving = true;
            movementDistance = 0f;
            accumulatedVelocity = 0f;
            movementStartDirection = movementDir;
            movementPeakVelocity = currentVelocity;
        }

        if (wasMoving && isMovingFast)
        {
            movementDistance += displacement.magnitude;
            accumulatedVelocity += currentVelocity;
            if (currentVelocity > movementPeakVelocity)
                movementPeakVelocity = currentVelocity;
        }

        if (wasMoving && isSlowingDown)
        {
            if (movementDistance > 0.1f && movementPeakVelocity > throwVelocityThreshold && accumulatedVelocity > throwVelocityThreshold && cooldown)
            {
                FireBall(movementStartDirection, accumulatedVelocity * 0.2f);
                hasThrown = true;
                lastThrowDetectedTime = Time.time;
            }
            wasMoving = false;
            movementDistance = 0f;
            accumulatedVelocity = 0f;
            movementPeakVelocity = 0f;
        }

        // Reseta o flag após o cooldown
        if (hasThrown && (Time.time - lastThrowDetectedTime > cooldownTime))
        {
            GenerateBall();
            hasThrown = false;
        }

        lastHandPos = currentHandPos;
        lastVelocity = currentVelocity;

        // Apply ball position
        if (currentBallInstance != null)
        {
            currentBallInstance.transform.position = transform.position;
            currentBallInstance.transform.rotation = transform.rotation;
        }
    }

    public void FireBall(Vector3 direction, float velocity)
    {
        Quaternion upRotation = Quaternion.AngleAxis(-20f, Vector3.right);
        Vector3 adjustedDirection = upRotation * direction;
        currentBallInstance?.Play(adjustedDirection, velocity);
        currentBallInstance = null;
    }

    public void GenerateBall()
    {
        if (!isPlaying) return;
        if (currentBallInstance != null) return;
        var currentBall = Instantiate(ballPrefab[currentBallStyle], transform.position, Quaternion.identity);
        currentBall.transform.localScale = Vector3.zero;
        currentBall.transform.DOScale(new Vector3(1f, 1f, 1f), 0.32f).SetEase(Ease.OutBack);
        currentBallInstance = currentBall.GetComponent<ProjectileBall>();
    }

#if UNITY_EDITOR

    private string info = "";

    void OnDrawGizmos()
    {
        if (!isDebugMode) return;
        Vector3 dir = transform.forward;
        Gizmos.color = isForward ? Color.blue : Color.red;
        Gizmos.DrawRay(transform.position, dir);

        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.fontStyle = FontStyle.Bold;

        if (currentVelocity > 1f)
            style.normal.textColor = Color.red;
        else
            style.normal.textColor = Color.black;

        if (wasMoving)
        {
            info = $"Vel (suave): {currentVelocity:F2}\n" +
                          $"Forward: {(isForward ? "SIM" : "NÃO")}\n" +
                          $"Estado: {(isAccumulating ? "Acumulando" : hasThrown ? "Lançou" : "-")}\n" +
                          $"Vel Acum: {accumulatedVelocity:F2}";

        }
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.1f, info, style);
    }
#endif
}
