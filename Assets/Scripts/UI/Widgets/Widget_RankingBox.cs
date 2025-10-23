using SimpleJSON;
using TMPro;
using UnityEngine;

public class Widget_RankingBox : MonoBehaviour
{

    public TMP_Text fieldPosition;
    public TMP_Text fieldName;
    public TMP_Text fieldScore;
    
    public void SetAttributes(JSONNode playerData)
    {
        fieldName.SetText(playerData["id"].Value);
        fieldScore.SetText(playerData["score"].Value + "<size=40%>km/h</size>");
        fieldPosition.SetText(playerData["position"].Value);
    }

}
