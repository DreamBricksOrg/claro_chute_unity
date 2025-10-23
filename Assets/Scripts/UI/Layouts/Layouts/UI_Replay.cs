using System;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UI_Replay : UI
{
	public TMP_Text fieldScore;
	public TMP_Text fieldRanking;
	public VideoPlayer videoPlayer;

    internal override void OnShow(object data, Action<object> callback)
    {
        base.OnShow(data, callback);

        fieldScore.SetText("");
        fieldRanking.SetText("");

		API.Request("/api/playerdata/" + GameRoundController.Instance.playerId,
			onSuccess: (result) =>
			{
				var jsonData = JSON.Parse(result);
				fieldScore.SetText(jsonData["player"]["score"].Value + "<size=40%>km/h</size>");
				fieldRanking.SetText(jsonData["player"]["position"].Value + "º");
			},
			onError: (error) =>
			{
				Debug.LogError("API Error: " + error);
			}
		);

		videoPlayer.url = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
		videoPlayer.loopPointReached += OnVideoEnd;
		videoPlayer.Play();
    }

    private void OnVideoEnd(VideoPlayer source)
    {
        
    }
}
