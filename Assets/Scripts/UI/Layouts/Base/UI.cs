using Kasulo.Animations.UI;
using SimpleJSON;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CanvasGroup))]
public class UI : MonoBehaviour
{
    protected UIAnimationModule animationModule;
    private CanvasGroup canvasComponent;

    [Header("Screen Label")]
    public UITypes type = UITypes.None;
    [Header("Animation Setup")]
    public float uiAnimationDelay = 0f;

    internal bool autoAnimate = true;

    protected bool isActive = true;
    public bool isPersistent = false;

    // -----------------------------------------
    // [UI] DELEGATES / EVENTS
    // -----------------------------------------

    public delegate void _delegateUIShow(UITypes uiType, object data = null, Action<object> callback = null);
    public static event _delegateUIShow ShowEvent;
    public static void Show(UITypes uiType, object data = null, Action<object> callback = null)
    {
        ShowEvent?.Invoke(uiType, data, callback);
    }

    public delegate void _delegateUIHide(UITypes uiType, object data = null, Action<object> callback = null);
    public static event _delegateUIHide HideEvent;
    public static void Hide(UITypes uiType, object data = null, Action<object> callback = null)
    {
        HideEvent?.Invoke(uiType, data, callback);
    }

    public delegate void _delegateUIHideAll();
    public static event _delegateUIHideAll HideAllEvent;
    public static void HideAll() => HideAllEvent?.Invoke();


    void Setup()
    {
        animationModule = GetComponent<UIAnimationModule>();
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
        if (isPersistent) return;
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

    internal void OnReceiveShow(UITypes uiType, object data, Action<object> callback)
    {
        if (type != uiType) return;
        if (isActive) return;
        OnShow(data, callback);
    }

    internal void OnReceiveHide(UITypes uiType, object data, Action<object> callback)
    {
        if (type != uiType) return;
        if (!isActive) return;
        OnHide(data, callback);
    }

    internal virtual void StartAnimationIn()
    {
        if (animationModule)
        {
            Utils.DelayAction(uiAnimationDelay, delegate
            {
                animationModule.SetState(UIAnimationModule.WidgetState.Show);
            });
        }

        // if (animatorComponent)
        // {
        //     animatorComponent.ResetTrigger("Out");
        //     animatorComponent.SetTrigger("In");
        // }

    }

    internal virtual void StartAnimationOut()
    {
        if (animationModule)
        {
            animationModule.SetState(UIAnimationModule.WidgetState.Hide);
        }
        // if (animatorComponent)
        // {
        //     animatorComponent.ResetTrigger("In");
        //     animatorComponent.SetTrigger("Out");
        // }
    }

    internal virtual void OnShow(object data, Action<object> callback)
    {
        isActive = true;
        if (autoAnimate) StartAnimationIn();
        Fade(true);
    }
    internal virtual void OnHide(object data, Action<object> callback)
    {
        isActive = false;
        if (autoAnimate) StartAnimationOut();
        Fade(false);
    }

    void Fade(bool flag)
    {
        if (canvasComponent != null)
        {
            canvasComponent.blocksRaycasts = flag;
            if (!animationModule)
            {
                canvasComponent.alpha = flag ? 1f : 0f;
            }
        }
    }

    // internal virtual void OnButtonTrigger(PlayerInput playerInput, InputAction.CallbackContext context)
    // {
    //     if (!isActive) return;
    //     InputPerformed(context.action.name);
    // }

    // public virtual void InputPerformed(string inputName) { }

}

public enum UITypes
{
    None = -1,

    Intro = 10,
    Ranking = 11,
    HowToPlay = 20,
    Countdown = 30,
    GameHUD = 40,
    FinalScore = 70,
    QRCode = 71,
    Gameover = 80,
    Replay = 90,
    Schedule = 100,

    DeveloperMode = 999,
}
