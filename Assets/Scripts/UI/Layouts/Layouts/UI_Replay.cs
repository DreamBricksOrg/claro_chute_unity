using System;
using DG.Tweening;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UI_Replay : UI
{
	public VideoPlayer videoPlayer;
	public CanvasGroup videoCanvasGroup;
	public RectTransform loadingSpinner;
	private RawImage videoRawImage;

	internal override void Awake()
	{
		base.Awake();
		videoRawImage = videoPlayer.GetComponent<RawImage>();
	}

	internal override void Start()
	{
		base.Start();
		EventManager.Section.OnSectionEvent += OnSection;
		EventManager.VideoReplay.OnVideoReplayCompletedEvent += OnVideoReplayCompleted;
		videoCanvasGroup.alpha = 0;
	}

	internal override void OnDestroy()
	{
		base.OnDestroy();
		EventManager.Section.OnSectionEvent -= OnSection;
		EventManager.VideoReplay.OnVideoReplayCompletedEvent -= OnVideoReplayCompleted;
	}

	private void OnSection(SectionTypes sectionType)
	{
		if (sectionType == SectionTypes.Replay)
		{
			if (videoPlayer.isPrepared)
			{
				Play();
			}
		}
	}

	override internal void OnShow(object data, Action<object> callback)
	{
		base.OnShow(data, callback);
		loadingSpinner.DORotate(new Vector3(0, 0, -360), 1f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
		ClearRenderTexture();
	}

	override internal void OnHide(object data, Action<object> callback)
	{
		base.OnHide(data, callback);
		loadingSpinner.DOKill();
		ClearRenderTexture();
	}

	public void ClearRenderTexture()
	{
		var rt = videoRawImage.texture as RenderTexture;
        var currentRT = RenderTexture.active;
        RenderTexture.active = rt;
        GL.Clear(true, true, new Color(0f, 0f, 0f, 0f));
        RenderTexture.active = currentRT;
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
		if (Main.Instance.currentSectionType != SectionTypes.Replay) return;
		Play();
	}

	void Play()
	{
		videoPlayer.Play();
		videoCanvasGroup.DOFade(1f, 0.3f).SetEase(Ease.Linear);
	}

	private void OnVideoEnd(VideoPlayer source)
	{
		videoCanvasGroup.alpha = 0;
		// videoCanvasGroup.DOFade(0, 0.3f);
		EventManager.Section.SetSection(SectionTypes.Gameover);
	}

}
