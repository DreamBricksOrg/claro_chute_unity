using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class ToggleReactionComponent : MonoBehaviour
{
    public List<GraphicReactionBehaviour> behaviourList;
    private bool isNormal = true;

    public UnityEvent OnNormalEvent;
    public UnityEvent OnSelectedEvent;

    [System.Serializable]
    public class GraphicReactionBehaviour
    {
        public GraphicReactionBehaviour()
        {
            this.transitionTime = 1f;
            this.colorNormal = new Color(1, 1, 1, 1);
            this.colorSelected = new Color(1, 1, 1, 1);
        }

        public Graphic graphic;
        public float transitionTime = 1f;
        private Color colorNormal = new Color(1, 1, 1, 1);
        public Color colorSelected = new Color(1, 1, 1, 1);
        private bool setupColor = false;

        public void SetNormal()
        {
            SetupNormalColor();
            this.graphic.DOColor(this.colorNormal, this.transitionTime);
        }

        public void SetSelected()
        {
            SetupNormalColor();
            this.graphic.DOColor(this.colorSelected, this.transitionTime);
        }

        private void SetupNormalColor()
        {
            if (!this.setupColor)
            {
                this.colorNormal = this.graphic.color;
                this.setupColor = true;
            }
        }
    }

    public void SetNormal()
    {
        if (isNormal) return;
        isNormal = true;
        foreach (var item in behaviourList)
        {
            item.SetNormal();
        }
        OnNormalEvent?.Invoke();
    }

    public void SetSelected()
    {
        if (!isNormal) return;
        isNormal = false;
        foreach (var item in behaviourList)
        {
            item.SetSelected();
        }
        OnSelectedEvent?.Invoke();
    }
}
