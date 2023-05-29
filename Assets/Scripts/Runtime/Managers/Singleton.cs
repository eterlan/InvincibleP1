using System;
using UnityEngine;

namespace Lib
{
    public class Singleton<T> where T : Singleton<T>
    {
        private static readonly Lazy<T> Lazy = new ();

        public static T instance => Lazy.Value;
        
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitializeOnEnterPlayMode()
        {
            Debug.Log("InitializeOnEnterPlayMode");
            instance.BeforeAwake();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitBeforeStart()
        {
            instance.BeforeStart();
        }
        protected virtual void BeforeAwake()
        {
            Debug.Log($"初始化{typeof(T).Name}");
        }

        protected virtual void BeforeStart()
        {
            
        }
        
    }
}