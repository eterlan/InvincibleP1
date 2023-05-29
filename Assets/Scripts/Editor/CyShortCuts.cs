using System;
using UnityEditor;
using UnityEngine;

namespace Lib.Editor
{
    public class CyShortCuts : UnityEditor.Editor
    {
        /// <summary>
        /// 全局.选择子物体, 如果没有子物体选择根节点
        /// </summary>
        [MenuItem("GameObject/SelectRoot %#R")]
        static void SelectRoot()
        {
            if (Selection.activeGameObject.transform.childCount != 0)
            {
                //Selection.activeGameObject = Selection.activeGameObject.transform.Find("Root").gameObject;
                Selection.activeGameObject = Selection.activeGameObject.transform.GetChild(0).gameObject;
                if (Selection.activeGameObject == null)
                {
                    Debug.LogWarning("选中物体没有子物体");
                    return;
                }
                // var window = EditorWindow.focusedWindow;
                // var a = SceneView.sceneViews;
                // Debug.Log(EditorWindow.focusedWindow);
                return;
            }
            //Selection.activeGameObject = PrefabUtility.GetNearestPrefabInstanceRoot(Selection.activeGameObject);
            Selection.activeGameObject = Selection.activeGameObject.transform.root.gameObject;
            var hierarchy = Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor.dll");
            EditorWindow.FocusWindowIfItsOpen(hierarchy);
        }

        /// <summary>
        /// 全局.方便开启动画录制模式
        /// </summary>
        [MenuItem("GameObject/Animation/Record %R")]
        private static void Record()
        {
            EditorWindow.FocusWindowIfItsOpen<AnimationWindow>();
            var animWindow = EditorWindow.GetWindow<AnimationWindow>();
            animWindow.recording = !animWindow.recording;
        }

        /// <summary>
        /// 主要作用是配合alt+,/.切换到下/上一关键帧, 快速k动画
        /// </summary>
        [MenuItem("GameObject/Animation/Global Play %SPACE")]
        private static void GlobalPlay()
        {
            EditorWindow.FocusWindowIfItsOpen<AnimationWindow>();
            var animWindow = EditorWindow.GetWindow<AnimationWindow>();
            animWindow.playing = !animWindow.playing;
        }
    
        /// <summary>
        /// 选中物体的同级创建空物体
        /// </summary>
        [MenuItem("GameObject/CreateEmpty &E")]
        public static void CreateEmptyGO()
        {
            var go = Instantiate(new GameObject(), Selection.activeGameObject.transform.parent);
            Undo.RegisterCreatedObjectUndo(go, "Create Empty");
        }
    }
}