using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lib.Editor
{
    public static class EditorUtil
    {
        //透明淡黄色
        public static Color drag = new Color(0.961f, 0.91f, 0.78f, 0.2f);
        // 淡黄色
        public const string YELLOW = "FCEB7720";
        // 淡橙色
        public const  string H_ORANGE = "F0B67F20";
        public static Color  orange   = new Color(240, 182, 127, 1);

        // 奶白色
        public const string WHITE = "FCF7F830";
        // 暖粉色
        public const  string H_WARM_PINK = "#FAD5C520";
        public static Color  warmPink    = new Color(250, 213, 197, 255);
        // 灰绿色
        public const  string H_GREY_GREEN = "C5DECD30";
        public static Color  greyGreen    = new Color(197, 222, 205, 255);

        public static void SetPosition(Type windowType, EditorWindow window, Vector2 position = default, Vector2 size = default)
        {
            var allWindows = (Resources.FindObjectsOfTypeAll(windowType) as EditorWindow[])!.Where(w => !w.docked).ToArray();
            if (allWindows.Length < 2)
                return;
            var firstWindowPos = allWindows[1].position;
            {
                var newPos = new Rect();
                if (position == default)
                {
                    newPos   =  new Rect(firstWindowPos);
                    newPos.x += newPos.width;
                }
            
                window.position = newPos;

            }
        }
    
        /// <summary>
        /// 打开独立的检视器窗口
        /// </summary>
        public static EditorWindow InspectObject(Object obj, bool keepPrevSelection)
        {
            var inspectorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");

            //var inspectorType = Type.GetType("UnityEditor.InspectorWindow, UnityEditor.CoreModule.dll");
            var instance = ScriptableObject.CreateInstance(inspectorType) as EditorWindow;
            instance.Show();

            var lastSelectedObj = Selection.activeObject;
            Selection.activeObject = obj;
            var lockProperty = inspectorType.GetProperty("isLocked");
            lockProperty.GetSetMethod().Invoke(instance, new object[] { true });

            if (keepPrevSelection) Selection.activeObject = lastSelectedObj;
            return instance;
        }
    
    
        public static bool IsFileLocked(FileInfo file)
        {
            try
            {
                using var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                stream.Close();
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }
    
        public static bool TimeUp(double startTime, double duration)
        {
            var timeElapsed = EditorApplication.timeSinceStartup - startTime;
            return timeElapsed > 0 && timeElapsed < duration;
        }
        public static bool ContainsChinese(string input)
        {
            return Regex.IsMatch(input, "[\u4e00-\u9fa5]");
        }
        public static string GetSourceFilePath([CallerFilePath] string sourceFileName = "") => sourceFileName;
    
        /// <summary>
        /// 自动转成Excel条目格式并复制到剪贴板
        /// </summary>
        /// <param name="input"></param>
        public static void ToClipBoard4Excel(this List<string> input)
        {
            var sb = new StringBuilder();
            foreach (var v in input)
            {
                sb.Append(v);
                sb.Append('\t');
            }
            var output = sb.ToString().TrimEnd('\t');
            GUIUtility.systemCopyBuffer = output;
        }

    
        public static Vector2 GetMousePosInWorld()
        {
            var rawMousePos = Event.current.mousePosition;
            var screenPos   = HandleUtility.GUIPointToScreenPixelCoordinate(rawMousePos);
            return SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(screenPos);
        }

        public static Vector2Int RoundVector2(Vector2 origin) => new Vector2Int(Mathf.RoundToInt(origin.x), Mathf.RoundToInt(origin.y));

        // public static bool Same<TSource>(this IEnumerable<TSource> source, )
    }
}