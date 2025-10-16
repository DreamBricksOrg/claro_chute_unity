using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Element : MonoBehaviour
{
    public ElementTypes type = ElementTypes.None;

    private CanvasGroup canvasComponent;
    public float animDelay = 0f;
    
    protected bool isActive = false;
    public bool isPersistent = false;
    protected bool isCached = false;

    // -----------------------------------------
    // [UI] DELEGATES / EVENTS
    // -----------------------------------------
    public delegate void _delegateUIShow(ElementTypes type, object data = null, Action<object> callback = null);
    public static event _delegateUIShow ShowEvent;
    public static void Show(ElementTypes type, object data = null, Action<object> callback = null)
    {
        ShowEvent?.Invoke(type, data, callback);
    }

    public delegate void _delegateUIHide(ElementTypes type, object data = null, Action<object> callback = null);
    public static event _delegateUIHide HideEvent;
    public static void Hide(ElementTypes type, object data = null, Action<object> callback = null)
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

    private void OnClearChange()
    {
        isCached = false;
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

    internal void OnReceiveShow(ElementTypes type, object data, Action<object> callback)
    {
        if (this.type != type) return;
        OnShow(data, callback);
    }

    internal void OnReceiveHide(ElementTypes type, object data, Action<object> callback)
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

    public virtual void LoadAssets() { isCached = true; }

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

public enum ElementTypes
{
    None = -1,
    Graphic = 10,
    ParticleFX = 20,
}