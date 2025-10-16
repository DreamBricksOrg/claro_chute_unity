using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class WorldScoreComponent : MonoBehaviour
{
    public int scoreValue = 0;
    public TMP_Text fieldValue;
    public Color colorPositive = Color.white;
    public Color colorPositiveOutline = Color.white;
    public Color colorNegative = Color.red;
    public Color colorNegativeOutline = Color.red;
    public Color colorSpecial = Color.yellow;
    public Color colorSpecialOutline = Color.red;

    public void SetValue(int value)
    {
        string signText = "";
        if (value != 0)
        {
            signText = (value > 0) ? "+" : "-";
        }

        fieldValue.text = signText + Mathf.Abs(value).ToString();

        var colorBody = Color.white;
        var colorOutline = Color.white;

        if (value > 10)
        {
            colorBody = colorSpecial;
            colorOutline = colorSpecialOutline;
        }
        else if (value > 0)
        {
            colorBody = colorPositive;
            colorOutline = colorPositiveOutline;
        }
        else if (value < 0)
        {
            colorBody = colorNegative;
            colorOutline = colorNegativeOutline;
        }

        fieldValue.fontMaterial.SetColor("_FaceColor", colorBody);
        fieldValue.fontMaterial.SetColor("_OutlineColor", colorOutline);

        transform.DOScale(Vector3.zero, 0.32f)
            .SetDelay(1f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                Destroy(gameObject);
            });
    }

}
