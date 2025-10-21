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
    public float forwardCrossMargin = 0.05f;          // margem mínima de avanço sobre o ponto zero

    [Header("Referência frontal")]
    public bool useCameraForward = true;              // usar eixo da câmera para profundidade
    public Transform forwardReference;                // fallback: transform de referência
    private Vector3 lastHandPos;
    private float currentVelocity = 0f;
    private float lastVelocity = 0f;
    private Vector3 lastDirection = Vector3.zero;

    // Controle de ponto zero e cruzamento frontal
    private float zeroForwardPos = 0f;
    private bool zeroSet = false;
    private bool hasCrossedZero = false;
    private bool wasPastZero = false;
    private Vector3 forwardAxis = Vector3.forward;

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

        // Assinar evento de pré-spawn da bola para capturar ponto zero
        if (ballController != null)
            ballController.OnBallPreSpawn += HandleBallPreSpawn;
    }

    void OnDisable()
    {
        EventManager.Game.OnGameStartEvent -= OnGameStart;
        EventManager.Game.OnGameEndEvent -= OnGameEnd;

        if (ballController != null)
            ballController.OnBallPreSpawn -= HandleBallPreSpawn;
    }

    private void OnGameStart()
    {
        isPlaying = true;
        Reset();
        throwVelocityThreshold = Config.Instance.configData["game"]["throwVelocityThreshold"].AsFloat;
        throwDecelerationThreshold = Config.Instance.configData["game"]["throwDecelerationThreshold"].AsFloat;

        // Definir eixo frontal com base na câmera/sensor
        forwardAxis = ResolveForwardAxis();
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

        zeroSet = false;
        hasCrossedZero = false;
        wasPastZero = false;
    }

    private Vector3 ResolveForwardAxis()
    {
        // Usa câmera principal se disponível para medir profundidade
        if (useCameraForward && Camera.main != null)
            return Camera.main.transform.forward.normalized;
        // Usa transform de referência caso configurado
        if (forwardReference != null)
            return forwardReference.forward.normalized;
        // Fallback: eixo Z global
        return Vector3.forward;
    }

    // Capturar ponto zero imediatamente antes da bola nascer
    private void HandleBallPreSpawn()
    {
        // Recalcular eixo frontal imediatamente antes do spawn
        forwardAxis = ResolveForwardAxis();
        // Registrar posição atual projetada no eixo frontal (profundidade)
        zeroForwardPos = Vector3.Dot(transform.position, forwardAxis);
        zeroSet = true;
        hasCrossedZero = false;
        wasPastZero = Vector3.Dot(transform.position, forwardAxis) >= zeroForwardPos + forwardCrossMargin;
    }

    void Update()
    {
        if (!isPlaying) return;

        var currentHandPos = transform.position;
        var displacement = currentHandPos - lastHandPos;
        currentVelocity = displacement.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        var movementDir = displacement.sqrMagnitude > 0.0001f ? displacement.normalized : lastDirection;
        lastDirection = movementDir;

        float dot = Vector3.Dot(movementDir, forwardAxis);
        isForward = dot >= forwardDotThreshold;

        // Coordenada atual ao longo do eixo frontal e checagem de cruzamento do zero
        float coordNow = Vector3.Dot(currentHandPos, forwardAxis);
        bool isPastZero = zeroSet && (coordNow >= zeroForwardPos + forwardCrossMargin);
        if (zeroSet)
        {
            if (isPastZero && !wasPastZero)
                hasCrossedZero = true;
            wasPastZero = isPastZero;
        }

        bool cooldown = (Time.time - lastThrowDetectedTime) > cooldownTime;
        bool isMovingFast = currentVelocity > throwVelocityThreshold && isForward && cooldown && isPastZero;
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
            // Exigir que tenha cruzado o ponto zero para frente antes de disparar
            if (movementDistance > 0.1f && movementDistance < 5f && accumulatedVelocity < 50f && hasCrossedZero)
            {
                Debug.Log(accumulatedVelocity + " | " + movementDistance + " | " + movementPeakVelocity);
                ballController.ShootBall(movementStartDirection, accumulatedVelocity * 0.2f);
                hasThrown = true;
                lastThrowDetectedTime = Time.time;

                // Resetar estados de cruzamento para próxima bola
                hasCrossedZero = false;
                zeroSet = false;
                wasPastZero = false;
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


// #if UNITY_EDITOR

//     private string info = "";

//     void OnDrawGizmos()
//     {
//         if (!isDebugMode) return;
//         Vector3 dir = forwardAxis;
//         Gizmos.color = isForward ? Color.blue : Color.red;
//         Gizmos.DrawRay(transform.position, dir * 0.3f);

//         // Desenhar referência do ponto zero ao longo do eixo frontal
//         if (zeroSet)
//         {
//             Gizmos.color = Color.green;
//             Vector3 zeroPoint = transform.position;
//             float offset = zeroForwardPos - Vector3.Dot(transform.position, forwardAxis);
//             zeroPoint += forwardAxis.normalized * offset;
//             Gizmos.DrawWireSphere(zeroPoint, 0.05f);
//         }

//         GUIStyle style = new GUIStyle();
//         style.fontSize = 20;
//         style.fontStyle = FontStyle.Bold;

//         if (currentVelocity > 1f)
//             style.normal.textColor = Color.red;
//         else
//             style.normal.textColor = Color.black;

//         if (wasMoving)
//         {
//             info = $"Vel (suave): {currentVelocity:F2}\n" +
//                           $"Forward: {(isForward ? "SIM" : "NÃO")}\n" +
//                           $"Cruzou Zero: {(hasCrossedZero ? "SIM" : "NÃO")}\n" +
//                           $"Estado: {(isAccumulating ? "Acumulando" : hasThrown ? "Lançou" : "-")}\n" +
//                           $"Vel Acum: {accumulatedVelocity:F2}";
//         }
//         UnityEditor.Handles.Label(transform.position + Vector3.up * 0.1f, info, style);
//     }
// #endif
}
