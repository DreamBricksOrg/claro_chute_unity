using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JointType = nuitrack.JointType;
using NuitrackSDK.Calibration;
using NuitrackSDK;

public class NuiTrackBodyController : NuitrackSDK.Avatar.BaseAvatar
{
    [Header("Referências")]
    public Transform headTarget;
    public Transform handLeftTarget;
    public Transform handRightTarget;
    public Transform footLeftTarget;
    public Transform footRightTarget;
    public bool isFootLeftVisible;
    public bool isFootRightVisible;

    public int footLeftVisibleCount;

    public int footRightVisibleCount;


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

    // private bool isBodyTracking = false;

    private Dictionary<Transform, Quaternion> lastRotations = new Dictionary<Transform, Quaternion>();

    // Ultra responsivo	0.05f	0.0005f	Reage rápido, menos suavização
    // Balanceado	0.01f	0.001f	Suavização leve, boa resposta
    // Suave (padrão lento)	0.000001f	0.01f	Muito estável, mas lento demais

    private KalmanFilterVector3 kalmanSpine = new KalmanFilterVector3(0.001f, 0.1f);
    private KalmanFilterVector3 kalmanHead = new KalmanFilterVector3(0.0001f, 0.01f);
    private KalmanFilterVector3 kalmanHandL = new KalmanFilterVector3(0.0005f, 0.001f);
    private KalmanFilterVector3 kalmanHandR = new KalmanFilterVector3(0.0005f, 0.001f);
    private KalmanFilterVector3 kalmanFootL = new KalmanFilterVector3(0.0005f, 0.001f);
    private KalmanFilterVector3 kalmanFootR = new KalmanFilterVector3(0.0005f, 0.001f);

    private KalmanFilterVector4 kalmanHeadRot = new KalmanFilterVector4(0.00001f, 0.01f);
    private KalmanFilterVector4 kalmanHandLRot = new KalmanFilterVector4(0.00001f, 0.01f);
    private KalmanFilterVector4 kalmanHandRRot = new KalmanFilterVector4(0.00001f, 0.01f);
    private KalmanFilterVector4 kalmanFootLRot = new KalmanFilterVector4(0.00001f, 0.01f);
    private KalmanFilterVector4 kalmanFootRRot = new KalmanFilterVector4(0.00001f, 0.01f);


    JointType[] jointsInfo = new JointType[]
    {
        JointType.Head,
        JointType.Neck,
        JointType.LeftCollar,
        JointType.RightCollar,
        JointType.Torso,
        JointType.Waist,
        JointType.LeftShoulder,
        JointType.RightShoulder,
        JointType.LeftElbow,
        JointType.RightElbow,
        JointType.LeftWrist,
        JointType.RightWrist,
        JointType.LeftHand,
        JointType.RightHand,
        JointType.LeftHip,
        JointType.RightHip,
        JointType.LeftKnee,
        JointType.RightKnee,
        JointType.LeftAnkle,
        JointType.RightAnkle
    };

    void Start()
    {
        var pos = Config.Instance.configData["game"]["playerLocation"];
        fixedLocation = new Vector3(pos["x"].AsFloat, pos["y"].AsFloat, pos["z"].AsFloat);
    }

    void Update()
    {
        var user = ControllerUser;

        if (user == null || user.Skeleton == null)
            return;

        var joitWaist = GetJoint(JointType.Waist);

        var spineBaseRaw = new Vector3(
            joitWaist.Position.x,
            joitWaist.Position.y,
            -joitWaist.Position.z
        );
        var skeletonRootBase = kalmanSpine.Update(spineBaseRaw);
        // var skeletonRootBase = spineBaseRaw;

        transform.position = fixedLocation;

        // Head
        SetRelativeJoint(headTarget, JointType.Head, skeletonRootBase, kalmanHead, kalmanHeadRot, headRotationOffset);

        // Left Hand
        SetRelativeJoint(handLeftTarget, JointType.LeftHand, skeletonRootBase, kalmanHandL, kalmanHandLRot, handLeftRotationOffset);

        // Right Hand
        SetRelativeJoint(handRightTarget, JointType.RightHand, skeletonRootBase, kalmanHandR, kalmanHandRRot, handRightRotationOffset);

        // Right Foot
        SetRelativeJoint(footRightTarget, JointType.RightAnkle, skeletonRootBase, kalmanFootR, kalmanFootRRot, footRightRotationOffset);

        // Left Foot
        SetRelativeJoint(footLeftTarget, JointType.LeftAnkle, skeletonRootBase, kalmanFootL, kalmanFootLRot, footLeftRotationOffset);
    }

    void SetRelativeJoint(Transform target, JointType jointType, Vector3 rootBase, KalmanFilterVector3 kalman, KalmanFilterVector4 kalmanRot, Vector3 rotationOffset)
    {
        if (target == null) return;

        var joint = GetJoint(jointType);


        if (jointType == JointType.LeftAnkle)
        {
            if (joint.Confidence >= 0.5f)
            {
                footLeftVisibleCount++;
            }
            else
            {
                footLeftVisibleCount = 0;
            }

            isFootLeftVisible = footLeftVisibleCount > 2; 
        }
        else if (jointType == JointType.RightAnkle)
        {
            if (joint.Confidence >= 0.5f)
            {
                footRightVisibleCount++;
            }
            else
            {
                footRightVisibleCount = 0;
            }

            isFootRightVisible = footRightVisibleCount > 2;
        }

        if (joint.Confidence <= 0.5f) return;

        var raw = new Vector3(
            -joint.Position.x,
            joint.Position.y,
            -joint.Position.z
        );

        var filtered = kalman.Update(raw);
        // var filtered = raw;
        var relative = filtered - rootBase;

        // if (jointType == JointType.RightAnkle)
        // {
        //     Debug.Log("RF Raw: " + joint.Position.x + "/" + joint.Position.y + "/" + joint.Position.z + " K: " + filtered.x + "/" + filtered.y + "/" + filtered.z + "\n");
        // }
        // if (jointType == JointType.LeftAnkle)
        // {
        //     Debug.Log("LF Raw: " + joint.Position.x + "/" + joint.Position.y + "/" + joint.Position.z + " K: " + filtered.x + "/" + filtered.y + "/" + filtered.z + "\n");
        // }
    
        target.localPosition = relative * jointScale;
        target.localPosition = ClampVector3(target.localPosition, groundLimit);

        // Send Events
        if (jointType == JointType.LeftHand)
        {
            EventManager.Kinect.KinectHandPosition(false, target.position);
        }
        else if (jointType == JointType.RightHand)
        {
            EventManager.Kinect.KinectHandPosition(true, target.position);
        }
        else if (jointType == JointType.LeftFoot)
        {
            EventManager.Kinect.KinectFootPosition(false, target.position);
        }
        else if (jointType == JointType.RightFoot)
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