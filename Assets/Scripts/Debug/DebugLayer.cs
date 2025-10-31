using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugLayer : MonoBehaviour
{
    public CanvasGroup debugCanvasGroup;
    private bool isDeveloperMode = false;
    private InputAction developerModeAction;

    void Awake()
    {
        debugCanvasGroup.alpha = 0;
        debugCanvasGroup.interactable = false;
    }

    private void Start()
    {
        var gameActions = Main.Instance.InputActions.FindActionMap("Game");
        developerModeAction = gameActions.FindAction("DeveloperMode");
        developerModeAction.performed += OnDeveloperModePerformed;
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
        if (developerModeAction != null)
        {
            developerModeAction.performed -= OnDeveloperModePerformed;
            developerModeAction = null;
        }
    }

    private void OnDeveloperModePerformed(InputAction.CallbackContext context)
    {
        Debug.Log("DEV");
        Debug.Log("Developer Mode: " + (isDeveloperMode ? "ON" : "OFF"));
        debugCanvasGroup.alpha = isDeveloperMode ? 1 : 0;
        debugCanvasGroup.interactable = isDeveloperMode;
        isDeveloperMode = !isDeveloperMode;
    }
}
