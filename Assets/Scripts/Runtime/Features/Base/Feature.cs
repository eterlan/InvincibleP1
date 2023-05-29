using System;
using UnityEngine;

namespace Lib
{
    [Serializable]
    public abstract class Feature
    {
        public    bool             enabled = true;
        protected FeatureContainer owner;
        public    bool             oneShot;
        
        protected internal virtual void OnEnabled(FeatureContainer owner)
        {
            this.owner = owner;
            //enabled   = Filter();
            if (!enabled)
            {
                Debug.LogWarning($"Feature: {GetType().Name} is disabled because of filtered");
            }
        }

        protected internal virtual void OnDisabled(){}

        // protected internal virtual void InternalUpdate()
        // {
        //     if (Filter())
        //     {
        //         Update();
        //     }
        // }
        //
        // protected internal virtual void InternalFixedUpdate()
        // {
        //     if (Filter())
        //     {
        //         FixedUpdate();
        //     }
        // }


        protected virtual bool Filter()
        {
            return true;
        }
        
        protected internal virtual void Update()
        {
            
        }
        protected internal virtual void FixedUpdate(){}
        protected internal virtual void Start(){}
    }

    [Serializable]
    public abstract class Feature<T1> : Feature where T1 : IValue, new()
    {
        protected T1 v1;

        protected internal override void OnEnabled(FeatureContainer owner)
        {
            base.OnEnabled(owner);
            v1 = owner.GetOrAddValue<T1>();
        }
        // protected override bool Filter()
        // {
        //     var hasValue = container.TryGetValue(out v1);
        //     return hasValue;
        // }
    }

    [Serializable]
    public abstract class Feature<T1, T2> : Feature where T1 : IValue, new() 
                                                    where T2 : IValue, new()
    {
        protected T1 v1;
        protected T2 v2;

        protected internal override void OnEnabled(FeatureContainer owner)
        {
            base.OnEnabled(owner);
            v1 = owner.GetOrAddValue<T1>();
            v2 = owner.GetOrAddValue<T2>();
        }

        // protected override bool Filter()
        // {
        //     var hasV1 = container.TryGetValue(out v1);
        //     var hasV2 = container.TryGetValue(out v2);
        //     return hasV1 && hasV2;
        // }
    }

    [Serializable]
    public abstract class Feature<T1, T2, T3> : Feature where T1 : IValue, new() 
                                                        where T2 : IValue, new()
                                                        where T3 : IValue, new()
    {
        protected T1 v1;
        protected T2 v2;
        protected T3 v3;

        protected internal override void OnEnabled(FeatureContainer owner)
        {
            base.OnEnabled(owner);
            v1 = owner.GetOrAddValue<T1>();
            v2 = owner.GetOrAddValue<T2>();
            v3 = owner.GetOrAddValue<T3>();
        }
    }
    
        
    [Serializable]
    public abstract class Feature<T1, T2, T3, T4> : Feature where T1 : IValue, new() 
                                                        where T2 : IValue, new()
                                                        where T3 : IValue, new()
                                                        where T4 : IValue, new()
    {
        protected T1 v1;
        protected T2 v2;
        protected T3 v3;
        protected T4 v4;

        protected internal override void OnEnabled(FeatureContainer owner)
        {
            base.OnEnabled(owner);
            v1 = owner.GetOrAddValue<T1>();
            v2 = owner.GetOrAddValue<T2>();
            v3 = owner.GetOrAddValue<T3>();
            v4 = owner.GetOrAddValue<T4>();
        }
    }
    
            
    [Serializable]
    public abstract class Feature<T1, T2, T3, T4, T5> : Feature where T1 : IValue, new() 
                                                            where T2 : IValue, new()
                                                            where T3 : IValue, new()
                                                            where T4 : IValue, new()
                                                            where T5 : IValue, new()
    {
        protected T1 v1;
        protected T2 v2;
        protected T3 v3;
        protected T4 v4;
        protected T5 v5;

        protected internal override void OnEnabled(FeatureContainer owner)
        {
            base.OnEnabled(owner);
            v1 = owner.GetOrAddValue<T1>();
            v2 = owner.GetOrAddValue<T2>();
            v3 = owner.GetOrAddValue<T3>();
            v4 = owner.GetOrAddValue<T4>();
            v5 = owner.GetOrAddValue<T5>();
        }
    }
    
    [Serializable]
    public abstract class Feature<T1, T2, T3, T4, T5, T6> : Feature where T1 : IValue, new() 
                                                                where T2 : IValue, new()
                                                                where T3 : IValue, new()
                                                                where T4 : IValue, new()
                                                                where T5 : IValue, new()
                                                                where T6 : IValue, new()
    {
        protected T1 v1;
        protected T2 v2;
        protected T3 v3;
        protected T4 v4;
        protected T5 v5;
        protected T6 v6;

        protected internal override void OnEnabled(FeatureContainer owner)
        {
            base.OnEnabled(owner);
            v1 = owner.GetOrAddValue<T1>();
            v2 = owner.GetOrAddValue<T2>();
            v3 = owner.GetOrAddValue<T3>();
            v4 = owner.GetOrAddValue<T4>();
            v5 = owner.GetOrAddValue<T5>();
            v6 = owner.GetOrAddValue<T6>();
        }
    }
    
    [Serializable]
    public abstract class Feature<T1, T2, T3, T4, T5, T6, T7> : Feature where T1 : IValue, new() 
                                                                    where T2 : IValue, new()
                                                                    where T3 : IValue, new()
                                                                    where T4 : IValue, new()
                                                                    where T5 : IValue, new()
                                                                    where T6 : IValue, new()
                                                                    where T7 : IValue, new()
    {
        protected T1 v1;
        protected T2 v2;
        protected T3 v3;
        protected T4 v4;
        protected T5 v5;
        protected T6 v6;
        protected T7 v7;

        protected internal override void OnEnabled(FeatureContainer owner)
        {
            base.OnEnabled(owner);
            v1 = owner.GetOrAddValue<T1>();
            v2 = owner.GetOrAddValue<T2>();
            v3 = owner.GetOrAddValue<T3>();
            v4 = owner.GetOrAddValue<T4>();
            v5 = owner.GetOrAddValue<T5>();
            v6 = owner.GetOrAddValue<T6>();
            v7 = owner.GetOrAddValue<T7>();
        }
    }
}