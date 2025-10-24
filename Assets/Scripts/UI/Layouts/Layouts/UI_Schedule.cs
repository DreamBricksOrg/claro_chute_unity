using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Schedule : UI
{
    public TMP_Text fieldSchedule;

	internal override void Start()
	{
		base.Start();
		var infoData = Config.Instance.configData["info"];
		fieldSchedule.SetText(infoData["scheduleStart"].Value + " às " + infoData["scheduleStop"].Value);
		fieldSchedule.SetText(infoData["scheduleStart"].Value + " às " + infoData["scheduleStop"].Value);
	}
}
