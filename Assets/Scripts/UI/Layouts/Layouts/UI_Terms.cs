using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_Terms : UI
{
	public Button btnConfirm;

	internal override void Awake()
	{
		base.Awake();
		btnConfirm.onClick.AddListener(() =>
		{
			// AudioController.PlaySFX(AudioTypes.SFX_uiClick);
			EventManager.Section.SetSection(SectionTypes.Game);
		});
	}

	internal override void OnShow(object data, Action<object> callback)
	{
		base.OnShow(data, callback);
		btnConfirm.enabled = false;
		Invoke(nameof(EnableButton), 2f);
	}

    internal override void OnHide(object data, Action<object> callback)
    {
        base.OnHide(data, callback);
		btnConfirm.enabled = false;
    }

    void EnableButton()
	{
		btnConfirm.enabled = true;
    }
}
