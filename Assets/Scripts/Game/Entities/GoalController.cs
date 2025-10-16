using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    public Cloth cloth;

    void OnEnable()
    {
        EventManager.Game.OnBallCreatedEvent += OnBallCreated;
    }

    void OnDisable()
    {
        EventManager.Game.OnBallCreatedEvent -= OnBallCreated;
    }

    private void OnBallCreated(GameObject ball)
    {
        var ballComponent = ball.GetComponent<ProjectileBall>();
        var sphere = ballComponent.colliderComponent as SphereCollider;
        var colliders = cloth.sphereColliders;
        var pair = new ClothSphereColliderPair(sphere, sphere);
        colliders[0] = pair;
        cloth.sphereColliders = colliders;
    }
}
