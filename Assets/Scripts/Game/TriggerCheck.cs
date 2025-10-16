using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCheck : MonoBehaviour
{
    public Transform root;
    
    public void OnHit()
    {
        root.SendMessage("OnHit", SendMessageOptions.DontRequireReceiver);
    }
}
