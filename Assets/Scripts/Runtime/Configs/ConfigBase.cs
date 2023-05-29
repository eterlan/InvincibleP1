using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Lib.Configs
{
    public abstract class ConfigBase : ScriptableObject
    {
        //public abstract void InitConfig();
        public abstract void FillDictionary();
        //public abstract void AddConfig();
    }
    
    public abstract class ConfigBase<T> : ConfigBase where T : ConfigItemBase, new()
    {
        /// <summary>
        /// 只应该让编辑器代码使用
        /// </summary>
        [Searchable]
        [SerializeField]
        [ValidateInput(nameof(ValidateConflict), "id冲突, 请换个id")]
        private List<T> configItems;

        /// <summary>
        /// 只应该让编辑器代码使用
        /// </summary>
        public List<T> GetAllItems => configItems;

        public bool TryGetItem(int key, out T item)
        {
            if (!m_configItemDict.TryGetValue(key, out item))
            {
                Debug.LogWarning($"找不到id为: {key} 的 {typeof(T).Name}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 为了统一性 只有一个配置的表就用第一个作为默认
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool GetDefaultItem(out T item)
        {
            item = default;
            if (GetAllItems == null)
            {
                CyLog.LogError($"{this} 找不到默认配置, 请检查");
                return false;
            }

            if (GetAllItems.Count == 0)
            {
                CyLog.LogError($"{this} 找不到默认配置, 请检查");
                return false;
            }

            item = GetAllItems[0];
            return item != null;
        }
        
        private Dictionary<int, T> m_configItemDict;

        // 当游戏中的时候把所有配置加载到字典中
        // 添加一个按钮能重新加载到字典中

        private bool TryFillDictionary()
        {
            if (GetAllItems == null)
                return false;

            m_configItemDict ??= new Dictionary<int, T>();
            m_configItemDict.Clear();
            foreach (var configItem in GetAllItems)
            {
                if (!m_configItemDict.TryAdd(configItem.id, configItem))
                    return false;
            } 

            return true;
        }
        
        
        #region ODIN

#if UNITY_EDITOR
        
        [Button("新增")]
        public virtual void AddConfig()
        {
            T newConfigItem;
            if (configItems.Count > 0)
            {
                var template = configItems[^1];
                newConfigItem = Sirenix.OdinInspector.Editor.Internal.FastDeepCopier.DeepCopy(template);
                newConfigItem.id++;
            }
            else
                newConfigItem = new T();

            configItems.Add(newConfigItem);
            EditorUtility.SetDirty(this);
        }

#endif
        
        [Button("填充数据")]
        public override void FillDictionary()
        {
            var msg = "";
            msg = TryFillDictionary() ? $"配置表: {this} 成功加载{m_configItemDict.Count}条数据" 
                : $"加载失败, 请查看配置表: {this}是否有冲突id";

            Debug.Log(msg);  
        }

        
        private bool ValidateConflict()
        {
            return TryFillDictionary(); 
        }

        #endregion
    }
}