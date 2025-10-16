using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalKeeperController : MonoBehaviour
{
    public Animator animator;

    public float turnValue = 1f;
    public float heightValue = 1f;

    void Start()
    {
        StartCoroutine(nameof(Action));
    }

    IEnumerator Action()
    {
        while (true)
        {
            // RigidBody.MovePosition(transform.position + MovementDirection.normalized * CurrentSpeed * Time.deltaTime);
            yield return new WaitForSeconds(4f);
            animator.SetFloat("Turn", Random.Range(-1f, 1f) * turnValue);
            animator.SetFloat("Height", Random.Range(0f, 1f) * heightValue);
            animator.SetTrigger("Dive");
        }
    }



}
