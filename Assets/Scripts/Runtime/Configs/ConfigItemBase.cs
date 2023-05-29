using System;
using UnityEngine;

namespace Lib.Configs
{
    [Serializable]
    public abstract class ConfigItemBase : IIndex, IName
    {
        [field: SerializeField]
        public int id { get; set; }
        [field: SerializeField]
        public virtual string name { get; set; }
    }
}