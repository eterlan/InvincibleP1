using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lib
{
    public class EventManager 
    {
        private readonly Dictionary<uint, Action<EventArgs>> m_eventDict = new ();

        private static readonly Lazy<EventManager> Lazy = new();
        private static          EventManager       instance => Lazy.Value;

        // public static EventManager instance {
        //     get {
        //         if (!eventManager) {
        //             eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;
        //
        //             if (!eventManager) {
        //                 Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
        //             } else {
        //                 eventManager.m_eventDict ??= new Dictionary<Event, Action<EventArgs>>();
        //
        //                 //  Sets this to not be destroyed when reloading scene
        //                 DontDestroyOnLoad(eventManager);
        //             }
        //         }
        //         return eventManager;
        //     }
        // }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitializeOnEnterPlayMode()
        {
            instance.m_eventDict.Clear();
        }
        
        public static void AddListener(uint @event, Action<EventArgs> listener) 
        {
            if (instance.m_eventDict.TryGetValue(@event, out var thisEvent))
            {
                thisEvent                    += listener;
                instance.m_eventDict[@event] =  thisEvent;
            }
            else 
            {
                // Debug.Log($"找不到事件{@event}, 已添加进字典");
                thisEvent += listener;
                instance.m_eventDict.Add(@event, thisEvent);
            }
        }

        public static void RemoveListener(uint @event, Action<EventArgs> listener) {
            if (instance == null) return;
            if (!instance.m_eventDict.TryGetValue(@event, out var thisEvent))
            {
                Debug.LogError($"找不到事件{@event}, 删除监听者失败");
                return;
            }
            if (thisEvent == null)
            {
                instance.m_eventDict.Remove(@event);
                return;
            }
            
            thisEvent                    -= listener;
            instance.m_eventDict[@event] =  thisEvent;
        }

        public static void Invoke(uint @event, EventArgs message)
        {
            if (!instance.m_eventDict.TryGetValue(@event, out var thisEvent))
            {
                Debug.LogWarning($"事件{@event}没有监听者, 触发监听者失败");
                return;
            }

            if (thisEvent == null)
            {
                instance.m_eventDict.Remove(@event);
                return;
            }

            thisEvent.Invoke(message);
        }
    }
}