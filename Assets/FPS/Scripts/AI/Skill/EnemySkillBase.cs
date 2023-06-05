using System;
using Cysharp.Threading.Tasks;
using FPS.Scripts.Game.Managers;
using FPS.Scripts.Game.Shared;
using Sirenix.OdinInspector;
using Unity.FPS.AI;
using UnityEngine;

namespace FPS.Scripts.AI
{
    [RequireComponent(typeof(Health))]
    public abstract class EnemySkillBase : MonoBehaviour
    {
        public bool      canUseAbility;
        public Transform target => Owner.DetectionModule.KnownDetectedTarget.transform;
        public float     skillDuration = 10;
        public bool      complete;
        
        [Header("Show")]
        public float     showDuration = 2;
        public bool invincibleOnShow;
        
        protected float     useSkillTime = Mathf.NegativeInfinity;
        

        protected EnemyController Owner;
        
        
        private Health m_health;

        protected virtual void Awake()
        {
            Owner = GetComponentInParent<EnemyController>();
            m_health = GetComponent<Health>();
            m_health.OnDie += () => HideWeaknessPerform();
        }

        protected virtual void Start()
        {
            //HideWeakness();
        }

        private void OnDestroy()
        {
            HideWeakness(false);
        }

        /// <summary>
        /// 准备放技能 展示出弱点
        /// </summary>
        [Button]
        public async UniTaskVoid ShowWeakness()
        {
            m_health.Revive();
            canUseAbility = true;
            if (invincibleOnShow)
            {
                m_health.Invincible = true;
            }
            await ShowWeaknessPerform();
            //m_health.Invincible = false;
            TryUse();
        }
        
        /// <summary>
        /// 停止放技能，收回弱点
        /// </summary>
        [Button]
        public void HideWeakness(bool perform)
        {
            if (m_health != null)
            {
                m_health.Hide();
            }

            if (perform)
            {
                HideWeaknessPerform().Forget();
            }
        }

        protected virtual async UniTask ShowWeaknessPerform()
        {
            await UniTask.CompletedTask;
        }
        

        
        protected virtual async UniTask HideWeaknessPerform()
        {
            await UniTask.CompletedTask;
        }


        [Button]
        public void TryUse()
        {
            if (!canUseAbility)
            {
                return;
            }
            UniTask.Void(UseAsync);
        }

        private async UniTaskVoid UseAsync()
        {
            complete = false;
            OnUse();
            useSkillTime = Time.time;
            var token = this.GetCancellationTokenOnDestroy();
            while (Time.time - useSkillTime <= skillDuration && canUseAbility)
            {
                OnUsing();  
                await UniTask.NextFrame(token);
            }

            complete = true;
            OnComplete();
            // 如果是被摧毁的，就不需要再主动去隐藏弱点了，因为已经坏了。这个时候会走生命为0的事件。
            if (canUseAbility)
            {
                // 如果是时间到了才坏了，就需要主动去隐藏，走Hide的事件
                canUseAbility = false;
                HideWeakness(true);
            }
        }

        protected abstract void OnUsing();

        protected abstract void OnUse();

        protected abstract void OnComplete();
    }
}