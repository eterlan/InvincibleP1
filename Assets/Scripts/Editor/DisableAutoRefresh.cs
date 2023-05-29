using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Lib.Editor
{
    [InitializeOnLoad]
    public class DisableScripReloadInPlayMode
    {
        // static DisableScripReloadInPlayMode()
        // {
        //     EditorApplication.playModeStateChanged
        //         += OnPlayModeStateChanged;
        // }
        //
        // static void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        // {
        //     switch (stateChange) {
        //         case (PlayModeStateChange.EnteredPlayMode): {
        //             EditorApplication.LockReloadAssemblies(); 
        //             Debug.Log ("Assembly Reload locked as entering play mode");
        //             break;
        //         }
        //     }
        // }
        //
        // [MenuItem("Tools/Cy/Compile &R")]
        // public static void Compile()
        // {
        //     EditorApplication.UnlockReloadAssemblies();       
        //     EditorUtility.RequestScriptReload(); 
        // }
    
    }
}