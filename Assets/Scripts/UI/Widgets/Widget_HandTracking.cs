using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Widget_HandTracking : MonoBehaviour
{
    public Image cooldownImage;
    public CanvasGroup canvasGroup;
    public RectTransform playButtonRect;
    public float cooldownDuration = 3f;
    public bool isWorking = true;
    bool isFreeHovering = true;
    Button activeButton;
    bool isHoveringOverButton = false;


    float _currentCooldownValue = 0f;
    float CurrentCooldownValue
    {
        get => _currentCooldownValue;
        set
        {
            _currentCooldownValue = Mathf.Clamp(value, 0f, 1f);
            if (_currentCooldownValue > 0f)
            {
                isHoveringOverButton = true;
                playButtonRect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            }
            else
            {
                isHoveringOverButton = false;
                playButtonRect.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutBack);
            }
            UpdateCooldownImage();
        }
    }

    GameObject currentHoveredObject = null;
    RectTransform rectTransform;
    Canvas canvas;
    public bool isRightHand = true;
    PointerEventData pointerData;
    private GameObject lastHoveredObject;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        CurrentCooldownValue = 0f;
    }

    void OnEnable()
    {
        EventManager.Kinect.OnKinectHandPositionEvent += OnKinectHandPosition;
        EventManager.Section.OnSectionEvent += OnSection;
    }

    void OnDisable()
    {
        EventManager.Kinect.OnKinectHandPositionEvent -= OnKinectHandPosition;
        EventManager.Section.OnSectionEvent -= OnSection;
    }

    private void OnSection(SectionTypes sectionType)
    {
        switch (sectionType)
        {
            case SectionTypes.Intro:
            case SectionTypes.Ranking:
            case SectionTypes.HowToPlay:
                isWorking = true;
                isFreeHovering = true;
                UpdateCooldownImage();
                canvasGroup.alpha = 1f;
                break;
            default:
                isWorking = false;
                isFreeHovering = false;
                CurrentCooldownValue = 0f;
                UpdateCooldownImage();
                canvasGroup.alpha = 0f;
                break;
        }
    }

    private void OnKinectHandPosition(bool isRight, Vector3 position)
    {
        if (!isWorking) return;
        if (isRightHand != isRight) return;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);
        if (rectTransform != null && canvas != null)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPosition,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out localPoint
            );
            rectTransform.anchoredPosition = localPoint;
            pointerData = new PointerEventData(EventSystem.current)
            {
                position = screenPosition
            };
        }
    }

    void Update()
    {
        if (!isWorking) return;
        if (isFreeHovering) DetectButtonUnderWidget();
    }

    private void DetectButtonUnderWidget()
    {
        if (canvas == null || pointerData == null) return;
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        var isHoveringOverButton = false;
        currentHoveredObject = null;
        foreach (var result in results)
        {
            activeButton = result.gameObject.GetComponent<Button>();
            if (activeButton != null)
            {
                currentHoveredObject = result.gameObject;
                CurrentCooldownValue = Mathf.Clamp(_currentCooldownValue + Time.deltaTime / cooldownDuration, 0f, 1f);
                isHoveringOverButton = true;
                break;
            }
        }

        if (lastHoveredObject != null && lastHoveredObject != currentHoveredObject)
        {
            ExecuteEvents.Execute<IPointerExitHandler>(lastHoveredObject, pointerData, ExecuteEvents.pointerExitHandler);
        }
        if (currentHoveredObject != null && lastHoveredObject != currentHoveredObject)
        {
            ExecuteEvents.Execute<IPointerEnterHandler>(currentHoveredObject, pointerData, ExecuteEvents.pointerEnterHandler);
        }

        lastHoveredObject = currentHoveredObject;

        if (!isHoveringOverButton) CurrentCooldownValue = 0;

        if (CurrentCooldownValue >= 1f)
        {
            CurrentCooldownValue = 0f;
            CallToAction();
        }
    }

    void UpdateCooldownImage()
    {
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = CurrentCooldownValue;
        }
    }

    void CallToAction()
    {
        if (currentHoveredObject != null)
        {
            ExecuteEvents.Execute<IPointerClickHandler>(currentHoveredObject, pointerData, ExecuteEvents.pointerClickHandler);
            activeButton?.onClick.Invoke();
            isFreeHovering = false;
            Invoke(nameof(ResetCooldown), 1f);
        }
    }

    public void ResetCooldown()
    {
        CurrentCooldownValue = 0f;
        isFreeHovering = true;
        UpdateCooldownImage();
    }
}
