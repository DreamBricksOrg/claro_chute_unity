using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Schedule : UI
{
    public TMP_Text fieldSchedule;

	private DateTime timeStart;
	private DateTime timeStop;
	private bool isInScheduleTime = false;

	internal override void Start()
	{
		base.Start();
		var infoData = Config.Instance.configData["info"];

		timeStart = DateTime.ParseExact(infoData["scheduleStart"].Value, "HH:mm", CultureInfo.InvariantCulture);
		timeStop = DateTime.ParseExact(infoData["scheduleStop"].Value, "HH:mm", CultureInfo.InvariantCulture);

		fieldSchedule.SetText(infoData["scheduleStart"].Value + " às " + infoData["scheduleStop"].Value);
		
		InvokeRepeating(nameof(CheckSystemTime), 0f, 30f);
	}

	void CheckSystemTime()
	{
		DateTime currentTime = DateTime.Now;
		TimeSpan currentTimeOfDay = currentTime.TimeOfDay;
		TimeSpan startTimeOfDay = timeStart.TimeOfDay;
		TimeSpan stopTimeOfDay = timeStop.TimeOfDay;

		bool isNowInScheduleTime = currentTimeOfDay >= startTimeOfDay && currentTimeOfDay <= stopTimeOfDay;

		if (isNowInScheduleTime && !isInScheduleTime)
		{

			isInScheduleTime = true;
			EventManager.CoreSystem.SetState(true);
			EventManager.Section.SetSection(SectionTypes.Intro);
		}
		else if (!isNowInScheduleTime && isInScheduleTime)
		{
			isInScheduleTime = false;
			EventManager.CoreSystem.SetState(false);
			EventManager.Section.SetSection(SectionTypes.Schedule);
		}
	}

}
