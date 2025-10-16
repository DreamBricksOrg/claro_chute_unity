using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class Joint : MonoBehaviour
{
    public BodySourceManager bodyManager;
    public JointType TrackedJointBase;
    public JointType TrackedJointTip;

    public GameObject ballPrefab;

    private Body[] bodies;
    public float ballSpeed = 20f;

    private bool hasThrown = false;
    private Vector3 lastBasePoint = Vector3.zero;
    public float throwVelocityThreshold = 0.5f;
    private float lastThrowDetectedTime = -10f;

    public bool isForward = false;
    public float forwardDotThreshold = 1f;

    private KalmanFilterVector3 kalmanBasePoint = new KalmanFilterVector3();
    private bool isThrowing = false;
    private float lastVelocity = 0f;
    public float throwDecelerationThreshold = 1.0f;
    public float currentVelocity = 0f;

    [Header("Z travado")]
    public float fixedZ = 0f; // valor fixo de Z no espaço Unity

    void Update()
    {
        if (bodyManager == null) return;
        bodies = bodyManager.GetData();
        if (bodies == null) return;

        Body trackedBody = null;
        foreach (var b in bodies)
        {
            if (b != null && b.IsTracked)
            {
                trackedBody = b;
                break;
            }
        }
        if (trackedBody == null) return;

        // Posição real do joint (com filtro)
        var basePointRaw = new Vector3(
            trackedBody.Joints[TrackedJointBase].Position.X,
            trackedBody.Joints[TrackedJointBase].Position.Y,
            -trackedBody.Joints[TrackedJointBase].Position.Z
        );

        var basePoint = kalmanBasePoint.Update(basePointRaw);

        // Calcula o movimento com basePoint REAL (com Z original)
        Vector3 displacement = basePoint - lastBasePoint;
        currentVelocity = displacement.magnitude / Time.deltaTime;
        Vector3 movementDir = displacement.normalized;

        float dot = Vector3.Dot(movementDir, Vector3.forward);
        isForward = dot >= forwardDotThreshold;
        bool cooldown = (Time.time - lastThrowDetectedTime) > 1.0f;

        // Arremesso detectado
        if (!isThrowing && currentVelocity > throwVelocityThreshold && isForward && cooldown)
        {
            isThrowing = true;
        }

        if (isThrowing && currentVelocity < throwDecelerationThreshold && isForward && cooldown)
        {
            hasThrown = true;
            lastThrowDetectedTime = Time.time;
            Debug.Log($"Arremesso detectado! Velocidade: {lastVelocity:F2} -> {currentVelocity:F2} Direção: {movementDir}");
            FireBall(movementDir, currentVelocity);
            isThrowing = false;
        }

        if (isThrowing && (!isForward || !cooldown))
        {
            isThrowing = false;
        }

        if (hasThrown && (Time.time - lastThrowDetectedTime > 1.0f))
        {
            hasThrown = false;
        }

        // 🎯 AQUI É ONDE TRAVAMOS O Z
        Vector3 fixedBasePoint = new Vector3(basePoint.x, basePoint.y, fixedZ);
        transform.position = fixedBasePoint;

        // Visual debug
        Debug.DrawRay(basePoint, movementDir, isForward ? Color.blue : Color.red, 0.1f);

        lastBasePoint = basePoint;
        lastVelocity = currentVelocity;
    }

    void FireBall(Vector3 direction, float velocity = 2f)
    {
        if (ballPrefab == null) return;
        GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.LookRotation(direction));
        Destroy(ball, 5f);

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * (velocity * ballSpeed);
            rb.angularDrag = 0.05f;
            rb.AddTorque(ball.transform.right * 0.02f, ForceMode.Impulse);
        }
    }
}
