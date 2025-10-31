using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DevBallShoot : MonoBehaviour
{
    public BallController ballController;
    private InputAction fireAction;

    private void Start()
    {
        var gameActions = Main.Instance.InputActions.FindActionMap("Game");
        fireAction = gameActions.FindAction("Fire");
        fireAction.performed += OnFirePerformed;
    }

    private void OnDestroy()
    {
        CleanupInputs();
    }

    private void OnApplicationQuit()
    {
        CleanupInputs();
    }

    private void CleanupInputs()
    {
        if (fireAction != null)
        {
            fireAction.performed -= OnFirePerformed;
            fireAction = null;
        }
    }

    private void OnFirePerformed(InputAction.CallbackContext context)
    {
        Debug.Log("SHOOT BALL");
        var direction = transform.forward;
        var upRotation = Quaternion.AngleAxis(-20f, Vector3.right);
        var adjustedDirection = upRotation * direction;
        ballController.ShootBall(adjustedDirection, 2f);
    }
}
