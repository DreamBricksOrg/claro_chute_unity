using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowDetector : MonoBehaviour
{
    public BallController ballController;

    [Header("Parâmetros de arremesso")]
    public float throwVelocityThreshold = 1.0f;       // velocidade mínima para começar a acumular
    public float throwDecelerationThreshold = 0.5f;   // velocidade abaixo da qual considera que o movimento parou
    public float forwardDotThreshold = 0.7f;
    public float cooldownTime = 1.0f;

    private Vector3 lastHandPos;
    private float currentVelocity = 0f;
    private float lastVelocity = 0f;
    private Vector3 lastDirection = Vector3.zero;

    public Renderer meshrender;

    // public Transform pinnedPosition;

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

    void OnEnable()
    {
        EventManager.Game.OnGameStartEvent += OnGameStart;
        EventManager.Game.OnGameEndEvent += OnGameEnd;
    }

    void OnDisable()
    {
        EventManager.Game.OnGameStartEvent -= OnGameStart;
        EventManager.Game.OnGameEndEvent -= OnGameEnd;
    }

    void Start()
    {
        if (meshrender != null)
        {
            meshrender.enabled = Config.Instance.configData["kinect"]["showDebugFeet"].AsBool;            
        };
    }

    private void OnGameStart()
    {
        isPlaying = true;
        Reset();
        throwVelocityThreshold = Config.Instance.configData["game"]["throwVelocityThreshold"].AsFloat;
        throwDecelerationThreshold = Config.Instance.configData["game"]["throwDecelerationThreshold"].AsFloat;
    }

    private void OnGameEnd()
    {
        isPlaying = false;
        Reset();
    }

    void Reset()
    {
        lastThrowDetectedTime = -10f;
        hasThrown = false;
        wasMoving = false;
        movementDistance = 0f;
        accumulatedVelocity = 0f;
        movementPeakVelocity = 0f;
    }

    void Update()
    {
        if (!isPlaying) return;

        var currentHandPos = transform.position;
        var displacement = currentHandPos - lastHandPos;
        currentVelocity = displacement.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        var movementDir = displacement.sqrMagnitude > 0.0001f ? displacement.normalized : lastDirection;
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
            // if (movementDistance > 0.1f && movementDistance < 5f && accumulatedVelocity < 50f && movementPeakVelocity > throwVelocityThreshold && accumulatedVelocity > throwVelocityThreshold && cooldown)
            if (movementDistance > 0.1f && movementDistance < 5f && accumulatedVelocity < 50f)
            {
                Debug.Log(accumulatedVelocity + " | " + movementDistance + " | " + movementPeakVelocity);
                ballController.ShootBall(movementStartDirection, accumulatedVelocity * 0.2f);
                hasThrown = true;
                lastThrowDetectedTime = Time.time;
            }
            wasMoving = false;
            movementDistance = 0f;
            accumulatedVelocity = 0f;
            movementPeakVelocity = 0f;
        }

        if (hasThrown && (Time.time - lastThrowDetectedTime > cooldownTime))
        {
            hasThrown = false;
        }

        lastHandPos = currentHandPos;
        lastVelocity = currentVelocity;
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
