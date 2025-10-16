using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LayoutGroupRefresh : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        Refresh();
    }

    public void Refresh()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }
}
