using System;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ranking : UI
{
	public Button btnConfirm;
	public Transform toggleRoot;
    public GameObject togglePrefab;

	internal override void Awake()
	{
		base.Awake();
		btnConfirm.onClick.AddListener(() =>
		{
			// AudioController.PlaySFX(AudioTypes.SFX_uiClick);
			EventManager.Section.SetSection(SectionTypes.Game);
		});
	}

	internal override void OnShow(object data, Action<object> callback)
    {
        base.OnShow(data, callback);

        API.Request("/api/getranking", 
            onSuccess: (result) => {
				var jsonData = JSON.Parse(result);
				CreateRanking(jsonData["player_list"]);
            },
            onError: (error) => {
                Debug.LogError("API Error: " + error);
            }
        );
    }

	void ClearRanking()
    {
        foreach (Transform child in toggleRoot)
        {
            Destroy(child.gameObject);
        }
    }

    void CreateRanking(JSONNode playerList)
	{
		ClearRanking();
        for (int i = 0; i < playerList.Count; i++)
        {
            if (i >= 10) break;
            var rankingBox = Instantiate(togglePrefab, toggleRoot);
			var comp = rankingBox.GetComponent<Widget_RankingBox>();
            comp.SetAttributes(playerList[i]);
            
        }
    }
}
