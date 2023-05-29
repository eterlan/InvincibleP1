using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lib
{
    public class FeatureContainer : MonoBehaviour
    {
        [SerializeReference]
        public List<Feature> features;
        //public  List<IValue>             values;
        private Dictionary<Type, IValue> m_valuesDict = new();

        private void Update()
        {
            for (var i = 0; i < features.Count; i++)
            {
                if (features[i].enabled)
                {
                    features[i].Update();
                }
            }
        }

        private void FixedUpdate()
        {
            for (var i = 0; i < features.Count; i++)
            {
                features[i].FixedUpdate();
            }
        }

        private void OnEnable()
        {
            for (var i = 0; i < features.Count; i++)
            {
                features[i].OnEnabled(this);
            }
        }

        private void OnDisable()
        {
            for (var i = 0; i < features.Count; i++)
            {
                features[i].OnDisabled();
            }
        }

        private void Start()
        {
            for (var i = 0; i < features.Count; i++)
            {
                features[i].Start();
                if (features[i].oneShot)
                {
                    features.RemoveAt(i);
                }
            }
        } 

        public IValue GetOrAddValue(Type type)
        {
            if (!m_valuesDict.TryGetValue(type, out var iValue))
            {
                iValue = (IValue)Activator.CreateInstance(type);
                m_valuesDict.Add(type, iValue);
            }

            return iValue;
        }
        
        public T GetOrAddValue<T>() where T : IValue, new()
        {
            if (!m_valuesDict.TryGetValue(typeof(T), out var iValue))
            {
                iValue = new T();
                m_valuesDict.Add(typeof(T), iValue);
            }

            return (T)iValue;
        }
        
        public bool TryGetValue<T>(out T value) where T : IValue
        {
            value = default;
            if (!m_valuesDict.TryGetValue(typeof(T), out var iValue))
            {
                Debug.LogError($"获取数值: {typeof(T).Name}失败, 该值未添加到{gameObject}上");
                return false;
            }

            value = (T)iValue;
            return true;
        }
    }
}