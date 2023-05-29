using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lib.Editor.Tool
{
    // TODO 不知道为什么删除的时候用快捷键会报错, 用按钮就没事..
    public class FastSLObjectWindow : EditorWindow
    {
        private SerializedObject windowSO;
        private bool             showHeader;
        private Vector2          scrollPos;
        private GUIStyle         wrapStyle;
        private Rect             historyPos;

        // list
        [SerializeField]
        private List<SelectionObjs> SelectionsSaveSlots = new List<SelectionObjs>();
        private ReorderableList roList;


        // Error
        private       bool   showNullError;
        private       int    labelWidth = 30;
        private const string NULL_ERROR = "请选中任意希望保存的物体";


        private bool hasResize;
    
    
        // FastKey
        private bool saveIsPressed, deleteIsPressed;

        // 设置相关
        [SerializeField] private bool autoFocus = true, openInspector = true, keepPrevSelectedObjInInspector, autoHeightLight = true;
        [SerializeField]
        private int multiObjsShowNamesCount = 2;

        [SerializeField] private Vector2 windowSize = new Vector2(300, 500);

        [MenuItem("Tools/Cy/打开保存选中工具(如果已打开就保存) &T")]
        public static void CreateFastSLObjectWindow()
        {
            var isOpen = HasOpenInstances<FastSLObjectWindow>();
            if (isOpen)
            {
                var window = GetWindow<FastSLObjectWindow>();
                window.position = new Rect(window.position.x, window.position.y, window.windowSize.x, window.windowSize.y);
                window.AddToList();
            }
            else
            {
                GetWindow<FastSLObjectWindow>();
                Event.current.Use();
            }
        }

        private void OnEnable()
        {
            Init().Forget();
        }

        private async UniTaskVoid Init()
        {
            await UniTask.WaitWhile(() => EditorApplication.isCompiling);
            Load();
            windowSO = new SerializedObject(this); 
            roList = new ReorderableList(windowSO, windowSO.FindProperty(nameof(SelectionsSaveSlots)))
            {
                drawElementCallback = OnDrawElement,
                drawHeaderCallback  = rect => EditorGUI.LabelField(rect, "选中物体列表"),
                // roList.onAddCallback = list => AddToList();
                displayAdd            = false,
                displayRemove         = false,
                onSelectCallback      = OnSelectCallback,
                elementHeightCallback = ChangeListElementHeight
            };
            // roList.onChangedCallback = list => Save();
            Undo.undoRedoPerformed += OnChangeSaveSlots;
        }
        
        
        private float ChangeListElementHeight(int index)
        {
            // roList.DoLayoutList();
            return SelectionsSaveSlots[index].SlotHeight;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnChangeSaveSlots;
            // OnChangeSaveSlots();
        }

        private void OnChangeSaveSlots()
        {
            Save();
            GUI.changed = true;
            Repaint();
            GUIUtility.hotControl = 0;
        }
        // 简而言之, 就是
        bool HasResize() => Mathf.Abs(historyPos.width - position.width) > Mathf.Epsilon || Mathf.Abs(historyPos.height - position.height) > Mathf.Epsilon;

        private void OnGUI()
        {
            hasResize          = HasResize();
            wrapStyle          = GUI.skin.label;  
            wrapStyle.wordWrap = true;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            Introduction();
            Function();
            EditorGUILayout.EndScrollView();
        
            void Introduction()
            {
                EditorGUIUtility.labelWidth = 70;
                showHeader                  = EditorGUILayout.BeginFoldoutHeaderGroup(showHeader, "保存历史选中物体");
                if (showHeader)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("说明", "快速保存和读取选中的物体, 方便之后一键选取,", wrapStyle);
                    EditorGUILayout.Separator();
                
                    EditorGUILayout.LabelField("选中后: ");
                    EditorGUI.indentLevel++;
                    autoHeightLight = EditorGUILayout.ToggleLeft("自动高亮", autoHeightLight);
                    EditorGUI.BeginChangeCheck();
                    autoFocus = EditorGUILayout.ToggleLeft("自动注视", autoFocus);
                    if (EditorGUI.EndChangeCheck() && autoFocus)
                    {
                        keepPrevSelectedObjInInspector = false;
                    }
                    EditorGUI.BeginChangeCheck();
                    keepPrevSelectedObjInInspector = EditorGUILayout.ToggleLeft("不改变固定检视器", keepPrevSelectedObjInInspector);
                    if (EditorGUI.EndChangeCheck() && keepPrevSelectedObjInInspector)
                    {
                        autoFocus = false;
                    }
                    openInspector = EditorGUILayout.ToggleLeft("打开独立检视器", openInspector);

                    EditorGUI.indentLevel--;
                    EditorGUILayout.Separator();
                
                    EditorGUILayout.LabelField("个性化: ");
                    EditorGUI.indentLevel++;
                    EditorGUIUtility.labelWidth = 150;
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.BeginChangeCheck();
                    multiObjsShowNamesCount = EditorGUILayout.IntSlider("多选物体名字展示最大数量", multiObjsShowNamesCount, 1, 5);
                    if (EditorGUI.EndChangeCheck()) 
                        GUI.changed = true;
                    
                    windowSize = EditorGUILayout.Vector2Field("检视器窗口大小", windowSize);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Separator();
                    
                    EditorGUILayout.LabelField("快捷键");
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("打开窗口/保存新选中物体到储存槽", "alt + T");
                    EditorGUILayout.LabelField("关闭窗口", "Esc");
                    //EditorGUILayout.LabelField("删除选中储存槽", "ctrl + T");
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;    
                
                
                    EditorGUILayout.LabelField("",GUI.skin.horizontalSlider);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.Separator();
            }

            void Function()
            {
                // var e = Event.current;
                // var lmbIsPressed = e.button == 0 && e.type == EventType.MouseUp;
                //
                // if (lmbIsPressed)
                // {
                //     EditorUtil.InspectObject(SelectionsSaveSlots[roList.index].objs[0], false);
                // }
                var e            = Event.current;
                var escIsPressed = e.type == EventType.KeyUp && e.keyCode == KeyCode.Escape;
                //var delIsPressed = e.type == EventType.KeyUp && e.control && e.keyCode == KeyCode.T;

                if (escIsPressed)
                {
                    Close();
                    return;
                }


                EditorGUIUtility.labelWidth = labelWidth;
                if (windowSO == null)
                {
                    return;
                }
                windowSO.Update();

                if (GUILayout.Button("读取"))
                {
                    Load();
                }
                
                if (GUILayout.Button("保存当前选中物体")) 
                    AddToList();

                if (GUILayout.Button("删除储存槽物体"))
                    RemoveFromList();
    
                if (showNullError) EditorGUILayout.HelpBox(NULL_ERROR, MessageType.Warning);

                EditorGUILayout.LabelField("",GUI.skin.horizontalSlider);
                roList.DoLayoutList();
                windowSO.ApplyModifiedProperties();
            }
        }

        private void OnSelectCallback(ReorderableList list)
        {
            // var selectionObjs = list.serializedProperty.GetArrayElementAtIndex(list.index);
            // var selections = selectionObjs.FindPropertyRelative("selections");
            var prevSelected    = Selection.objects;
            var currentSelected = SelectionsSaveSlots[list.index].objs; 
        
            Selection.objects = currentSelected.ToArray();
            var e = Event.current;

            if (openInspector && e.clickCount == 2)
            {
                var inspector = EditorUtil.InspectObject(SelectionsSaveSlots[roList.index].objs[0], keepPrevSelectedObjInInspector);
                EditorUtil.SetPosition(inspector.GetType(), inspector);
            }
            if (autoHeightLight)
            {
                foreach (var obj in currentSelected)
                {
                    EditorGUIUtility.PingObject(obj);
                }
            }

            if (autoFocus)
            {
                SceneView.lastActiveSceneView.FrameSelected();
            }

            // 这个应该在执行完前面所有选项之后再调用
            if (keepPrevSelectedObjInInspector)
            {
                Selection.objects = prevSelected.ToArray();
            }
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var selectionsObjs = roList.serializedProperty.GetArrayElementAtIndex(index);
            var selections     = selectionsObjs.FindPropertyRelative(SelectionObjs.LIST_NAME);
            var labelRect      = new Rect(rect.x, rect.y, rect.width / 4, rect.height);
            var objRect        = new Rect(rect.x, rect.y, rect.width, rect.height);
            if (objRect.width < 0)
            {
                return;
            }
            if (hasResize)
            {
                hasResize  = false;
                historyPos = position; 
                //EditorUtility.SetDirty(this);
                GUI.changed = true;
            }
            objRect = EditorGUI.PrefixLabel(objRect, new GUIContent($"{index.ToString()}"));
        
            // 一个物体时使用Obj Field
            var firstObjInSlot = selections.GetArrayElementAtIndex(0);
            if (firstObjInSlot.objectReferenceValue == null)
            {
                return;
            }
        
            if (selections.arraySize == 1)
            {
                //EditorGUI.LabelField(labelRect, new GUIContent($"{index.ToString()}"));
                SelectionsSaveSlots[index].SlotHeight = 17;
                EditorGUI.ObjectField(objRect, firstObjInSlot, GUIContent.none);
            }
            else
            {
                var sb = new StringBuilder();
                sb.Append(firstObjInSlot.objectReferenceValue.name);
                var i = 1;
                for (; i < Math.Min(selections.arraySize, multiObjsShowNamesCount); i++)
                {
                    sb.Append(" 和 ");
                    var obj = selections.GetArrayElementAtIndex(i);
                    sb.Append(obj.objectReferenceValue.name);
                } 

                if (selections.arraySize > multiObjsShowNamesCount)
                {
                    sb.Append(" ...");
                }

                var content = new GUIContent(sb.ToString());
                var height  = wrapStyle.CalcHeight(content, objRect.width);
                SelectionsSaveSlots[index].SlotHeight = height;
                objRect.height                        = height;
                EditorGUI.LabelField(objRect, content, wrapStyle);
            }
        }

        public void AddToList()
        {
            Undo.RecordObject(this, nameof(SelectionsSaveSlots));
            var noSelection = Selection.objects.Length < 1;
            showNullError = noSelection;
            if (noSelection)
                return;
        
            SelectionsSaveSlots.Add(new SelectionObjs(Selection.objects));
            OnChangeSaveSlots();
        }

        private void RemoveFromList()
        {
            Undo.RecordObject(this, nameof(SelectionsSaveSlots));
            if (roList.index == -1) roList.index = roList.count - 1;
            SelectionsSaveSlots.RemoveAt(roList.index);
            roList.index -= 1;
            OnChangeSaveSlots();
        }

        private void Save()
        {
            //Debug.Log("Changed & Save");
            ToolSaveData.TrySaveData(nameof(FastSLObjectWindow), this);
        }

        private void Load()
        {
            Object fastSLObjectWindow = this;
            if (!ToolSaveData.TryGetData(nameof(FastSLObjectWindow), ref fastSLObjectWindow))
                return;
            
            SelectionsSaveSlots = ((FastSLObjectWindow)fastSLObjectWindow).SelectionsSaveSlots;
            for (var index = 0; index < SelectionsSaveSlots.Count; index++)
            {
                var saveSlot = SelectionsSaveSlots[index];
                if (saveSlot.instanceReference == null)
                {
                    Debug.LogWarning($"第{index}号槽位物体引用为空");
                    continue;
                }

                for (var i = 0; i < saveSlot.instanceReference.Count; i++)
                {
                    var instanceRef = saveSlot.instanceReference[i];
                    saveSlot.objs[instanceRef.index] = EditorUtility.InstanceIDToObject(instanceRef.instanceID);
                }
            }
        }
    } 

    /// <summary>
    /// 代表一个储存格的一堆物体
    /// 序列化不支持嵌套List, 必须这样套一层
    /// </summary>
    [Serializable]
    public class SelectionObjs
    {
        public const string LIST_NAME = nameof(objs);
        [SerializeReference]
        public List<Object> objs;
        [SerializeField]
        public List<Reference> instanceReference = new List<Reference>();
    
        [SerializeField]
        public float slotHeight = 17;
        public SelectionObjs(Object[] selections)
        {
            objs = selections.ToList();
            for (var i = 0; i < selections.Length; i++)
            {
                var obj = selections[i];
                if (obj is GameObject)
                {
                    instanceReference.Add(new Reference(i, obj.GetInstanceID()));
                }
            }
        }
    
        [Serializable]
        public class Reference
        {
            public int index;
            public int instanceID;

            public Reference(int index, int instanceID)
            {
                this.index      = index;
                this.instanceID = instanceID;
            }
        }

        public float SlotHeight
        {
            get => slotHeight;
            set => slotHeight = value;
        }
    }
}