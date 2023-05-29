using UnityEditor;
using UnityEngine;

namespace Lib
{
    static class FullscreenShortcut
    {
#if UNITY_EDITOR
        [MenuItem("Window/MaximizeCurrentWindow _F11")]
        static void ToggleCurrentWindowMaximized()
        {
            var windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            foreach (var window in windows)
            {
                if (window.titleContent.text == "Game")
                {
                    window.maximized = !window.maximized;
                    return;
                }
            }
            //var window = UnityEditor.EditorWindow.focusedWindow;
            // if (window == null)
            //     return;
            // window.maximized = !window.maximized;
        }
#endif
    }
}