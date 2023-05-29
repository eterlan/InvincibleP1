using System;
using UnityEngine;

namespace Lib
{
    [Serializable]
    public abstract class SaveManager<T> : IManager where T : PlayerSave
    {
        [SerializeReference]
        public T save;
        public SaveManager()
        {
            
        }
        public void OnAwake()
        {
            if (save == null)
            {
                Debug.Log("请在GameController上选择一个新的save类型来开启存档功能");
                return;
            }

            // TODO 如果扩展的话应该通过不同的按钮决定初始是读取还是新建
            // 初始化存档
            if (!HasLoad())
            {
                Save();
            }
        }
        
        public virtual void Save()
        {
            PlayerPrefs.SetString(nameof(PlayerSave), JsonUtility.ToJson(save));
        }

        public virtual bool TryLoad()
        {
            if (!HasLoad())
            {
                return false;
            }
            var saveJson = PlayerPrefs.GetString(nameof(PlayerSave));
            if (saveJson.IsNullOrEmpty())
            {
                return false;
            }
            JsonUtility.FromJsonOverwrite(saveJson, save);
            return true;
        }

        protected virtual bool HasLoad()
        {
            return PlayerPrefs.HasKey(nameof(PlayerSave));
        }
    }
}