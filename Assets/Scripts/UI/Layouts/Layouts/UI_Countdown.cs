using System;
using System.Collections;
using Kasulo.Animations.UI;
using TMPro;
using UnityEngine;

public class UI_Countdown : UI
{
	public TMP_Text fieldCounter;
	public TMP_Text fieldCounterGhost;

	internal override void OnShow(object data, Action<object> callback)
	{
		base.OnShow(data, callback);
	}

	internal override void OnHide(object data, Action<object> callback)
	{
		base.OnHide(data, callback);

	}

	IEnumerator Countdown(int value)
	{
		yield return new WaitForSeconds(1f);
		for (int i = value; i >= 0; i--)
		{
			fieldCounter.text = i.ToString();
			AudioController.PlaySFX(i > 0 ? AudioTypes.SFX_countdown : AudioTypes.SFX_countdownEnd);
			yield return new WaitForSeconds(1f);
			fieldCounterGhost.text = i.ToString();
		}
		EventManager.Section.SetSection(SectionTypes.Game);
	}

}
