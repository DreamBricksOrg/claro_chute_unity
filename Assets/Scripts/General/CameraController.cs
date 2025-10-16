using DG.Tweening;
using System;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // public Transform aimTransform;
    // public CinemachineOrbitalFollow orbit;
    // private Tween horizontalTween;
    // private Tween verticalTween;
    // private float centerTime = 1f;
    // private bool isAutoSpin = true;
    // public float speed = 3f;

    // void OnEnable()
    // {
    //     Events.ChangeCameraAimEvent += OnChangeCameraAim;
    //     Events.ChangeStateCameraSpinEvent += OnCameraAutoSpin;
    // }

    // void OnDisable()
    // {
    //     Events.ChangeCameraAimEvent -= OnChangeCameraAim;
    //     Events.ChangeStateCameraSpinEvent -= OnCameraAutoSpin;
    // }

    // private void OnCameraAutoSpin(bool flag)
    // {
    //     isAutoSpin = flag;
    // }

    // private void Update()
    // {
    //     if (isAutoSpin)
    //     {
    //         orbit.HorizontalAxis.Value += speed * Time.deltaTime;
    //     }
    // }

    // private void OnChangeCameraAim(float newPosY)
    // {
    //     aimTransform.DOKill();
    //     aimTransform.DOMoveY(newPosY, 1f).SetEase(Ease.OutQuad);
    //     RecenterCamera();
    // }

    // private void RecenterCamera()
    // {
    //     horizontalTween?.Kill();
    //     horizontalTween = DOTween.To(() => orbit.HorizontalAxis.Value, x => orbit.HorizontalAxis.Value = x, 225, centerTime)
    //         .SetEase(Ease.InOutQuad)
    //         .OnComplete(() => horizontalTween = null);

    //     verticalTween?.Kill();
    //     verticalTween = DOTween.To(() => orbit.VerticalAxis.Value, x => orbit.VerticalAxis.Value = x, 18, centerTime)
    //         .SetEase(Ease.InOutQuad)
    //         .OnComplete(() => verticalTween = null);
    // }
}

