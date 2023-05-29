using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Lib
{
    public abstract class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        [SuppressMessage("ReSharper", "InvertIf")]
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>(true);
                    if (instance == null)
                    {
                        var go = new GameObject(typeof(T).Name);
                        instance = go.AddComponent<T>();
                    }
                    DontDestroyOnLoad(instance.gameObject);
                }

                return instance;
            }
        }

        private void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            // 加载的时候如何发现已经有同伙了, 那就把自己干掉吧~
            if (instance == null)
            {
                instance = this as T;
            }
            else if(instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}