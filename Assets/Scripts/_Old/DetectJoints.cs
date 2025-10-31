using UnityEngine;
using System.Collections;
using Windows.Kinect;

// Classe simples de filtro de Kalman para 1D
public class KalmanFilter
{
   private float Q; // Variância do processo
   private float R; // Variância da medição
   private float X; // Valor estimado
   private float P; // Erro estimado
   private float K; // Kalman Gain
   private bool initialized = false;

   public KalmanFilter(float q = 0.01f, float r = 0.1f)
   {
      Q = q;
      R = r;
   }

   public float Update(float measurement)
   {
      if (!initialized)
      {
         X = measurement;
         P = 1;
         initialized = true;
      }
      // Prediction update
      P = P + Q;
      // Measurement update
      K = P / (P + R);
      X = X + K * (measurement - X);
      P = (1 - K) * P;
      return X;
   }
}

public class DetectJoints : MonoBehaviour
{

   public GameObject BodySrcManager;
   public JointType TrackedJoint;
   private BodySourceManager bodyManager;
   private Body[] bodies;
   public float multiplier = 10f;

   private KalmanFilter kalmanX = new KalmanFilter(0.001f, 0.5f);
   private KalmanFilter kalmanY = new KalmanFilter(0.001f, 0.5f);
   private KalmanFilter kalmanZ = new KalmanFilter(0.001f, 0.5f);

   // Filtros de Kalman para rotação com parâmetros mais suaves
   private KalmanFilter kalmanRotX = new KalmanFilter(0.001f, 0.5f);
   private KalmanFilter kalmanRotY = new KalmanFilter(0.001f, 0.5f);
   private KalmanFilter kalmanRotZ = new KalmanFilter(0.001f, 0.5f);
   private KalmanFilter kalmanRotW = new KalmanFilter(0.001f, 0.5f);

   private float lastFilteredZ = 0f;
   public float throwThreshold = 1.0f; // Ajuste este valor conforme necessário

   public float throwResetThreshold = 0.3f;
   private bool hasThrown = false;

   public Transform baseTransform; // Transformação base para referência, se necessário

   // Use this for initialization
   void Start()
   {

      if (BodySrcManager == null)
      {
         Debug.Log("Asign Game Object with Body Source Manager");
      }
      else
      {
         bodyManager = BodySrcManager.GetComponent<BodySourceManager>();
      }
   }

   // Update is called once per frame
   void Update()
   {

      if (BodySrcManager == null)
      {
         return;
      }

      bodies = bodyManager.GetData();
      if (bodies == null)
      {
         return;
      }
      foreach (var body in bodies)
      {
         if (body == null)
         {
            continue;
         }
         if (body.IsTracked)
         {
            var hipZ = body.Joints[JointType.HipLeft].Position.Z;
            var pos = body.Joints[TrackedJoint].Position;
            // Aplica o filtro de Kalman em cada eixo
            float filteredX = kalmanX.Update(pos.X);
            float filteredY = kalmanY.Update(pos.Y);
            float filteredZ = kalmanZ.Update(pos.Z);
            Vector3 unityPos = new Vector3(filteredX, filteredY, -filteredZ) * multiplier;
            gameObject.transform.position = unityPos;

            // --- Detecta arremesso em Z relativo ao baseTransform ---
            float baseZ = baseTransform != null ? baseTransform.position.z : 0f;
            float relativeFilteredZ = filteredZ - baseZ;
            float deltaZ = relativeFilteredZ - lastFilteredZ;
            float velocityZ = deltaZ / Time.deltaTime;
            if (!hasThrown && velocityZ > throwThreshold)
            {
               // Debug.Log("Arremesso!");
               hasThrown = true;
            }

            if (hasThrown && Mathf.Abs(hipZ - pos.Z) > throwResetThreshold)
            {
               hasThrown = false;
            }

            lastFilteredZ = relativeFilteredZ;

            // DebugLayer.Instance.field_bodyHipZ.text = "Body Hip Z: " + body.Joints[JointType.HipLeft].Position.Z.ToString("F2");
            // DebugLayer.Instance.field_rightHandZ.text = "Right Hand Z: " + body.Joints[JointType.HandRight].Position.Z.ToString("F2");
            // DebugLayer.Instance.field_distance.text = "Distância: " + Mathf.Abs(hipZ - pos.Z).ToString("F2");
            // DebugLayer.Instance.field_trown.text = hasThrown ? "Arremessou" : "IDLE";
            // DebugLayer.Instance.field_trown.color = hasThrown ? Color.green : Color.yellow;
            // --- Fim detecção arremesso ---

            // var rot = body.JointOrientations[TrackedJoint].Orientation;
            // // Conversão correta do quaternion do Kinect para o Unity
            // // Kinect: (X, Y, Z, W) -> Unity: (X, Y, -Z, -W)
            // Quaternion raw = new Quaternion(rot.X, rot.Y, -rot.Z, -rot.W);
            // // Aplica filtro de Kalman em cada componente do quaternion
            // float filteredRotX = kalmanRotX.Update(raw.x);
            // float filteredRotY = kalmanRotY.Update(raw.y);
            // float filteredRotZ = kalmanRotZ.Update(raw.z);
            // float filteredRotW = kalmanRotW.Update(raw.w);
            // Quaternion filtered = new Quaternion(filteredRotX, filteredRotY, filteredRotZ, filteredRotW);
            // filtered.Normalize();
            // // Ajuste de orientação extra (180° em Y pode ser necessário, mas pode testar 90° ou 0°)
            // Quaternion y180 = Quaternion.Euler(0, 180, 0);
            // Quaternion finalRot = y180 * filtered;
            // // Se ainda estiver errado, tente inverter outros eixos:
            // // Quaternion finalRot = y180 * new Quaternion(-filtered.x, filtered.y, filtered.z, -filtered.w);
            // // Ou troque a ordem: Quaternion finalRot = filtered * y180;
            // gameObject.transform.rotation = finalRot;
         }
      }
   }

   
}
