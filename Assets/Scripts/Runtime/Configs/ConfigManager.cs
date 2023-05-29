using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Lib.Configs
{
    /// <summary>
    /// 统一的配置入口, 方便管理
    /// </summary>
    // [CreateAssetMenu(fileName = nameof(ConfigManager), menuName = "GameConfig/ConfigManager")]
    public class ConfigManager
    {
        private static readonly Lazy<ConfigManager> Lazy = new();

        public static ConfigManager instance => Lazy.Value;

        private          ConfigBase[]                 m_configs;
        private readonly Dictionary<Type, ConfigBase> m_configDict = new();

        public ConfigManager()
        {
            Init();
        }
        
        public static T GetConfig<T>() where T : ConfigBase
        {
            if (!instance.m_configDict.TryGetValue(typeof(T), out var config))
            {
                Debug.LogError($"无法找到名为: {typeof(T).Name}的配置");
                return null;
            }

            return (T)config;
        }

        private void Init()
        {
            LoadAllConfigs();
        }

        [Button]
        private void LoadAllConfigs()
        {
            m_configDict.Clear();
            m_configs = Resources.LoadAll<ConfigBase>("Config");
            foreach (var config in m_configs)
            {
                config.FillDictionary();
                m_configDict.Add(config.GetType(), config);
            }
        }
    }
}