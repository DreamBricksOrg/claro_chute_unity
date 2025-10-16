using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class JointHead : MonoBehaviour
{
    public BodySourceManager bodyManager;
    private Body[] bodies;

    private Vector3 lastBasePoint = Vector3.zero;
    private KalmanFilterVector3 kalmanHeadPoint = new KalmanFilterVector3();
    private KalmanFilterVector3 kalmanNeckPoint = new KalmanFilterVector3();
    private float lastVelocity = 0f;

    public float currentVelocity = 0f;
    public float forwardDotThreshold = 0.6f;

    [Header("Z travado")]
    public float fixedZ = 0f;

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

        // Posição real (Kinect) com Z invertido
        var headPointRaw = new Vector3(
            trackedBody.Joints[JointType.Head].Position.X,
            trackedBody.Joints[JointType.Head].Position.Y,
            -trackedBody.Joints[JointType.Head].Position.Z
        );

        var neckPointRaw = new Vector3(
            trackedBody.Joints[JointType.Neck].Position.X,
            trackedBody.Joints[JointType.Neck].Position.Y,
            -trackedBody.Joints[JointType.Neck].Position.Z
        );

        // Aplica filtros
        var headPoint = kalmanHeadPoint.Update(headPointRaw);
        var neckPoint = kalmanNeckPoint.Update(neckPointRaw);

        // 🟢 Travar a posição Z
        Vector3 fixedHeadPoint = new Vector3(headPoint.x, headPoint.y, fixedZ);
        transform.position = fixedHeadPoint;

        // 🧭 Usa o vetor entre cabeça e pescoço (real) para rotação
        Vector3 displacement = headPoint - neckPoint;
        currentVelocity = displacement.magnitude / Time.deltaTime;
        Vector3 movementDir = displacement.normalized;

        // Aplica rotação
        if (movementDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDir, Vector3.forward);
            transform.rotation = targetRotation * Quaternion.Euler(-90, 0, 180);
        }
    }
}
