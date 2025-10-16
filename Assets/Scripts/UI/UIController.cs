using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(UIController))]
public class UIControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var master = (UIController)target;

        GUILayout.BeginHorizontal(GUILayout.Height(30));

        if (GUILayout.Button("Refresh Screen List"))
        {
            master.CollectScreens();
        }

        if (GUILayout.Button("Hide All"))
        {
            master.HideAll();
        }

        GUILayout.EndHorizontal();

        foreach (var item in master.screensList)
        {
            GUILayout.BeginHorizontal();

            var originalColor = GUI.backgroundColor;
            var btnLabel = item.Value.gameObject.activeSelf ? "Enabled" : "Disabled";

            GUI.backgroundColor = item.Value.gameObject.activeSelf ? Color.cyan : Color.grey;

            if (GUILayout.Button(btnLabel, GUILayout.Width(80)))
            {
                item.Value.gameObject.SetActive(!item.Value.gameObject.activeSelf);

                if (item.Value.gameObject.activeSelf)
                {
                    EditorGUIUtility.PingObject(item.Value.gameObject);
                }
            }

            GUILayout.Label($"{item.Key.ToString()}");


            GUI.backgroundColor = originalColor;

            GUILayout.EndHorizontal();
        }

        base.OnInspectorGUI();
    }
}
#endif


[ExecuteInEditMode]
public class UIController : MonoBehaviour
{
    [SerializeField] public Dictionary<UITypes, UI> screensList = new Dictionary<UITypes, UI>();

    // private void Start()
    // {
    //     UI.Show(UITypes.Loading);
    // }

    // private void OnEnable()
    // {
    //     EventManager.Section.OnSectionEvent += OnSection;
    // }

    // private void OnDisable()
    // {
    //     EventManager.Section.OnSectionEvent -= OnSection;
    // }

    // private void OnSection(SectionTypes section)
    // {
    //     switch (section)
    //     {
    //         case SectionTypes.Home:
    //             UI.Show(UITypes.Home);
    //             //Utils.DelayAction(0.01f, () =>
    //             //{
    //             //});
    //             break;
    //     }
    // }

    void Awake()
    {
        if (!Application.isPlaying) return;
        CollectScreens();

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }

        foreach (var item in screensList)
        {
            screensList[item.Key].gameObject.SetActive(true);
        }
    }

    private void OnValidate()
    {
        CollectScreens();
    }

    public void CollectScreens()
    {
        screensList.Clear();
        foreach (var ui in GetComponentsInChildren<UI>(true))
        {
            if (ui != null && ui.type != UITypes.None)
            {
                screensList.Add(ui.type, ui);
            }
        }
    }

    public void HideAll()
    {
        foreach (var item in screensList)
        {
            screensList[item.Key].gameObject.SetActive(false);
        }
    }

}