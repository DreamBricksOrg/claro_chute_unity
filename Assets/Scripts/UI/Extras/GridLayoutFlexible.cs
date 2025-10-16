using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridLayoutFlexible : GridLayoutGroup
{
    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        var rectTransform = (RectTransform)transform;
        cellSize = new Vector2((rectTransform.rect.width - (m_Spacing.x * (m_ConstraintCount - 1))) / m_ConstraintCount, m_CellSize.y);
    }
}
