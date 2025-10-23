using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DevBallShoot : MonoBehaviour, InputControls.IGameActions
{
    private InputControls inputControls;
    public BallController ballController;

    private void Awake()
    {
        inputControls = new InputControls();
    }

    private void OnEnable()
    {
        inputControls.Game.SetCallbacks(this);
        inputControls.Game.Enable();
    }

    private void OnDisable()
    {
        inputControls.Game.Disable();
        inputControls.Game.SetCallbacks(null);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var direction = transform.forward;
            var upRotation = Quaternion.AngleAxis(-20f, Vector3.right);
            var adjustedDirection = upRotation * direction;
            ballController.ShootBall(adjustedDirection, 2f);
        }
    }

    public void OnPrintScreen(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
           ScreenCapture.CaptureScreenshot("screenshot.png", 2);
        }
    }
}
