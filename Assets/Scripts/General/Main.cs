using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Threading;
using System;
using System.Globalization;
using UnityEngine.InputSystem;

public class Main : MonoBehaviour
{
    public static Main Instance;
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActionAsset;
    public InputActionAsset InputActions => inputActionAsset;
    CancellationTokenSource cts;
    int scheduleState = -1;
    Coroutine scheduleCoroutine;

    [Header("Section")]
    public SectionTypes beginSectionType = SectionTypes.Intro;

    void OnEnable()
    {
        Application.lowMemory += OnLowMemory;
        EventManager.Section.OnSectionEvent += OnSection;
        EventManager.CoreSystem.OnCoreSystemStateEvent += OnCoreSystemState;
        
        var gameActions = inputActionAsset.FindActionMap("Game");
        gameActions.Enable();
    }

    void OnDisable()
    {
        CleanupEvents();
        DisableInputs();
    }

    void OnApplicationQuit()
    {
        CleanupEvents();
        DisableInputs();
    }

    private void CleanupEvents()
    {
        Application.lowMemory -= OnLowMemory;
        EventManager.Section.OnSectionEvent -= OnSection;
        EventManager.CoreSystem.OnCoreSystemStateEvent -= OnCoreSystemState;
    }

    private void DisableInputs()
    {
        var gameActions = inputActionAsset.FindActionMap("Game");
        gameActions.Disable();
    }

    private void OnCoreSystemState(bool isCoreSystemWorking)
    {
        scheduleState = isCoreSystemWorking ? 1 : 0;
        Utils.CancelDelay(cts);
        if (isCoreSystemWorking)
        {
            Debug.Log("SYSTEM ONLINE >>> GO TO INTRO");
            LogManager.SendLog("TOTEM_INICIO");
            EventManager.Section.SetSection(SectionTypes.Intro);
        }
        else
        {
            Debug.Log("SYSTEM OFFLINE >>> GO TO SCHEDULE");
            LogManager.SendLog("TOTEM_FIM");
            EventManager.Section.SetSection(SectionTypes.Schedule);
        }
    }

    void Awake()
    {
        Instance = this;
        DOTween.SetTweensCapacity(5000, 20);
        LogManager.Init();

        // Validate Input Action Asset
        if (inputActionAsset == null)
        {
            Debug.LogError("Input Action Asset not assigned in Main component. Please assign it in the Inspector.");
        }
    }

    IEnumerator Start()
    {
        yield return null;
        if (Application.isEditor)
        {
            EventManager.Section.SetSection(beginSectionType);
        }
        else
        {
            EventManager.Section.SetSection(SectionTypes.Intro);
        }

        if (scheduleCoroutine != null) StopCoroutine(scheduleCoroutine);
        scheduleCoroutine = StartCoroutine(CheckSystemTimeCoroutine());

        LogManager.SendLog("TOTEM_START");
    }

    private void OnLowMemory()
    {
        ClearUnusedTextures();
        Debug.Log("LOW MEMORY >>> CLEAR TEXTURES");
    }

    void ClearUnusedTextures()
    {
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }

    private void OnSection(SectionTypes sectionType)
    {
        var timeoutArray = Config.Instance.configData["timeout"];
        UI.HideAll();
        Utils.CancelDelay(cts);
        switch (sectionType)
        {
            case SectionTypes.Intro:
                UI.Show(UITypes.Intro);
                cts = Utils.DelayActionCancelable(timeoutArray["intro"].AsInt, () =>
                {
                    EventManager.Section.SetSection(SectionTypes.Ranking);
                });
                CheckSystemSchedule();
                break;
            case SectionTypes.Ranking:
                UI.Show(UITypes.Ranking);
                cts = Utils.DelayActionCancelable(timeoutArray["ranking"].AsInt, () =>
                {
                    EventManager.Section.SetSection(SectionTypes.HowToPlay);
                });
                break;
            case SectionTypes.HowToPlay:
                UI.Show(UITypes.HowToPlay);
                cts = Utils.DelayActionCancelable(timeoutArray["howtoplay"].AsInt, () =>
                {
                    EventManager.Section.SetSection(SectionTypes.Intro);
                });
                break;
            case SectionTypes.Game:
                LogManager.SendLog("TOTEM_JOGO");
                UI.Show(UITypes.GameHUD);
                EventManager.Game.GameStart();
                break;
            case SectionTypes.FinalScore:
                LogManager.SendLog("TOTEM_PONTUACAO_FINAL");
                UI.Show(UITypes.FinalScore);
                cts = Utils.DelayActionCancelable(timeoutArray["finalscore"].AsInt, () =>
                {
                    EventManager.Section.SetSection(SectionTypes.QRCode);
                });
                break;
            case SectionTypes.QRCode:
                LogManager.SendLog("TOTEM_QRCODE");
                UI.Show(UITypes.QRCode);
                break;
            case SectionTypes.Replay:
                LogManager.SendLog("TOTEM_REPLAY");
                UI.Show(UITypes.Replay);
                break;
            case SectionTypes.Terms:
                LogManager.SendLog("TOTEM_TERMS");
                UI.Show(UITypes.Terms);
                break;
            case SectionTypes.Gameover:
                LogManager.SendLog("TOTEM_FIM_DE_JOGO");
                UI.Show(UITypes.Gameover);
                cts = Utils.DelayActionCancelable(timeoutArray["gameover"].AsInt, () =>
                {
                    EventManager.Section.SetSection(SectionTypes.Intro);
                });
                break;
            case SectionTypes.Schedule:
                LogManager.SendLog("TOTEM_AGENDA");
                UI.Show(UITypes.Schedule);
                break;
        }
    }

    void CheckSystemSchedule()
    {
        var infoData = Config.Instance.configData["info"];
        var timeStart = DateTime.ParseExact(infoData["scheduleStart"].Value, "HH:mm", CultureInfo.InvariantCulture);
        var timeStop = DateTime.ParseExact(infoData["scheduleStop"].Value, "HH:mm", CultureInfo.InvariantCulture);

        DateTime currentTime = DateTime.Now;
        TimeSpan currentTimeOfDay = currentTime.TimeOfDay;
        TimeSpan startTimeOfDay = timeStart.TimeOfDay;
        TimeSpan stopTimeOfDay = timeStop.TimeOfDay;

        bool isNowInScheduleTime = currentTimeOfDay >= startTimeOfDay && currentTimeOfDay <= stopTimeOfDay;

        if (isNowInScheduleTime && scheduleState <= 0)
        {
            EventManager.CoreSystem.SetState(true);
        }
        else if (!isNowInScheduleTime && (scheduleState == -1 || scheduleState == 1))
        {
            EventManager.CoreSystem.SetState(false);
        }
    }

    IEnumerator CheckSystemTimeCoroutine()
    {
        while (true)
        {
            if (scheduleState != 1) CheckSystemSchedule();
            yield return new WaitForSeconds(2f);
        }
    }   
    

}