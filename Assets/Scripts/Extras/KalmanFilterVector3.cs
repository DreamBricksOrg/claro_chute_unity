using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 3x independent 1D Kalman filters for a Vector3 with constant-state model.
/// </summary>
public class KalmanFilterVector3
{
    public const float DEFAULT_Q = 1e-6f;  // process noise variance
    public const float DEFAULT_R = 1e-2f;  // measurement noise variance
    public const float DEFAULT_P = 1f;     // initial estimate variance

    private float q;
    private float r;

    // Per-axis covariance and gain
    private Vector3 p = new Vector3(DEFAULT_P, DEFAULT_P, DEFAULT_P);
    private Vector3 k = Vector3.zero;

    // State
    private Vector3 x = Vector3.zero;
    private bool initialized = false;

    public KalmanFilterVector3() : this(DEFAULT_Q, DEFAULT_R) { }
    public KalmanFilterVector3(float aQ = DEFAULT_Q, float aR = DEFAULT_R)
    {
        q = aQ;
        r = aR;
    }

    /// <summary>Single measurement update. Optionally change Q/R on the fly.</summary>
    public Vector3 Update(Vector3 z, float? newQ = null, float? newR = null)
    {
        if (newQ.HasValue && q != newQ.Value) q = newQ.Value;
        if (newR.HasValue && r != newR.Value) r = newR.Value;

        if (!initialized)
        {
            x = z;
            p = new Vector3(DEFAULT_P, DEFAULT_P, DEFAULT_P);
            initialized = true;
            return x;
        }

        // Predict (constant state): p = p + q
        Vector3 pPred = p + new Vector3(q, q, q);

        // Kalman gain: k = pPred / (pPred + r)
        Vector3 denom = pPred + new Vector3(r, r, r);
        k = new Vector3(pPred.x / denom.x, pPred.y / denom.y, pPred.z / denom.z);

        // Update state: x = x + k * (z - x)
        Vector3 residual = z - x;
        x = x + new Vector3(k.x * residual.x, k.y * residual.y, k.z * residual.z);

        // Update covariance: p = (1 - k) * pPred
        p = new Vector3(
            (1f - k.x) * pPred.x,
            (1f - k.y) * pPred.y,
            (1f - k.z) * pPred.z);

        return x;
    }

    /// <summary>Batch update. If newestFirst=true, consumes in reverse order.</summary>
    public Vector3 Update(List<Vector3> measurements, bool areMeasurementsNewestFirst = false,
                          float? newQ = null, float? newR = null)
    {
        if (measurements == null || measurements.Count == 0) return x;

        if (areMeasurementsNewestFirst)
        {
            for (int i = measurements.Count - 1; i >= 0; --i)
                Update(measurements[i], newQ, newR);
        }
        else
        {
            for (int i = 0; i < measurements.Count; ++i)
                Update(measurements[i], newQ, newR);
        }

        return x;
    }

    public void Reset(Vector3? newState = null)
    {
        x = newState ?? Vector3.zero;
        p = new Vector3(DEFAULT_P, DEFAULT_P, DEFAULT_P);
        k = Vector3.zero;
        initialized = newState.HasValue; // if we got a state, we're initialized
    }

    public Vector3 State => x;
}
