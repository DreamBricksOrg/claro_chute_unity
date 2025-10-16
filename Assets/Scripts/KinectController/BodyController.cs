using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
using System.Linq;
using System;

public class BodyController : MonoBehaviour
{
    public BodySourceManager bodyManager;

    [Header("Referências")]
    public Transform headTarget;
    public Transform handLeftTarget;
    public Transform handRightTarget;
    public Transform footLeftTarget;
    public Transform footRightTarget;

    [Header("Escala e posição")]
    public float jointScale = 2f;
    public Vector3 fixedLocation = new Vector3(0f, 0f, 0f);

    [Header("Offsets de Rotação (Euler)")]
    public Vector3 headRotationOffset;
    public Vector3 handLeftRotationOffset;
    public Vector3 handRightRotationOffset;
    public Vector3 footLeftRotationOffset;
    public Vector3 footRightRotationOffset;

    public Vector3 groundLimit = new Vector3(-10f, 0f, -10f);

    private Body[] bodies;
    private bool isBodyTracking = false;

    private Dictionary<Transform, Quaternion> lastRotations = new Dictionary<Transform, Quaternion>();

    // Ultra responsivo	0.05f	0.0005f	Reage rápido, menos suavização
    // Balanceado	0.01f	0.001f	Suavização leve, boa resposta
    // Suave (padrão lento)	0.000001f	0.01f	Muito estável, mas lento demais

    private KalmanFilterVector3 kalmanSpine = new KalmanFilterVector3(0.01f, 0.1f);
    private KalmanFilterVector3 kalmanHead = new KalmanFilterVector3(0.001f, 0.01f);
    private KalmanFilterVector3 kalmanHandL = new KalmanFilterVector3(0.001f, 0.01f);
    private KalmanFilterVector3 kalmanHandR = new KalmanFilterVector3(0.001f, 0.01f);
    private KalmanFilterVector3 kalmanFootL = new KalmanFilterVector3(0.0001f, 0.01f);
    private KalmanFilterVector3 kalmanFootR = new KalmanFilterVector3(0.0001f, 0.01f);

    private KalmanFilterVector4 kalmanHeadRot = new KalmanFilterVector4(0.00001f, 0.01f);
    private KalmanFilterVector4 kalmanHandLRot = new KalmanFilterVector4(0.00001f, 0.01f);
    private KalmanFilterVector4 kalmanHandRRot = new KalmanFilterVector4(0.00001f, 0.01f);
    private KalmanFilterVector4 kalmanFootLRot = new KalmanFilterVector4(0.00001f, 0.01f);
    private KalmanFilterVector4 kalmanFootRRot = new KalmanFilterVector4(0.00001f, 0.01f);

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
        if (trackedBody == null)
        {
            if (isBodyTracking)
            {
                isBodyTracking = false;
                EventManager.Kinect.KinectBodyDetected(false);
            }
            return;
        }
        if (!isBodyTracking)
        {
            isBodyTracking = true;
            EventManager.Kinect.KinectBodyDetected(true);
        }

        // Spine Base
        var spineBaseRaw = new Vector3(
            trackedBody.Joints[JointType.SpineBase].Position.X,
            trackedBody.Joints[JointType.SpineBase].Position.Y,
            -trackedBody.Joints[JointType.SpineBase].Position.Z
        );
        var spineBase = kalmanSpine.Update(spineBaseRaw);

        transform.position = fixedLocation;

        // Head
        SetRelativeJoint(headTarget, trackedBody, JointType.Head, spineBase, kalmanHead, kalmanHeadRot, headRotationOffset);

        // Left Hand
        SetRelativeJoint(handLeftTarget, trackedBody, JointType.HandLeft, spineBase, kalmanHandL, kalmanHandLRot, handLeftRotationOffset);

        // Right Hand
        SetRelativeJoint(handRightTarget, trackedBody, JointType.HandRight, spineBase, kalmanHandR, kalmanHandRRot, handRightRotationOffset);

        // Right Foot
        SetRelativeJoint(footRightTarget, trackedBody, JointType.FootRight, spineBase, kalmanFootR, kalmanFootRRot, footRightRotationOffset);

        // Left Foot
        SetRelativeJoint(footLeftTarget, trackedBody, JointType.FootLeft, spineBase, kalmanFootL, kalmanFootLRot, footLeftRotationOffset);
    }

    void SetRelativeJoint(Transform target, Body body, JointType jointType, Vector3 spineBase, KalmanFilterVector3 kalman, KalmanFilterVector4 kalmanRot, Vector3 rotationOffset)
    {
        if (target == null) return;

        var joint = body.Joints[jointType];
        if (joint.TrackingState == TrackingState.NotTracked) return;

        Vector3 raw = new Vector3(
            joint.Position.X,
            joint.Position.Y,
            -joint.Position.Z
        );
        Vector3 filtered = kalman.Update(raw);

        Vector3 relative = filtered - spineBase;

        target.localPosition = relative * jointScale;
        target.localPosition = ClampVector3(target.localPosition, groundLimit);

        var kinectRot = body.JointOrientations[jointType].Orientation;
        UnityEngine.Vector4 rawQuat = new UnityEngine.Vector4(kinectRot.X, kinectRot.Y, -kinectRot.Z, -kinectRot.W);
        UnityEngine.Vector4 filteredQuat = kalmanRot.Update(rawQuat);
        Quaternion q = new Quaternion(filteredQuat.x, filteredQuat.y, filteredQuat.z, filteredQuat.w);
        Quaternion offset = Quaternion.Euler(rotationOffset);
        Quaternion targetRotation = q * offset;
        targetRotation.Normalize();

        Quaternion lastRotation;
        if (!lastRotations.TryGetValue(target, out lastRotation))
        {
            lastRotation = target.localRotation;
        }
        float smooth = 0.25f;
        Quaternion smoothRotation = Quaternion.Slerp(lastRotation, targetRotation, smooth);
        target.localRotation = smoothRotation;
        lastRotations[target] = smoothRotation;

        // Send Events
        if (jointType == JointType.HandLeft)
        {
            EventManager.Kinect.KinectHandPosition(false, target.position);
        }
        else if (jointType == JointType.HandRight)
        {
            EventManager.Kinect.KinectHandPosition(true, target.position);
        }
        else if (jointType == JointType.FootLeft)
        {
            EventManager.Kinect.KinectFootPosition(false, target.position);
        }
        else if (jointType == JointType.FootRight)
        {
            EventManager.Kinect.KinectFootPosition(true, target.position);
        }
    }

    private Vector3 ClampVector3(Vector3 v, Vector3 min)
    {
        return new Vector3(
            Mathf.Max(v.x, min.x),
            Mathf.Max(v.y, min.y),
            Mathf.Max(v.z, min.z)
        );
    }
}
