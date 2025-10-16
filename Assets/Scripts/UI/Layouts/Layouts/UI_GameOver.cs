using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameOver : UI
{
    [Header("Components")]
    public TMP_Text fieldGameScore;
    public Button btnConfirm;

    internal override void Awake()
    {
        base.Awake();
        btnConfirm.onClick.AddListener(() =>
        {
            // AudioController.PlaySFX(AudioTypes.SFX_uiClick);
            EventManager.Section.SetSection(SectionTypes.HowToPlay);
        });
    }
    
}
