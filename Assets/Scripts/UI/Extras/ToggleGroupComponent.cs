using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleGroupComponent : MonoBehaviour
{
    public bool canDeselect = true;
    public UnityEvent<int> OnChange;
    public int defaultStartIndex = 0;
    public List<ToggleReactionComponent> reactionList = new List<ToggleReactionComponent>();
    private int m_currentIndex = -1;
    public int CurrentIndex
    {
        get => m_currentIndex;
        set 
        {
            var dir = value > m_currentIndex ? true : false;

            if (value <= -1)
            {
                m_currentIndex = -1;
                Refresh();
                return;
            }

            m_currentIndex = value;
            m_currentIndex = Mathf.Clamp(m_currentIndex, 0, reactionList.Count-1);
            
            if (reactionList[m_currentIndex].isActiveAndEnabled)
            {
                Refresh();
            }
            else
            {
                if (m_currentIndex > 0 && m_currentIndex < reactionList.Count - 1)
                {
                    m_currentIndex = dir ? m_currentIndex + 1 : m_currentIndex - 1;
                }
            }
        }
    }

    private void Awake()
    {
        for (int i = 0; i < reactionList.Count; i++)
        {
            var index = i;
            try
            {
                reactionList[i].GetComponent<Button>()?.onClick.AddListener(() => SetIndex(index));
            }
            catch (Exception)
            {
            }
        }

    }

    private void Start()
    {
        SetIndexWhithoutNotify(defaultStartIndex);
    }

    public void SetIndex(int index)
    {
        if (CurrentIndex == index && canDeselect) CurrentIndex = -1; else CurrentIndex = index;
        OnChange?.Invoke(CurrentIndex);
    }

    public void SetIndexWhithoutNotify(int index)
    {
        if (CurrentIndex == index && canDeselect) CurrentIndex = -1; else CurrentIndex = index;
    }

    void Refresh()
    {
        for (int i = 0; i < reactionList.Count; i++)
        {
            try
            {
                if (i == CurrentIndex) reactionList[i].SetSelected(); else reactionList[i].SetNormal();
            }
            catch (Exception)
            {
            }
        }
    }

}
