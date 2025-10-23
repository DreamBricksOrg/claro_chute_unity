using System;
using DG.Tweening;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class UI_Replay : UI
{
	public TMP_Text fieldScore;
	public TMP_Text fieldRanking;
	public VideoPlayer videoPlayer;
	public CanvasGroup replayerCanvasGroup;

	internal override void OnShow(object data, Action<object> callback)
	{
		base.OnShow(data, callback);

		replayerCanvasGroup.DOFade(1f, 0.5f).SetLoops(-1, LoopType.Yoyo);

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

		GetVideoReplay();

	}

	private void GetVideoReplay()
	{
		API.Request("/api/getvideo/" + GameRoundController.Instance.playerId,
			onSuccess: (result) =>
			{
				var jsonData = JSON.Parse(result);
				
				videoPlayer.url = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
				// videoPlayer.url = jsonData["video_url"].Value;
				videoPlayer.loopPointReached += OnVideoEnd;
				videoPlayer.Play();
			},
			onError: (error) =>
			{
				Debug.LogError("API Error: " + error);
			}
		);
	}

	internal override void OnHide(object data, Action<object> callback)
	{
		base.OnHide(data, callback);
		replayerCanvasGroup.DOKill();
	}


	private void OnVideoEnd(VideoPlayer source)
	{

	}

}
