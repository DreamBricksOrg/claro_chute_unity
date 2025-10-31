using System;
using DG.Tweening;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class UI_Replay : UI
{
	public VideoPlayer videoPlayer;
	public CanvasGroup videoCanvasGroup;
	public RectTransform loadingSpinner;

	internal override void Start()
	{
		base.Start();
		EventManager.VideoReplay.OnVideoReplayCompletedEvent += OnVideoReplayCompleted;
		videoCanvasGroup.alpha = 0;
	}

	internal override void OnDestroy()
	{
		base.OnDestroy();
		EventManager.VideoReplay.OnVideoReplayCompletedEvent -= OnVideoReplayCompleted;
	}

	override internal void OnShow(object data, Action<object> callback)
	{
		base.OnShow(data, callback);
		loadingSpinner.DORotate(new Vector3(0, 0, -360), 1f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
	}

	override internal void OnHide(object data, Action<object> callback)
	{
		base.OnHide(data, callback);
		loadingSpinner.DOKill();
	}

	private void OnVideoReplayCompleted(string url)
	{
		Debug.Log("<<Playing Replay Video>>" + url);
		videoPlayer.url = url;
		videoPlayer.loopPointReached -= OnVideoEnd;
		videoPlayer.prepareCompleted -= OnVideoPrepared;
		videoPlayer.loopPointReached += OnVideoEnd;
		videoPlayer.prepareCompleted += OnVideoPrepared;
		videoPlayer.Prepare();
	}

	private void OnVideoPrepared(VideoPlayer source)
	{
		Debug.Log("<<PLAY>>");
		videoPlayer.Play();
		videoCanvasGroup.DOFade(1, 0.3f);
	}

	private void OnVideoEnd(VideoPlayer source)
	{
		videoCanvasGroup.DOFade(0, 0.3f);
		EventManager.Section.SetSection(SectionTypes.Gameover);
	}

}
