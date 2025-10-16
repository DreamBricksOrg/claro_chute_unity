using System;
using System.Collections;
using UnityEngine;

public class UI_DeveloperMode : UI
{

	internal override void OnShow(object data, Action<object> callback)
	{
		base.OnShow(data, callback);
	}

	internal override void OnHide(object data, Action<object> callback)
	{
		base.OnHide(data, callback);

	}

	// IEnumerator Countdown(int value)
	// {
	// 	yield return new WaitForSeconds(1f);
	// 	for (int i = value; i >= 0; i--)
	// 	{
	// 		animationModuleCounter.In();
	// 		fieldCounter.text = i.ToString();
	// 		animationModuleCounterGhost.Out();
	// 		AudioController.PlaySFX(i > 0 ? AudioTypes.SFX_countdown : AudioTypes.SFX_countdownEnd);
	// 		yield return new WaitForSeconds(1f);
			
			
	// 		animationModuleCounter.Out();
	// 		animationModuleCounterGhost.In();
	// 		fieldCounterGhost.text = i.ToString();
	// 	}
	// 	animationModuleCounterGhost.Out();
	// 	EventManager.Section.SetSection(SectionTypes.Game);
	// }

}
