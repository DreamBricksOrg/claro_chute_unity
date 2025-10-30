using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ranking : UI
{
    public Button btnConfirm;
    public Transform playerBoxRoot;
    public List<Widget_RankingBox> rankingBoxList = new();

    internal override void Awake()
    {
        base.Awake();
        btnConfirm.onClick.AddListener(() =>
        {
            // AudioController.PlaySFX(AudioTypes.SFX_uiClick);
            EventManager.Section.SetSection(SectionTypes.Terms);
        });
        EventManager.Section.OnSectionEvent += OnSection;
        SetAllRankingBoxes();
    }

    internal override void OnDestroy()
    {
        base.OnDestroy();
        EventManager.Section.OnSectionEvent -= OnSection;
    }

    internal override void OnShow(object data, Action<object> callback)
    {
        base.OnShow(data, callback);
    }

    void SetAllRankingBoxes()
    {
        rankingBoxList.Clear();
        foreach (var box in playerBoxRoot.GetComponentsInChildren<Widget_RankingBox>())
        {
            rankingBoxList.Add(box);
        }
    }

    void CreateRanking(JSONNode playerList)
    {
        for (int i = 0; i < rankingBoxList.Count; i++)
        {
            rankingBoxList[i].SetAttributes(playerList[i]);
        }
    }

    private void OnSection(SectionTypes sectionType)
    {
        switch (sectionType)
        {
            case SectionTypes.Intro:
                API.Request("/api/getranking",
                    onSuccess: (result) =>
                    {
                        var jsonData = JSON.Parse(result);
                        CreateRanking(jsonData["player_list"]);
                    },
                    onError: (error) =>
                    {
                        Debug.LogError("API Error: " + error);
                    }
                );
                break;
        }
    }
}
