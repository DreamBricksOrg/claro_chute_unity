using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace Kasulo.Animations.UI
{
#if UNITY_EDITOR
    [CustomEditor(typeof(UIAnimationModule), true)]
    public class UIAnimationModuleEditor : Editor
    {
        #region VARIABLES
        //GUISkin skin;
        UIAnimationModule master;
        Canvas canvas;
        int selectedIndex = 0;
        Texture2D textureBlue;
        #endregion

        //void GenerateEditorTextures()
        //{
        //    if (textureBlue) DestroyImmediate(textureBlue);
        //    textureBlue = new Texture2D(1,1);

        //    for (int m = 0; m < textureBlue.mipmapCount; m++)
        //    {
        //        Color[] c = textureBlue.GetPixels(m);
        //        for (int i = 0; i < c.Length; i++)
        //        {
        //            c[i].r = 0;
        //            c[i].g = 0;
        //            c[i].b = 1;
        //        }
        //        textureBlue.SetPixels(c, m);
        //    }
        //    textureBlue.Apply();
        //}

        private void OnEnable()
        {
            master = (UIAnimationModule)target;

            //GenerateEditorTextures();

            //skin = Resources.Load<GUISkin>("skin");
            canvas = master.GetComponentInParent<Canvas>();

            master.reorderableList = new ReorderableList(master.itemList, typeof(UIAnimationModule.AnimationItem), true, false, false, false);
            master.reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                DrawListItems(master.itemList[index], rect, index, isActive, isFocused);
            };

            master.reorderableList.elementHeightCallback = (int index) =>
            {
                var size = 60;
                if (master.grid == 0)
                {
                    size = (master.itemList[index].foldout) ? 60 : 18;
                }
                else
                {
                    size = (master.itemList[index].foldout) ? 210 : 18;
                }

                var elementHeight = master.reorderableList.count;
                if (elementHeight >= 1)
                {
                    elementHeight--;
                }
                return size + elementHeight;
            };

            master.reorderableList.onSelectCallback = OnSelectCallback;

            OnHierarchyChanged();
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            master.grid = 0;

        }

        private void OnDisable()
        {
            if (!Application.isPlaying) master.SetRest();
            master.grid = 0;
        }

        private void OnHierarchyChanged()
        {
            if (!Application.isPlaying) master.SetRest();
        }

        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            GUILayout.BeginVertical();

            DrawAnimation();

            GUILayout.Space(8);
            GUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Animation changed");
            }

            if (GUI.changed)
            {
                //Undo.RecordObject(target, "Widget changed");
                EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();
        }

        void OnSceneGUI()
        {
            if (master.itemList == null) return;
            if (!master.debugShowWireframe) return;

            var i = -1;
            var canvasSize = canvas.transform.localScale.x;
            var rot = canvas.transform.rotation;

            switch (master.grid)
            {
                case 0:
                    break;
                case 1:
                    foreach (var item in master.itemList)
                    {
                        i++;
                        if (!item.driven || !item.driven.gameObject.activeSelf) continue;

                        var motion = (master.grid == 1) ? item.motionIn : item.motionOut;

                        var size = HandleUtility.GetHandleSize(item.driven.position) * 0.24f;
                        Handles.color = Color.cyan;
                        if (selectedIndex == i)
                        {
                            var offset = motion.position.value * canvasSize;
                            var newOffset = Handles.PositionHandle(offset + item.driven.position, rot) - item.driven.position;
                            motion.position.value = new Vector3(newOffset.x / canvasSize, newOffset.y / canvasSize, newOffset.z / canvasSize);
                            Handles.color = Color.yellow;
                            SceneView.RepaintAll();
                        }
                        var targetPosition = item.driven.position + (motion.position.value * canvas.transform.localScale.x);
                        var verts = new Vector3[4];
                        item.driven.GetWorldCorners(verts);
                        Handles.DrawSolidRectangleWithOutline(verts, Handles.color - new Color(0f, 0f, 0f, 0.9f), Handles.color);
                        if (motion.position.value.magnitude > 0)
                        {
                            Handles.DrawDottedLine(item.driven.position, targetPosition, 4);
                            Handles.ConeHandleCap(0, targetPosition, Quaternion.LookRotation(-motion.position.value, item.driven.forward), size, EventType.Repaint);
                        }
                    }
                    break;
                case 2:
                    foreach (var item in master.itemList)
                    {
                        var motion = (master.grid == 1) ? item.motionIn : item.motionOut;

                        i++;
                        if (!item.driven || !item.driven.gameObject.activeSelf) continue;
                        var size = HandleUtility.GetHandleSize(item.driven.position) * 0.24f;
                        Handles.color = Color.red;
                        if (selectedIndex == i)
                        {
                            var offset = motion.position.value * canvasSize;
                            var newOffset = Handles.PositionHandle(offset + item.driven.position, rot) - item.driven.position;
                            motion.position.value = new Vector3(newOffset.x / canvasSize, newOffset.y / canvasSize, newOffset.z / canvasSize);
                            Handles.color = Color.yellow;
                            SceneView.RepaintAll();
                        }
                        var targetPosition = item.driven.position + (motion.position.value * canvas.transform.localScale.x);
                        var verts = new Vector3[4];
                        item.driven.GetWorldCorners(verts);
                        Handles.DrawSolidRectangleWithOutline(verts, Handles.color - new Color(0f, 0f, 0f, 0.9f), Handles.color);
                        if (motion.position.value.magnitude > 0)
                        {
                            Handles.DrawDottedLine(item.driven.position, targetPosition, 4);
                            Handles.ConeHandleCap(0, targetPosition, Quaternion.LookRotation(motion.position.value, item.driven.forward), size, EventType.Repaint);
                        }
                    }
                    break;
            }
        }

        Rect GetRect(ref Rect rect)
        {
            rect.y += EditorGUIUtility.singleLineHeight + 5;
            return rect;
        }

        void DrawListItems(UIAnimationModule.AnimationItem item, Rect rect, int index, bool isActive, bool isFocused)
        {

            var elementTitle = new GUIContent();
            if (item.driven)
            {
                elementTitle.text = item.driven.name;
                elementTitle.image = EditorGUIUtility.IconContent("d_RectTransform Icon").image;
            }
            else
            {
                elementTitle.text = "-- WARNING - No driven assigned --";
                elementTitle.image = EditorGUIUtility.IconContent("conflict-icon").image;
            }

            GUI.color = item.driven ? Color.white : new Color(2, 1, 1);
            var style = new GUIStyle(EditorStyles.toolbar);
            style.fontSize = 10;

            GUI.Label(new Rect(rect.x, rect.y, Screen.width - 75, EditorGUIUtility.singleLineHeight), elementTitle, style);
            var rectLocal = new Rect(rect.x, rect.y, Screen.width - 75, EditorGUIUtility.singleLineHeight);

            rectLocal.width -= 16;
            rectLocal.x += 16;
            var currentRect = rectLocal;
            var guiContent = new GUIContent();

            item.foldout = EditorGUI.Foldout(rectLocal, item.foldout, "");
            if (!item.foldout) return;

            if (master.grid == 0)
            {
                var r = GetRect(ref rectLocal);
                r.height = 32;
                item.driven = (RectTransform)EditorGUI.ObjectField(r, item.driven, typeof(RectTransform), true);
            }
            else
            {
                var motion = (master.grid == 1) ? item.motionIn : item.motionOut;
                style = new GUIStyle(EditorStyles.miniButton);
                style.fontSize = 10;
                var content = new GUIContent("Use Curve", EditorGUIUtility.IconContent("UnityEditor.Graphs.AnimatorControllerTool").image);

                currentRect = GetRect(ref rectLocal);

                // Delay
                currentRect.width = 40;
                EditorGUI.LabelField(currentRect, "Delay");
                currentRect.x += 50;
                motion.delay = EditorGUI.FloatField(currentRect, "", motion.delay);

                GetRect(ref rectLocal);
                rectLocal.y += 6;

                #region POSITION
                //EditorGUI.DropShadowLabel(rectLocal, "Position", skin.GetStyle("headerItem"));
                EditorGUI.DropShadowLabel(rectLocal, "Position", EditorStyles.toggle);
                currentRect = rectLocal;
                currentRect.x += 4;
                currentRect.width = 24;

                motion.position.isEnabled = EditorGUI.Toggle(currentRect, motion.position.isEnabled);
                GUI.enabled = motion.position.isEnabled;
                currentRect.x = rectLocal.width - 35;
                currentRect.width = 80;
                motion.position.ease = (Ease)EditorGUI.EnumPopup(currentRect, motion.position.ease);
                currentRect.x -= 40;
                currentRect.width = 35;
                motion.position.delay = EditorGUI.FloatField(currentRect, motion.position.delay);
                currentRect.x -= 20;
                guiContent = EditorGUIUtility.IconContent("Loading");
                guiContent.tooltip = "Delay";
                EditorGUI.LabelField(currentRect, guiContent);
                currentRect.x -= 40;
                currentRect.width = 35;
                motion.position.duration = EditorGUI.FloatField(currentRect, "", motion.position.duration);
                currentRect.x -= 20;
                guiContent = EditorGUIUtility.IconContent("UnityEditor.AnimationWindow");
                guiContent.tooltip = "Duration";
                EditorGUI.LabelField(currentRect, guiContent);

                // Value
                rectLocal.x += 20;
                currentRect = GetRect(ref rectLocal);
                currentRect.width = 150;
                motion.position.value = EditorGUI.Vector2Field(currentRect, "", (Vector2)motion.position.value);

                currentRect.x = rectLocal.width - 35;
                currentRect.width = 80;

                if (motion.position.ease != Ease.Unset)
                {
                    //EditorGUIUtility.SetIconSize(Vector2.one * 14f);
                    if (GUI.Button(currentRect, content, style)) motion.position.ease = Ease.Unset;
                    //EditorGUIUtility.SetIconSize(Vector2.zero);
                }
                else
                {
                    motion.position.easeCurve = EditorGUI.CurveField(currentRect, motion.position.easeCurve);
                }
                //GUI.enabled = motion.position.ease == Ease.Unset;
                rectLocal.x -= 20;
                GUI.enabled = true;
                #endregion

                rectLocal.y += 8;

                #region FADE
                //EditorGUI.DropShadowLabel(GetRect(ref rectLocal), "Fade", skin.GetStyle("headerItem"));
                EditorGUI.DropShadowLabel(GetRect(ref rectLocal), "Fade", EditorStyles.toggle);
                currentRect = rectLocal;
                currentRect.x += 4;
                currentRect.width = 24;
                motion.fade.isEnabled = EditorGUI.Toggle(currentRect, motion.fade.isEnabled);
                GUI.enabled = motion.fade.isEnabled;
                currentRect.x = rectLocal.width - 35;
                currentRect.width = 80;
                motion.fade.ease = (Ease)EditorGUI.EnumPopup(currentRect, motion.fade.ease);
                currentRect.x -= 40;
                currentRect.width = 35;
                motion.fade.delay = EditorGUI.FloatField(currentRect, motion.fade.delay);
                currentRect.x -= 20;
                guiContent = EditorGUIUtility.IconContent("Loading");
                guiContent.tooltip = "Delay";
                EditorGUI.LabelField(currentRect, guiContent);
                currentRect.x -= 40;
                currentRect.width = 35;
                motion.fade.duration = EditorGUI.FloatField(currentRect, "", motion.fade.duration);
                currentRect.x -= 20;
                guiContent = EditorGUIUtility.IconContent("UnityEditor.AnimationWindow");
                guiContent.tooltip = "Duration";
                EditorGUI.LabelField(currentRect, guiContent);

                // --
                rectLocal.x += 20;
                currentRect = GetRect(ref rectLocal);
                EditorGUI.LabelField(currentRect, "Value");
                currentRect.x += 40;
                currentRect.width = 40;
                motion.fade.value = EditorGUI.FloatField(currentRect, "", motion.fade.value);
                currentRect.x = rectLocal.width - 35;
                currentRect.width = 80;
                if (motion.fade.ease != Ease.Unset)
                {
                    if (GUI.Button(currentRect, content, style)) motion.fade.ease = Ease.Unset;
                }
                else
                {
                    motion.fade.easeCurve = EditorGUI.CurveField(currentRect, motion.fade.easeCurve);
                }
                rectLocal.x -= 20;
                GUI.enabled = true;
                #endregion

                GetRect(ref rectLocal);
                rectLocal.y += 8;

                #region SCALE
                EditorGUI.DropShadowLabel(rectLocal, "Scale", EditorStyles.toggle);
                //EditorGUI.DropShadowLabel(rectLocal, "Scale", skin.GetStyle("headerItem"));
                currentRect = rectLocal;
                currentRect.x += 4;
                currentRect.width = 24;

                motion.scale.isEnabled = EditorGUI.Toggle(currentRect, motion.scale.isEnabled);
                GUI.enabled = motion.scale.isEnabled;
                currentRect.x = rectLocal.width - 35;
                currentRect.width = 80;
                motion.scale.ease = (Ease)EditorGUI.EnumPopup(currentRect, motion.scale.ease);
                currentRect.x -= 40;
                currentRect.width = 35;
                motion.scale.delay = EditorGUI.FloatField(currentRect, motion.scale.delay);
                currentRect.x -= 20;
                guiContent = EditorGUIUtility.IconContent("Loading");
                guiContent.tooltip = "Delay";
                EditorGUI.LabelField(currentRect, guiContent);
                currentRect.x -= 40;
                currentRect.width = 35;
                motion.scale.duration = EditorGUI.FloatField(currentRect, "", motion.scale.duration);
                currentRect.x -= 20;
                guiContent = EditorGUIUtility.IconContent("UnityEditor.AnimationWindow");
                guiContent.tooltip = "Duration";
                EditorGUI.LabelField(currentRect, guiContent);

                // Value
                rectLocal.x += 20;
                currentRect = GetRect(ref rectLocal);
                currentRect.width = 150;
                motion.scale.value = EditorGUI.Vector2Field(currentRect, "", (Vector2)motion.scale.value);

                currentRect.x = rectLocal.width - 35;
                currentRect.width = 80;

                if (motion.scale.ease != Ease.Unset)
                {
                    if (GUI.Button(currentRect, content, style)) motion.scale.ease = Ease.Unset;
                }
                else
                {
                    motion.scale.easeCurve = EditorGUI.CurveField(currentRect, motion.scale.easeCurve);
                }
                rectLocal.x -= 20;
                GUI.enabled = true;
                #endregion
            }


            GUI.color = Color.white;
        }

        void DrawInspectorAnimationGUI()
        {
            GUILayout.Space(15);

            GUILayout.BeginHorizontal(GUILayout.Height(32));

            var style = new GUIStyle(EditorStyles.miniButtonMid);
            style.fixedHeight = 32;

            var toolbarList = new GUIContent[3];
            toolbarList[0] = new GUIContent() { text = " DRIVEN LIST" };
            toolbarList[1] = new GUIContent() { text = " In", image = Resources.Load<Texture>("icon-transition-in") };
            toolbarList[2] = new GUIContent() { text = " Out", image = Resources.Load<Texture>("icon-transition-out") };

            master.grid = GUILayout.Toolbar(master.grid, toolbarList, style, GUILayout.ExpandHeight(true));

            GUILayout.EndHorizontal();

            if (GUI.changed)
            {
                foreach (var item in master.itemList)
                {
                    if (item.driven)
                    {
                        item.canvasComponent = item.driven.GetComponent<CanvasGroup>();
                        if (item.canvasComponent == null) item.canvasComponent = item.driven.gameObject.AddComponent<CanvasGroup>();
                    }
                }
                if (canvas != null) canvas.pixelPerfect = false;
            }
        }

        void OnSelectCallback(ReorderableList list)
        {
            selectedIndex = list.index;
            SceneView.RepaintAll();
        }

        //void DrawToolbar()
        //{
        //    GUILayout.BeginHorizontal();
        //    GUILayout.Label(Resources.Load<Texture>("liveXP"), GUILayout.Height(40), GUILayout.Width(40));
        //    var toolbarList = new GUIContent[3];
        //    toolbarList[0] = new GUIContent() { text = " General", image = Resources.Load<Texture>("icon-general") };
        //    toolbarList[1] = new GUIContent() { text = " Animation", image = Resources.Load<Texture>("icon-animation") };
        //    toolbarList[2] = new GUIContent() { text = " Websocket", image = Resources.Load<Texture>("icon-websocket") };
        //    master.currentStage = GUILayout.Toolbar(master.currentStage, toolbarList, EditorStyles.toolbar, GUILayout.ExpandHeight(true));
        //    GUILayout.EndHorizontal();
        //}

        //void DrawGeneral()
        //{
        //    master.label = EditorGUILayout.TextField("Widget Name", master.label);
        //    EditorGUILayout.Separator();
        //    EditorGUILayout.Space(20);
        //    var guiContent = new GUIContent();
        //    guiContent.text = " (RawImage) list wait for loading before show";
        //    guiContent.image = EditorGUIUtility.IconContent("console.infoicon.inactive.sml").image;
        //    EditorGUILayout.LabelField(guiContent, EditorStyles.boldLabel);
        //    EditorGUILayout.PropertyField(serializedObject.FindProperty("waitForPictureList"));
        //}

        void DrawAnimation()
        {
            DrawWidgetState();

            // Track the previous value of globalDelay
            float previousGlobalDelay = master.globalDelay;

            GUILayout.BeginVertical();
            master.animationStyle = (UIAnimationModule.AnimationStyle)EditorGUILayout.EnumPopup("Animation Style", master.animationStyle);
            master.debugShowWireframe = EditorGUILayout.Toggle(new GUIContent("Show Debug Wireframe"), master.debugShowWireframe);
            master.globalDelay = EditorGUILayout.FloatField("Delay", master.globalDelay);
            if (master.globalDelay != previousGlobalDelay)
            {
                foreach (var item in master.itemList)
                {
                    item.globalDelay = master.globalDelay;
                }
            }
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal(GUILayout.Height(12));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Fold all", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                foreach (var item in master.itemList) item.foldout = false;
            }
            if (GUILayout.Button("Unfold all", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                foreach (var item in master.itemList) item.foldout = true;
            }
            GUILayout.EndHorizontal();

            //master.waitAnimationComplete = EditorGUILayout.Toggle(new GUIContent("Wait Animation Complete", "Wait for the animation to complete in order to change status. Cancel Show/Hide commands during the animation."), master.waitAnimationComplete);

            //master.foloutEvents = EditorGUILayout.Foldout(master.foloutEvents, "Widget Events");
            //if (master.foloutEvents)
            //{
            //    EditorGUILayout.PropertyField(serializedObject.FindProperty("onWidgetShow"));
            //    EditorGUILayout.PropertyField(serializedObject.FindProperty("onWidgetHide"));
            //}

            switch (master.animationStyle)
            {
                case UIAnimationModule.AnimationStyle.Animator:
                    if (!master.animator)
                    {
                        master.animator = master.GetComponent<Animator>();
                        if (!master.animator) master.gameObject.AddComponent<Animator>();
                    }
                    if (master.animator && !master.animator.runtimeAnimatorController)
                    {
                        if (GUILayout.Button("Create Animator", GUILayout.Height(32)))
                        {
                            master.CreateController();
                        }
                    }
                    master.animator = (Animator)EditorGUILayout.ObjectField(master.animator, typeof(Animator), true);
                    break;
                case UIAnimationModule.AnimationStyle.Basic:
                    DrawInspectorAnimationGUI();
                    break;
                case UIAnimationModule.AnimationStyle.Custom:
                    break;
            }

            // ---------------------------------
            master.reorderableList.DoLayoutList();
            GUILayout.Space(-20);
            GUILayout.BeginHorizontal(GUILayout.Width(120), GUILayout.Height(32));

            //try
            //{
            var guiContent = new GUIContent();
            guiContent = EditorGUIUtility.IconContent("Toolbar Plus");
            if (GUILayout.Button(guiContent, GUILayout.ExpandHeight(true))) master.AddAnimationItem();
            GUI.enabled = master.reorderableList.index != -1;
            guiContent = EditorGUIUtility.IconContent("Toolbar Minus");
            if (GUILayout.Button(guiContent, GUILayout.ExpandHeight(true))) master.RemoveAnimationItem();
            GUI.enabled = true;
            //}
            //catch
            //{
            //}

            GUILayout.EndHorizontal();

        }

        void DrawWidgetState()
        {
            GUILayout.BeginHorizontal(GUILayout.Height(32));

            var toolbarList = new GUIContent[2];
            toolbarList[0] = new GUIContent() { text = " Show", image = EditorGUIUtility.IconContent("animationvisibilitytoggleon").image };
            toolbarList[1] = new GUIContent() { text = " Hide", image = EditorGUIUtility.IconContent("animationvisibilitytoggleoff").image };
            EditorGUI.BeginChangeCheck();
            var style = new GUIStyle(EditorStyles.miniButtonMid);
            style.fontSize = 10;
            style.fixedHeight = 32;
            var cur = GUILayout.Toolbar(master.currentState == UIAnimationModule.WidgetState.Show ? 0 : 1, toolbarList, style, GUILayout.ExpandHeight(true));
            GUI.backgroundColor = Color.white;
            if (EditorGUI.EndChangeCheck())
            {
                master.SetState(cur == 0 ? UIAnimationModule.WidgetState.Show : UIAnimationModule.WidgetState.Hide);
            }
            GUILayout.EndHorizontal();

            //EditorGUILayout.LabelField("Is Animating", master.isAnimating.ToString());
            //EditorGUILayout.ProgressBar(Vector2.one * 50, 50, 0 ,100, "test",Color.green, Color.white, false );
            var t = (master.animationCompleteCount >= master.animationItemCount) ? "Animation Complete" : $"Animating   {master.animationCompleteCount} / {master.animationItemCount}";
            var r = EditorGUILayout.GetControlRect();
            r.height = 12;

            EditorGUI.ProgressBar(r, Mathf.InverseLerp(0, master.animationItemCount, master.animationCompleteCount), t);
            GUI.color = Color.white;
        }


    }
#endif

    public partial class UIAnimationModule : MonoBehaviour
    {
        #region WIDGET VARIABLES
        public static Action PictureRefresh;

        public bool foloutEvents = false;
        public UnityEngine.Events.UnityEvent onWidgetShow;
        public UnityEngine.Events.UnityEvent onWidgetHide;
        public Action<RectTransform, AnimationState, AnimationElement> onWidgetAnimationState;

        public enum AnimationElement { Position = 0, Rotation = 1, Scale = 2, Fade = 3 }
        public enum AnimationState { In = 0, Out = 1 }
        public enum WidgetState { Hide = 0, Show = 1, Idle = 2 }
        public enum AnimationStyle { Basic = 0, Animator = 1, Custom = 2 }
        public AnimationStyle animationStyle = AnimationStyle.Basic;
        [SerializeField] internal bool debugShowWireframe = true;
        [SerializeField] internal int currentStage = 0;
        [SerializeField] internal Animator animator;
        [HideInInspector, NonSerialized] public WidgetState currentState = WidgetState.Idle;
        [SerializeField] internal string label = "[dummy]";
        [SerializeField] internal string lastJSONMessage = "";
        [SerializeField] internal string customWSMessage = "";
        [SerializeField] internal string customFakeReceiveWSMessage = "";
        [SerializeField] internal bool isAnimating = false;
        [SerializeField] internal bool canRefreshPicture = false;
        [SerializeField] public RawImage[] waitForPictureList;
        public RawImage[] waitForPictureList2;

        public string widgetName
        {
            get => this.label;
        }

        RectTransform rectTransform;

        #endregion

        #region ANIMATION
        // ----------------------------------------------------
        public List<AnimationItem> itemList = new List<AnimationItem>() { new AnimationItem() };
        public int animationCompleteCount = 0;
        public int animationItemCount = 0;
        public int animationIndex = 0;
        //[SerializeField] internal bool waitAnimationComplete = true;
        public Action<string> animationComplete;
        public int grid = 0;
        public float globalDelay = 0f;

#if UNITY_EDITOR
        public ReorderableList reorderableList;
#endif

        public void AddAnimationItem()
        {
            var animationItem = new AnimationItem();
            animationItem.Setup();
            itemList.Add(animationItem);
        }

        public void RemoveAnimationItem()
        {
#if UNITY_EDITOR
            if (reorderableList.index > -1) itemList.RemoveAt(reorderableList.index);
#endif
        }

        public void In()
        {
            if (!Application.isPlaying) return;
            //if (isAnimating && waitAnimationComplete) return;
            animationIndex = 0;
            StartAnimation();

            SetLock();
            SetRest();

            Canvas.ForceUpdateCanvases();
            foreach (var item in itemList)
            {
                if (item == null) continue;
                if (item.driven) item.In(this);
            }
            foreach (var item in itemList)
            {
                if (item?.canvasComponent != null)
                {
                    item.canvasComponent.blocksRaycasts = true;
                }
            }

            // Call In() on all child UIAnimationModules
            // var childModules = GetComponentsInChildren<UIAnimationModule>();
            // foreach (var childModule in childModules)
            // {
            //     if (childModule != this) // Avoid calling on self
            //     {
            //         childModule.In();
            //     }
            // }
        }

        public void Out()
        {
            if (!Application.isPlaying) return;
            //if (isAnimating) return;
            //if (isAnimating && waitAnimationComplete) return;
            animationIndex = 1;
            StartAnimation();
            Canvas.ForceUpdateCanvases();
            foreach (var item in itemList)
            {
                if (item.driven) item.Out(this);
            }
            foreach (var item in itemList)
            {
                if (item?.canvasComponent != null)
                {
                    item.canvasComponent.blocksRaycasts = false;
                }
            }

            // Call Out() on all child UIAnimationModules
            // var childModules = GetComponentsInChildren<UIAnimationModule>();
            // foreach (var childModule in childModules)
            // {
            //     if (childModule != this) // Avoid calling on self
            //     {
            //         childModule.Out();
            //     }
            // }
        }

        public void SetLock()
        {
            foreach (var item in itemList) item.Lock();
        }

        public void SetRest()
        {
            foreach (var item in itemList) item.SetRest();
        }

        public void PrepareAnimation()
        {
            currentState = WidgetState.Hide;
            animationIndex = 0;
            foreach (var item in itemList) item.PrepareAnimation(this);
        }

        void StartAnimation()
        {
            isAnimating = true;
            animationCompleteCount = 0;
            animationItemCount = itemList.Where(o => o.driven && o.driven.gameObject.activeSelf).Count();
            foreach (var item in itemList)
            {
                item.animationComplete = null;
                item.animationStart = null;
                if (!item.driven || !item.driven.gameObject.activeSelf) continue;
                item.animationComplete += OnComplete;
                item.animationStart += OnStart;
            }
        }

        void OnStart(RectTransform driven, AnimationState state, AnimationElement element)
        {
            onWidgetAnimationState?.Invoke(driven, state, element);
        }

        void OnComplete()
        {
            animationCompleteCount++;
            if (animationCompleteCount >= animationItemCount)
            {
                isAnimating = false;
                foreach (var item in itemList) item.Lock();
                SetRest();

                switch (animationIndex)
                {
                    case 0:
                        OnAnimationInComplete();
                        break;
                    case 1:
                        OnAnimationOutComplete();
                        break;
                }
            }
        }

        void OnAnimationInComplete()
        {
            animationComplete?.Invoke("in");
        }

        void OnAnimationOutComplete()
        {
            animationComplete?.Invoke("out");
            foreach (var item in itemList) item.ForceHide();
        }

        [Serializable]
        public class MotionItem
        {
            public bool isEnabled = true;
            public float delay = 0f;
            public Vector3 value;
            public float duration = 1f;
            public Ease ease = Ease.OutQuart;
            public AnimationCurve easeCurve = new AnimationCurve();

            public void SetupDefaults()
            {
                isEnabled = true;
                delay = 0f;
                duration = 1f;
                ease = Ease.OutQuart;
                easeCurve.AddKey(0, 0);
                easeCurve.AddKey(1, 1);
            }
        }

        [Serializable]
        public class FadeItem
        {
            public bool isEnabled = true;
            public float delay = 0f;
            public float value = 1f;
            public float duration = 1f;
            public Ease ease = Ease.OutQuart;
            public AnimationCurve easeCurve = new AnimationCurve();

            public void SetupDefaults()
            {
                isEnabled = true;
                delay = 0f;
                duration = 1f;
                ease = Ease.OutQuart;
                easeCurve.AddKey(0, 0);
                easeCurve.AddKey(1, 1);
            }
        }

        [Serializable]
        public class ScaleItem
        {
            public bool isEnabled = true;
            public float delay = 0f;
            public Vector3 value;
            public float duration = 1f;
            public Ease ease = Ease.OutQuart;
            public AnimationCurve easeCurve = new AnimationCurve();

            public void SetupDefaults()
            {
                isEnabled = true;
                delay = 0f;
                duration = 1f;
                ease = Ease.OutQuart;
                easeCurve.AddKey(0, 0);
                easeCurve.AddKey(1, 1);
            }
        }

        [Serializable]
        public class Item
        {
            public float delay = 0f;
            public MotionItem position = new MotionItem();
            public FadeItem fade = new FadeItem();
            public ScaleItem scale = new ScaleItem();
        }

        [Serializable]
        public class AnimationItem
        {
            public void Setup()
            {
                var itemIn = new Item();
                itemIn.position = new MotionItem() { value = new Vector3(0, 100, 0) };
                itemIn.position.SetupDefaults();
                itemIn.fade = new FadeItem();
                itemIn.fade.SetupDefaults();
                itemIn.fade.value = 1f;
                itemIn.scale = new ScaleItem() { value = new Vector3(0, 0, 0) };
                itemIn.scale.SetupDefaults();
                motionIn = itemIn;

                var itemOut = new Item();
                itemOut.position = new MotionItem() { value = new Vector3(0, -100, 0) };
                itemOut.position.SetupDefaults();
                itemOut.fade = new FadeItem() { value = 0f };
                itemOut.fade.SetupDefaults();
                itemOut.fade.value = 0f;
                itemOut.scale = new ScaleItem() { value = new Vector3(0, 0, 0) };
                itemOut.scale.SetupDefaults();
                motionOut = itemOut;
            }

            #region Variables
            public bool foldout = true;
            public RectTransform driven;
            [HideInInspector] public CanvasGroup canvasComponent;
            Sequence animationSequence;
            public float globalDelay = 0f;

            public Item motionIn;
            public Item motionOut;

            public Action<RectTransform, AnimationState, AnimationElement> animationStart;
            public Action animationComplete;
            public bool isLocked = false;
            private DrivenRectTransformTracker tracker = new DrivenRectTransformTracker();
            public Vector3 localRestPosition;
            public Vector3 localRestScale;
            #endregion

            public void ReleaseMove(UnityEngine.Object driver)
            {
                if (driven == null) return;

                tracker.Clear();
                tracker.Add(driver, driven, DrivenTransformProperties.All);
                isLocked = false;
            }

            public void Lock()
            {
                if (driven == null && !isLocked) return;
                tracker.Clear();
                if (driven)
                {
                    driven.anchoredPosition = localRestPosition;
                    driven.localScale = localRestScale;
                    if (driven.parent) LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)driven.parent);
                }
                isLocked = true;
            }

            public void ForceHide()
            {
                if (canvasComponent) canvasComponent.alpha = 0;
            }

            public void SetRest()
            {
                if (driven == null) return;
                localRestPosition = driven.anchoredPosition;
                localRestScale = driven.localScale;
            }

            public void PrepareAnimation(UnityEngine.Object driver)
            {
                if (driver == null) return;
                if (driven && driven.parent) LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)driven.parent);
                ForceHide();
            }

            public void In(UnityEngine.Object driver)
            {
                if (driven == null) return;

                if (driver != null) ReleaseMove(driver);
                if (animationSequence != null) animationSequence.Kill();
                animationSequence = DOTween.Sequence();
                animationSequence.Pause();

                var motion = motionIn;

                if (motion.position.isEnabled)
                {
                    driven.anchoredPosition = localRestPosition + motion.position.value;
                    if (motion.position.ease == Ease.Unset)
                    {
                        animationSequence.Join(driven.DOAnchorPos(localRestPosition, motion.position.duration).SetDelay(motion.position.delay).SetEase(motion.position.easeCurve).OnStart(() => animationStart?.Invoke(driven, AnimationState.In, AnimationElement.Position)));
                    }
                    else
                    {
                        animationSequence.Join(driven.DOAnchorPos(localRestPosition, motion.position.duration).SetDelay(motion.position.delay).SetEase(motion.position.ease).OnStart(() => animationStart?.Invoke(driven, AnimationState.In, AnimationElement.Position)));
                    }
                }

                if (motion.fade.isEnabled)
                {
                    if (canvasComponent)
                    {
                        canvasComponent.alpha = 0;
                        if (motion.fade.ease == Ease.Unset)
                        {
                            animationSequence.Join(canvasComponent.DOFade(motion.fade.value, motion.fade.duration).SetDelay(motion.fade.delay).SetEase(motion.fade.easeCurve).OnStart(() => animationStart?.Invoke(driven, AnimationState.In, AnimationElement.Fade)));
                        }
                        else
                        {
                            animationSequence.Join(canvasComponent.DOFade(motion.fade.value, motion.fade.duration).SetDelay(motion.fade.delay).SetEase(motion.fade.ease).OnStart(() => animationStart?.Invoke(driven, AnimationState.In, AnimationElement.Fade)));
                        }
                    }
                }
                else
                {
                    if (canvasComponent) canvasComponent.alpha = 1;
                }

                if (motion.scale.isEnabled)
                {
                    driven.localScale = localRestScale + motion.scale.value;
                    if (motion.scale.ease == Ease.Unset)
                    {
                        animationSequence.Join(driven.DOScale(localRestScale, motion.scale.duration).SetDelay(motion.scale.delay).SetEase(motion.scale.easeCurve).OnStart(() => animationStart?.Invoke(driven, AnimationState.In, AnimationElement.Scale)));
                    }
                    else
                    {
                        animationSequence.Join(driven.DOScale(localRestScale, motion.scale.duration).SetDelay(motion.scale.delay).SetEase(motion.scale.ease).OnStart(() => animationStart?.Invoke(driven, AnimationState.In, AnimationElement.Scale)));
                    }
                }

                animationSequence.SetDelay(motionIn.delay + globalDelay);
                animationSequence.OnComplete(() => animationComplete?.Invoke());
                animationSequence.Play();
            }

            public void Out(UnityEngine.Object driver)
            {
                if (driven == null) return;
                if (driver != null) ReleaseMove(driver);
                if (animationSequence != null) animationSequence.Kill();
                animationSequence = DOTween.Sequence();

                var motion = motionOut;

                if (motion.position.isEnabled)
                {
                    driven.anchoredPosition = localRestPosition;
                    if (motion.position.ease == Ease.Unset)
                    {
                        animationSequence.Join(driven.DOAnchorPos(localRestPosition + motion.position.value, motion.position.duration).SetDelay(motion.position.delay).SetEase(motion.position.easeCurve).OnStart(() => animationStart?.Invoke(driven, AnimationState.Out, AnimationElement.Position)));
                    }
                    else
                    {
                        animationSequence.Join(driven.DOAnchorPos(localRestPosition + motion.position.value, motion.position.duration).SetDelay(motion.position.delay).SetEase(motion.position.ease).OnStart(() => animationStart?.Invoke(driven, AnimationState.Out, AnimationElement.Position)));
                    }
                }

                if (motion.fade.isEnabled)
                {
                    if (canvasComponent)
                    {
                        canvasComponent.alpha = motionIn.fade.value;
                        if (motion.fade.ease == Ease.Unset)
                        {
                            animationSequence.Join(canvasComponent.DOFade(motion.fade.value, motion.fade.duration).SetDelay(motion.fade.delay).SetEase(motion.fade.easeCurve).OnStart(() => animationStart?.Invoke(driven, AnimationState.Out, AnimationElement.Fade)));
                        }
                        else
                        {
                            animationSequence.Join(canvasComponent.DOFade(motion.fade.value, motion.fade.duration).SetDelay(motion.fade.delay).SetEase(motion.fade.ease).OnStart(() => animationStart?.Invoke(driven, AnimationState.Out, AnimationElement.Fade)));
                        }
                    }
                }

                if (motion.scale.isEnabled)
                {
                    driven.localScale = localRestScale;
                    if (motion.scale.ease == Ease.Unset)
                    {
                        animationSequence.Join(driven.DOScale(localRestScale + motion.scale.value, motion.scale.duration).SetDelay(motion.scale.delay).SetEase(motion.scale.easeCurve).OnStart(() => animationStart?.Invoke(driven, AnimationState.Out, AnimationElement.Scale)));
                    }
                    else
                    {
                        animationSequence.Join(driven.DOScale(localRestScale + motion.scale.value, motion.scale.duration).SetDelay(motion.scale.delay).SetEase(motion.scale.ease).OnStart(() => animationStart?.Invoke(driven, AnimationState.Out, AnimationElement.Scale)));
                    }
                }

                animationSequence.OnComplete(() => animationComplete?.Invoke());
                animationSequence.SetDelay(motionOut.delay);
                animationSequence.Play();
            }
        }

        #endregion

        private void Awake()
        {
            this.label = this.label.ToLower();
            rectTransform = GetComponent<RectTransform>();

            foreach (var item in itemList)
            {
                if (item.driven)
                {
                    item.canvasComponent = item.driven.GetComponent<CanvasGroup>();
                    if (item.canvasComponent == null) item.canvasComponent = item.driven.gameObject.AddComponent<CanvasGroup>();
                }
            }
        }

        private void Start()
        {
            PrepareAnimation();
        }

        public void SetState(WidgetState state)
        {
            //if (isAnimating) return;
            //if (isAnimating && waitAnimationComplete) return;

            if (currentState == state) return;
            currentState = state;
            switch (state)
            {
                case WidgetState.Hide:
                    onWidgetHide?.Invoke();
                    if (animationStyle == AnimationStyle.Animator) animator?.CrossFade("Hide", 0.2f);
                    if (animationStyle == AnimationStyle.Basic) Out();
                    break;
                case WidgetState.Show:
                    onWidgetShow?.Invoke();
                    canRefreshPicture = false;
                    if (animationStyle == AnimationStyle.Animator) animator?.CrossFade("Show", 0.2f);
                    if (animationStyle == AnimationStyle.Basic) In();
                    break;
                case WidgetState.Idle:
                    if (animationStyle == AnimationStyle.Animator) animator?.CrossFade("Idle", 0.2f);
                    break;
            }
            //foreach (var widget in WidgetList) widget.OnChangeState(state);
            Canvas.ForceUpdateCanvases();
        }


#if UNITY_EDITOR
        public void CreateController()
        {
            if (animator.runtimeAnimatorController) return;
            var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath($"Assets/{name}.controller");
            var rootStateMachine = controller.layers[0].stateMachine;
            var stateHide = rootStateMachine.AddState("Hide");
            var stateShow = rootStateMachine.AddState("Show");
            var stateIdle = rootStateMachine.AddState("Idle");
            var clipHide = new AnimationClip();
            var clipShow = new AnimationClip();
            var clipIdle = new AnimationClip();
            clipHide.name = "Hide";
            clipShow.name = "Show";
            clipIdle.name = "Idle";
            stateHide.motion = clipHide;
            stateShow.motion = clipShow;
            stateIdle.motion = clipIdle;
            AssetDatabase.AddObjectToAsset(clipHide, stateHide);
            AssetDatabase.AddObjectToAsset(clipShow, stateShow);
            AssetDatabase.AddObjectToAsset(clipIdle, stateIdle);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(clipHide));
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(clipShow));
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(clipIdle));
            animator.runtimeAnimatorController = controller;
            EditorGUIUtility.PingObject(clipHide);
        }
#endif


    }
}

