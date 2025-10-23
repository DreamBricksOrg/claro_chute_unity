using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    public static class Delegates
    {
        public delegate void SectionHandler(SectionTypes sectionType);
        public delegate void GameScoreHandler(int score);
        public delegate void BallStyleHandler(int styleId);
        public delegate void GameScoreUpdateHandler(int globalScore);
        public delegate void GameTimeHandler(float time);
        public delegate void GameEndHandler();
        public delegate void GameStartHandler();
        public delegate void SetElementStateHandler(ElementTypes elementType, bool state);
        public delegate void KinectBodyDetectedHandler(bool state);
        public delegate void KinectHandPositionHandler(bool isRight, Vector3 position);
        public delegate void SetLogHandler(LogTypeInfo logType, string msg = "");
        public delegate void BallCreatedHandler(GameObject ball);
        public delegate void ShootSpeedHandler(float speed);
        public delegate void ShootResultHandler(bool isGoal, string info);
        public delegate void NextRoundHandler();
        // VideoReplay
        public delegate void VideoReplayProcessHandler();
        public delegate void VideoReplayCompletedHandler(string url);
        // Core System
        public delegate void CoreSystemStateHandler(bool isCoreSystemWorking);

    }

    public static class Section
    {
        public static event Delegates.SectionHandler OnSectionEvent;
        public static void SetSection(SectionTypes section) => OnSectionEvent?.Invoke(section);
    }

    public static class CoreSystem
    {
        public static event Delegates.CoreSystemStateHandler OnCoreSystemStateEvent;
        public static void SetState(bool isCoreSystemWorking) => OnCoreSystemStateEvent?.Invoke(isCoreSystemWorking);
    }

    public static class Kinect
    {
        public static event Delegates.KinectHandPositionHandler OnKinectHandPositionEvent;
        public static void KinectHandPosition(bool isRight, Vector3 position) => OnKinectHandPositionEvent?.Invoke(isRight, position);
        public static event Delegates.KinectHandPositionHandler OnKinectFootPositionEvent;
        public static void KinectFootPosition(bool isRight, Vector3 position) => OnKinectFootPositionEvent?.Invoke(isRight, position);
        public static event Delegates.KinectBodyDetectedHandler OnKinectBodyDetectedEvent;
        public static void KinectBodyDetected(bool state) => OnKinectBodyDetectedEvent?.Invoke(state);
    }

    public static class Game
    {
        public static event Delegates.BallStyleHandler OnBallStyleEvent;
        public static void SetBallStyle(int styleId) => OnBallStyleEvent?.Invoke(styleId);
        public static event Delegates.GameScoreHandler OnGameScoreEvent;
        public static void SetGameScore(int score) => OnGameScoreEvent?.Invoke(score);
        public static event Delegates.GameScoreUpdateHandler OnGameScoreUpdateEvent;
        public static void UpdateGameScore(int globalScore) => OnGameScoreUpdateEvent?.Invoke(globalScore);
        public static event Delegates.GameTimeHandler OnGameTimeChangeEvent;
        public static void SetGameTime(float time) => OnGameTimeChangeEvent?.Invoke(time);
        public static event Delegates.GameEndHandler OnGameEndEvent;
        public static void GameEnd() => OnGameEndEvent?.Invoke();
        public static event Delegates.GameStartHandler OnGameStartEvent;
        public static void GameStart() => OnGameStartEvent?.Invoke();
        public static event Delegates.BallCreatedHandler OnBallCreatedEvent;
        public static void BallCreated(GameObject ball) => OnBallCreatedEvent?.Invoke(ball);
        public static event Delegates.ShootSpeedHandler OnShootSpeedEvent;
        public static void ShootSpeed(float speed) => OnShootSpeedEvent?.Invoke(speed);
        public static event Delegates.ShootResultHandler OnShootResultEvent;
        public static void ShootResult(bool isGoal, string info) => OnShootResultEvent?.Invoke(isGoal, info);
        public static event Delegates.NextRoundHandler OnNextRoundEvent;
        public static void NextRound() => OnNextRoundEvent?.Invoke();
    }

    public static class VideoReplay
    {
        public static event Delegates.VideoReplayProcessHandler OnVideoReplayProcessEvent;
        public static void VideoReplayProcess() => OnVideoReplayProcessEvent?.Invoke();
        public static event Delegates.VideoReplayCompletedHandler OnVideoReplayCompletedEvent;
        public static void VideoReplayCompleted(string url) => OnVideoReplayCompletedEvent?.Invoke(url);
    }

    public static class Element
    {
        public static event Delegates.SetElementStateHandler OnElementStateEvent;
        public static void SetElementState(ElementTypes elementType, bool state) => OnElementStateEvent?.Invoke(elementType, state);
    }

    public static class Log
    {
        public static event Delegates.SetLogHandler OnLogEvent;
        public static void SetLog(LogTypeInfo logType, string msg = "") => OnLogEvent?.Invoke(logType, msg);
    }

}