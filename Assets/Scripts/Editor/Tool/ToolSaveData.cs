using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lib.Editor.Tool
{
    [CreateAssetMenu(fileName = nameof(ToolSaveData), menuName = "Editor/ToolSaveData")]
    public class ToolSaveData : ScriptableObject
    {
        public const string EDITOR_SAVE_DATA_PATH = "Assets/Settings/ToolSaveData.asset";
        public DictionaryOfToolData dict;

        public static void TrySaveData(string key, Object obj)
        {
            var json     = EditorJsonUtility.ToJson(obj);  
            var saveData = AssetDatabase.LoadAssetAtPath<ToolSaveData>(EDITOR_SAVE_DATA_PATH);
            saveData.dict[key] = json;
            EditorUtility.SetDirty(saveData);
            AssetDatabase.SaveAssets(); 
        }

        public static bool TryGetData(string key, ref Object obj)
        {
            var asset    = AssetDatabase.LoadAssetAtPath<ScriptableObject>(EDITOR_SAVE_DATA_PATH);
            var saveData = (ToolSaveData)asset;
            var success  = saveData.dict.TryGetValue(key, out var json);
            if (success)
            { 
                EditorJsonUtility.FromJsonOverwrite(json, obj);
                return true;
            }

            Debug.LogWarning($"读取编辑器数据失败, 没找到key: {key}");
            return false;
        }
    }

    [Serializable]
    public class SerializedDictionary
    {
        public List<ToolData> toolDataList;

        public bool TryGet(string key, out string data)
        {
            data = null;
            var toolData = toolDataList.Find(t => t.key == key);
            if (toolData != null)
            {
                data = toolData.value;
                return true;
            }
            return false;
        }

        public bool TrySet(string key, string value)
        {
            if (toolDataList.Exists(data => data.key == key))
            {
                Debug.LogWarning("已存在同key的值, 请换一个");
                return false;
            }
            toolDataList.Add(new ToolData(key, value));
            return true;
        }
    }

    [Serializable]
    public class ToolData
    {
        public string key;
        public string value;

        public ToolData(string key, string value)
        {
            this.key   = key;
            this.value = value;
        }
    }
    
    [Serializable] 
    public class DictionaryOfToolData : SerializableDictionary<string, string> {}
    
    

}