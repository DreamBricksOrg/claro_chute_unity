using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Widget : MonoBehaviour
{
    public WidgetTypes type = WidgetTypes.None;

    private CanvasGroup canvasComponent;
    public float animDelay = 0f;
    
    protected bool isActive = false;
    public bool isPersistent = false;

    // -----------------------------------------
    // [UI] DELEGATES / EVENTS
    // -----------------------------------------
    public delegate void _delegateUIShow(WidgetTypes type, object data = null, Action<object> callback = null);
    public static event _delegateUIShow ShowEvent;
    public static void Show(WidgetTypes type, object data = null, Action<object> callback = null)
    {
        ShowEvent?.Invoke(type, data, callback);
    }

    public delegate void _delegateUIHide(WidgetTypes type, object data = null, Action<object> callback = null);
    public static event _delegateUIHide HideEvent;
    public static void Hide(WidgetTypes type, object data = null, Action<object> callback = null)
    {
        HideEvent?.Invoke(type, data, callback);
    }

    public delegate void _delegateUIHideAll();
    public static event _delegateUIHideAll HideAllEvent;
    public static void HideAll() => HideAllEvent?.Invoke();


    void Setup()
    {
        canvasComponent = GetComponent<CanvasGroup>();
        if (canvasComponent == null) canvasComponent = gameObject.AddComponent<CanvasGroup>();
        if (canvasComponent != null) canvasComponent.blocksRaycasts = false;
    }

    void Prepare()
    {
        Hide(type);
    }

    internal virtual void Awake()
    {
        Setup();
        ShowEvent += OnReceiveShow;
        HideEvent += OnReceiveHide;
        HideAllEvent += OnHideAll;
    }

    internal virtual void OnDestroy()
    {
        ShowEvent -= OnReceiveShow;
        HideEvent -= OnReceiveHide;
        HideAllEvent -= OnHideAll;
    }

    private void OnHideAll()
    {
        if(isPersistent) return;
        Hide();
    }

    internal virtual void Start()
    {
        Prepare();
    }

    protected void Show()
    {
        Show(type);
    }

    protected void Hide()
    {
        Hide(type);
    }

    internal void OnReceiveShow(WidgetTypes type, object data, Action<object> callback)
    {
        if (this.type != type) return;
        OnShow(data, callback);
    }

    internal void OnReceiveHide(WidgetTypes type, object data, Action<object> callback)
    {
        if (this.type != type) return;
        OnHide(data, callback);
    }

    internal virtual void StartAnimationIn()
    {
        // if (animationModule)
        // {
        //     Utils.DelayAction(animDelay, delegate
        //     {
        //         animationModule.SetState(UIAnimationModule.WidgetState.Show);
        //     });
        // }
    }
    internal virtual void StartAnimationOut()
    {
        // if (animationModule)
        // {
        //     animationModule.SetState(UIAnimationModule.WidgetState.Hide);
        // }
    }

    internal virtual void OnShow(object data, Action<object> callback)
    {
        isActive = true;
        StartAnimationIn();
        Fade(true);
    }
    internal virtual void OnHide(object data, Action<object> callback)
    {
        isActive = false;
        StartAnimationOut();
        Fade(false);
    }

    void Fade(bool flag)
    {
        if (canvasComponent != null)
        {
            canvasComponent.blocksRaycasts = flag;
            canvasComponent.alpha = flag ? 1f : 0f;
        }
    }

    internal virtual void OnButtonTrigger(PlayerInput playerInput, InputAction.CallbackContext context)
    {
        if (!isActive) return;
        InputPerformed(context.action.name);
    }

    public virtual void InputPerformed(string inputName) { }
}

public enum WidgetTypes
{
    None = -1,
    Graphic = 10,
    ParticleFX = 20,
}