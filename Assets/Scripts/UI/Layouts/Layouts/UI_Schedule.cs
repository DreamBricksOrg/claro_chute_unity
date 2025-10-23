using System;
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
		fieldSchedule.text =  infoData["schedule"].Value;
	}

}
