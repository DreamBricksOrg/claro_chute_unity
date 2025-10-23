using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Threading;

public class Main : MonoBehaviour
{
    public static Main Instance;
    CancellationTokenSource cts;
    
    [Header("Section")]
    public SectionTypes beginSectionType = SectionTypes.Intro;

    void OnEnable()
    {
        Application.lowMemory += OnLowMemory;
        EventManager.Section.OnSectionEvent += OnSection;
    }

    void OnDisable()
    {
        Application.lowMemory -= OnLowMemory;
        EventManager.Section.OnSectionEvent -= OnSection;
    }

    void Awake()
    {
        Instance = this;
        DOTween.SetTweensCapacity(5000, 20);
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
        switch (sectionType)
        {
            case SectionTypes.Intro:
                UI.Show(UITypes.Intro);
                cts = Utils.DelayActionCancelable(timeoutArray["intro"].AsInt, () =>
                {
                    EventManager.Section.SetSection(SectionTypes.Ranking);
                });
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
                Utils.CancelDelay(cts);
                UI.Show(UITypes.GameHUD);
                EventManager.Game.GameStart();
                break;
            case SectionTypes.FinalScore:
                UI.Show(UITypes.FinalScore);
                Utils.DelayAction(timeoutArray["finalscore"].AsInt, () =>
                {
                    EventManager.Section.SetSection(SectionTypes.QRCode);
                });
                break;
            case SectionTypes.QRCode:
                UI.Show(UITypes.QRCode);
                Utils.DelayAction(timeoutArray["qrcode"].AsInt, () =>
                {
                    EventManager.Section.SetSection(SectionTypes.Replay);
                });
                break;
            case SectionTypes.Replay:
                UI.Show(UITypes.Replay);
                Utils.DelayAction(timeoutArray["replay"].AsInt, () =>
                {
                    EventManager.Section.SetSection(SectionTypes.Gameover);
                });
                break;
            case SectionTypes.Gameover:
                UI.Show(UITypes.Gameover);
                Utils.DelayAction(timeoutArray["gameover"].AsInt, () =>
                {
                    EventManager.Section.SetSection(SectionTypes.Schedule);
                });
                break;
            case SectionTypes.Schedule:
                UI.Show(UITypes.Schedule);
                Utils.DelayAction(timeoutArray["schedule"].AsInt, () =>
                {
                    EventManager.Section.SetSection(SectionTypes.Intro);
                });
                break;
        }
    }

}