using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugLayer : MonoBehaviour
{
    public static DebugLayer Instance;
    public TMP_Text field_bodyHipZ;
    public TMP_Text field_rightHandZ;
    public TMP_Text field_trown;
    public TMP_Text field_distance;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        
    }
}
