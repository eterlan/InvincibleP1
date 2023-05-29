using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lib
{
    /// <summary>
    /// 跟框架无关的基础功能
    /// </summary>
    public static class BaseUtil
    {
        public static T DeepCopySerializable<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
        public static T DeepCopy<T>(this T obj)
        {
            if (obj == null)
            {
                return default(T);
            }

            Type type = obj.GetType();

            if (type.IsValueType || type == typeof(string))
            {
                return obj;
            }

            object copy = Activator.CreateInstance(type, nonPublic: true);

            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                var originalFieldValue = field.GetValue(obj);
                var copiedFieldValue   = DeepCopy(originalFieldValue);
                field.SetValue(copy, copiedFieldValue);
            }

            return (T)copy;
        }

        public static int GetFlagsEnabledCount<T>(T t) where T : System.Enum
        {
            var count = 0;
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                if (t.HasFlag(value))
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// 获得继承了指定基类的所有派生类
        /// </summary>
        /// <typeparam name="T">基类</typeparam>
        /// <returns></returns>
        public static Type[] GetAllDerivedClass<T>(string assemblyName) where T : class
        {
            var baseType = typeof(T);
            return Assembly.Load(assemblyName).GetTypes().Where(type => type != baseType && baseType.IsAssignableFrom(type)).ToArray();
        }

        public static List<GameObject> listForFind = new();
        public static bool FindRootTransformInScene(this Scene scene, string name, out Transform tr)
        {
            tr = null;
            scene.GetRootGameObjects(listForFind);
            for (var i = 0; i < listForFind.Count; i++)
            {
                if (name == listForFind[i].name)
                {
                    tr = listForFind[i].transform;
                    listForFind.Clear();
                    return true;
                }
            }

            Debug.LogWarning($"在场景: {scene.name}中找不到名为{name}的根节点, 请检查场景文件");
            listForFind.Clear();
            return false;
        }
        
        
        public static Transform FindRootTransformInAllScene(string objName)
        {
            Transform result = null;
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.FindRootTransformInScene(objName, out result))
                    break;
            }

            return result;
        }

        public static T ChangeAlpha<T>(this T g, float newAlpha)
            where T : Graphic
        {
            var color = g.color;
            color.a = newAlpha;
            g.color = color;
            return g;
        }
    }
}