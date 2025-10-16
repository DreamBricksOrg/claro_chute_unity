using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var hit = other.GetComponent<ProjectileBall>();
        if (hit != null && hit.isActive)
        {
            AudioController.PlaySFX(AudioTypes.SFX_goal);
            hit.Deactivate();
        }
    }
}
