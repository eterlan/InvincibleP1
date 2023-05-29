using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lib
{
    public static class GameObjectEx
    {
        /// <summary>
        /// 编辑器用
        /// </summary>
        /// <param name="target"></param>
        public static void DestroyChildrenImmediately(this Transform target)
        {
            for (var i = target.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(target.GetChild(i).gameObject);
            }
        }
    
        public static void DestroyChildren(this Transform target)
        {
            for (var i = target.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(target.GetChild(i).gameObject);
            }
        }

        public static void Reset(this Transform transform)
        {
            transform.position   = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.rotation   = Quaternion.identity;
        }
    
        public static void ResetLocal(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localScale    = Vector3.one;
            transform.localRotation = Quaternion.identity;
        }
    
        /// <summary>
        /// 对于指定Transform的所有子物体, 如果满足predicate, 会对第一个子物体执行操作
        /// </summary>
        /// <param name="t">指定父物体</param>
        /// <param name="predicate">条件</param>
        /// <param name="action">操作</param>
        public static void ModifyNestedChildrenIf(this Transform t, Predicate<Transform> predicate, Action<Transform> action)
        {
            var counter = 0;
            for (var i = 0; i < t.childCount; i++)
            {
                var child = t.GetChild(i);
                if (predicate(child))
                {
                    action(child);
                    counter++;
                }

                ModifyNestedChildrenIf(child, predicate, action);
            }

            if (counter == 0)
            {
                Debug.LogWarning("所有的子物体都不符合条件");
            }
        }

        /// <summary>
        /// 对于指定Transform的所有子物体, 对于满足predicate的第一个子物体执行action
        /// </summary>
        /// <param name="t">指定父物体</param>
        /// <param name="predicate">条件</param>
        /// <param name="action">操作</param>
        public static void ModifyFirstNestedChildrenIf(this Transform t, Predicate<Transform> predicate, Action<Transform> action)
        {
            for (var i = 0; i < t.childCount; i++)
            {
                var child = t.GetChild(i);
                if (predicate(child))
                {
                    action(child);
                    return;
                }

                ModifyFirstNestedChildrenIf(child, predicate, action);
            }

            Debug.LogWarning("没有一个子物体符合条件");
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            if (!go.TryGetComponent(out T component)) 
                component = go.AddComponent<T>();

            return component;
        }

        public static List<T> GetComponentsOnlyInChildren<T>(this GameObject go, bool includeInactive = false) where T : Component
        {
            var childrenC = new List<T>();
            foreach (var c in go.GetComponentsInChildren<T>(includeInactive))
            {
                if (c.gameObject != go) childrenC.Add(c);
            }

            return childrenC;
        }

        
        public static async UniTaskVoid DelayDestroy(this GameObject go, int frame = 4)
        {
            go.SetActive(false);
            await UniTask.DelayFrame(frame);
            CyLog.Log("RealDestroy");
            Object.Destroy(go);
        }

        // public static List<Transform> DirectChildren(this Transform transform)
        // {
        //     var childCount = transform.childCount;
        //     var result     = new List<Transform>(childCount);
        //     for (var i = 0; i < childCount; i++)
        //     {
        //         result.Add(transform.GetChild(i));
        //     }
        //
        //     return result;
        // }
    }
}
