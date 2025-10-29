using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_HowToPlay : UI
{
	public Button btnConfirm;

	internal override void Awake()
	{
		base.Awake();
		btnConfirm.onClick.AddListener(() =>
		{
			// AudioController.PlaySFX(AudioTypes.SFX_uiClick);
			EventManager.Section.SetSection(SectionTypes.Terms);
		});
	}

}
