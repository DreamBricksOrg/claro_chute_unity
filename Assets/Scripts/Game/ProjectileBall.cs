using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBall : MonoBehaviour
{
    public TrailRenderer trailRenderer;
    public float ballSpeed = 15f;
    public Rigidbody rb;
    public Collider colliderComponent;
    public bool isActive = false;

    void Awake()
    {
        trailRenderer.enabled = false;
        trailRenderer.Clear();
        colliderComponent.enabled = false;
        rb.isKinematic = true;

        Utils.DelayAction(0.2f, () =>
        {
            AudioController.PlaySFX(AudioTypes.SFX_ballPlaced);
        });
    }

    public void Play(Vector3 direction, float velocity = 1f)
    {
        AudioController.PlaySFX(AudioTypes.SFX_ballShoot);
        EventManager.Game.ShootSpeed(velocity * Config.Instance.configData["game"]["uiSpeedMultiplier"].AsFloat);
        Destroy(gameObject, Config.Instance.configData["game"]["ballLifespan"].AsInt);
        isActive = true;
        colliderComponent.enabled = true;
        trailRenderer.enabled = true;
        rb.velocity = direction.normalized * (velocity * ballSpeed);
        rb.angularDrag = 0.05f;
        rb.isKinematic = false;
        rb.AddTorque(transform.right * 0.04f, ForceMode.Impulse);
    }

    void OnDestroy()
    {
        EventManager.Game.ShootResult(!isActive, isActive ? "Missed" : "Goal");
    }

    public void Deactivate()
    {
        isActive = false;
    }

    void OnCollisionEnter(Collision hit)
    {
        AudioController.PlaySFX(AudioTypes.SFX_ballHit);
    }
}
