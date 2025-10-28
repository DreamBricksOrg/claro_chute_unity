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

	internal override void Start()
	{
		base.Start();
		// EventManager.VideoReplay.OnVideoReplayProcessEvent += OnVideoReplayProcess;
		EventManager.VideoReplay.OnVideoReplayCompletedEvent += OnVideoReplayCompleted;
	}

	internal override void OnDestroy()
	{
		base.OnDestroy();
		// EventManager.VideoReplay.OnVideoReplayProcessEvent -= OnVideoReplayProcess;
		EventManager.VideoReplay.OnVideoReplayCompletedEvent -= OnVideoReplayCompleted;
	}

	// private void OnVideoReplayProcess()
	// {
	// 	API.Request("/api/process-video/" + GameRoundController.Instance.playerId,
	// 		onSuccess: (result) =>
	// 		{
	// 			var jsonData = JSON.Parse(result);
	// 			Debug.Log("<<Video Generation Response>>" + jsonData.ToString());

	// 			// Loading
	// 			EventManager.VideoReplay.VideoReplayCompleted(jsonData["video"]["download_url"].Value);
	// 			EventManager.Section.SetSection(SectionTypes.Replay);

	// 			// {
	// 			//     "status": "success",
	// 			//     "player": {
	// 			//         "id": "180c5dbf-5386-49ae-97c0-9306a96716ea",
	// 			//         "score": 201,
	// 			//         "position": 7,
	// 			//         "created_at": "2025-10-23T21:36:08.592000"
	// 			//     },
	// 			//     "video": {
	// 			//         "original_filename": "180c5dbf-5386-49ae-97c0-9306a96716ea_claro_tvbox.mp4",
	// 			//         "processed_filename": "180c5dbf-5386-49ae-97c0-9306a96716ea_claro_tvbox_processed.mp4",
	// 			//         "path": "/api/video/180c5dbf-5386-49ae-97c0-9306a96716ea",
	// 			//         "download_url": "https://clarotvboxchute.ngrok.app/api/video/180c5dbf-5386-49ae-97c0-9306a96716ea",
	// 			//         "processed": true,
	// 			//         "processing_time": 3.404806137084961,
	// 			//         "output_file": "C:\\Users\\db\\Documents\\db\\prj\\claro_tvbox\\claro_tvbox_server\\app\\recordings\\180c5dbf-5386-49ae-97c0-9306a96716ea_claro_tvbox_processed.mp4"
	// 			//     },
	// 			//     "message": "Vídeo processado com sucesso com labels aplicados"
	// 			// }

	// 		},
	// 		onError: (error) =>
	// 		{
	// 			EventManager.Section.SetSection(SectionTypes.Gameover);
	// 			Debug.LogError("API Error: " + error);
	// 		}
	// 	);
	// }

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
	}

	private void OnVideoEnd(VideoPlayer source)
	{
		EventManager.Section.SetSection(SectionTypes.Gameover);
	}

}
