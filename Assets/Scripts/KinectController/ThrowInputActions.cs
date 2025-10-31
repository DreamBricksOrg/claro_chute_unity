using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowInputActions : MonoBehaviour, InputControls.IGameActions
{
    public ThrowDetector throwDetectorComponent;
    private InputControls inputControls;
    public float power = 5f;
    public float yAxisSensitivity = 1f;

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
        if(throwDetectorComponent == null) return;
        if (context.performed && throwDetectorComponent != null)
        {
            var direction = throwDetectorComponent.transform.forward;
            var upRotation = Quaternion.AngleAxis(-30 * yAxisSensitivity, Vector3.right);
            var adjustedDirection = upRotation * direction;
            throwDetectorComponent.ballController.ShootBall(adjustedDirection, Random.Range(power, power + 10f));
        }
    }

    public void OnPrintScreen(InputAction.CallbackContext context)
    {
    }

    public void OnDeveloperMode(InputAction.CallbackContext context)
    {
    }
}
