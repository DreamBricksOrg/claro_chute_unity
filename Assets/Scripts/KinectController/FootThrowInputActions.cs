using UnityEngine;
using UnityEngine.InputSystem;

public class FootThrowInputActions : MonoBehaviour, InputControls.IGameActions
{
    public ThrowDetector handThrowDetector;
    private InputControls inputControls;

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
        // if(handThrowDetector.currentBallInstance == null || handThrowDetector == null) return;
        // if (context.performed && handThrowDetector != null)
        // {
        //     Vector3 direction = handThrowDetector.transform.forward;
        //     Quaternion upRotation = Quaternion.AngleAxis(-20f, Vector3.right);
        //     Vector3 adjustedDirection = upRotation * direction;
        //     handThrowDetector.ShootBall(adjustedDirection, handThrowDetector.throwVelocityThreshold);
        //     Invoke(nameof(ResetBall), 1f);
        // }
    }

    void ResetBall()
    {
        // if (handThrowDetector != null)
        // {
        //     handThrowDetector.GenerateBall();
        // }
    }

    public void OnPrintScreen(InputAction.CallbackContext context)
    {
    }

    public void OnDeveloperMode(InputAction.CallbackContext context)
    {
    }
}
