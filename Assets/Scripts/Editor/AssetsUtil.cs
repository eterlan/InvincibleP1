using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lib.Editor
{
    public static class AssetsUtil
    {
        /// <summary>
        /// 返回选中的Asset或者文件夹的目录名字, 以 Assets/ 开头
        /// </summary>
        /// <returns></returns>
        public static string GetSelectedFolderPath(bool full)
        {
            //TEST this https://answers.unity.com/questions/472808/how-to-get-the-current-selected-folder-of-project.html
            var path   = "";
            var assets = Selection.GetFiltered<Object>(SelectionMode.Assets);
            foreach (var asset in assets)
            {
                path = AssetDatabase.GetAssetPath(asset);
                if (path.IsNullOrEmpty())
                    continue;
                // 如果选中的是目录
                if (Directory.Exists(path))
                {
                    Debug.Log($"选中目录为: {path}");
                    break;
                }
                // 如果选中的是asset
                if (File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    Debug.Log($"SelectedDirectoryName: {path}");
                    break;
                }
            }

            if (path == "")
            {
                throw new Exception("请选择文件夹");
            }

            if (full)
            {
                path = Application.dataPath.Replace("Assets", string.Empty) + path;
            }
            return path;
        }

        /// <summary>
        /// 复制一个Prefab Asset并生成一个Linked Prefab 实例
        /// </summary>
        /// <param name="originPrefabName">原Prefab名字</param>
        /// <param name="newPrefabName">新Prefab名字</param>
        /// <param name="pathInResourceFolder">原Prefab目录</param>
        /// <returns></returns>
        public static GameObject DuplicatePrefab(string originPrefabName, string newPrefabName, string pathInResourceFolder)
        {
            var originPath = Path.Combine(pathInResourceFolder, originPrefabName + ".prefab");
            var newPath    = Path.Combine(pathInResourceFolder, newPrefabName    + ".prefab");
            if (!File.Exists(originPath))
            {
                throw new Exception($"指定位置'{originPath}'没找到Prefab, 请确认prefab是否处于指定资源文件夹目录中");
            }
            if (File.Exists(newPath))
            {
                throw new Exception($"指定位置'{newPath}'已有Prefab Asset生成, 跳过生成以防覆盖");
            }
            var success = AssetDatabase.CopyAsset(originPath, newPath);
            Debug.Log($"Prefab copy {(success ? $"Success, '{newPath}'" : "Failed")}");
            if (!success)
                throw new Exception("Prefab copy failed");

            var newPrefabPath     = Path.Combine(pathInResourceFolder, newPrefabName + ".prefab");
            var newPrefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(newPrefabPath));
            return newPrefabInstance;
        }
    }
}