using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FPS.Scripts.Game;
using FPS.Scripts.Game.Shared;
using Sirenix.OdinInspector;
using Unity.FPS.AI;
using UnityEngine;

namespace FPS.Scripts.AI
{
    [RequireComponent(typeof(Health))]
    public abstract class EnemySkillBase : MonoBehaviour
    {
        [ShowInInspector][ReadOnly]
        public bool canUseAbility;
        public    Transform   target => Owner.DetectionModule.KnownDetectedTarget.transform;


        [Header("AI")]
        public MinMaxFloat skillDurationRange = new(5, 10);
        public MinMaxFloat skillIntervalRange = new(10, 15);
        [Range(1, 2)]
        public float       bonusIntervalMultiplier = 1.2f;
        public float     useSkillDelay = 2f;
        public AudioClip playSkillSfx;
        
        protected float skillDuration         = 10;
        protected float lastEndUseSkillTime   = Mathf.NegativeInfinity;
        protected float LastStartUseSkillTime = Mathf.NegativeInfinity;
        [Range(0, 1)]
        public float canUseHp = 0.9f;
        
        
        public bool  complete;
        
        [Header("Show")]
        public float     showDuration = 2;
        public bool invincibleOnShow;
        
        

        protected EnemyController Owner;

        private   CancellationTokenSource m_weaknessBrokenCts;
        private   CancellationTokenSource m_linkedCts;
        private   float                   m_skillInterval;
        private   Health                  m_health;
        protected CancellationToken       OnDestroyToken;
        protected AudioSource             audioSource;

        protected virtual void Awake()
        {
            OnDestroyToken = this.GetCancellationTokenOnDestroy();
            Owner = GetComponentInParent<EnemyController>();
            audioSource = GetComponent<AudioSource>();
            m_health = GetComponent<Health>();
            m_health.OnDie += OnDie;
        }

        protected virtual void Start()
        {
            if (m_health != null)
            {
                m_health.Hide();
            }
        }

        /// <summary>
        /// 检查是否能使用技能
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckCondition(EnemyController enemyController)
        {
            return !canUseAbility && enemyController.health.GetRatio() < canUseHp && Time.time - lastEndUseSkillTime > m_skillInterval;
        }

        protected virtual void OnDestroy()
        {
            m_health.Kill();
            OnComplete();
        }

        public bool TryUse(EnemyController enemyController)
        {
            if (!CheckCondition(enemyController))
            {
                return false;
            }

            m_weaknessBrokenCts?.Dispose();
            m_weaknessBrokenCts = new CancellationTokenSource();
            m_linkedCts = CancellationTokenSource.CreateLinkedTokenSource(m_weaknessBrokenCts.Token, OnDestroyToken);
            ShowWeakness(m_linkedCts.Token).Forget();
            return true;
        }

        /// <summary>
        /// 准备放技能 展示出弱点
        /// </summary>
        /// <param name="token"></param>
        [Button]
        public async UniTaskVoid ShowWeakness(CancellationToken token)
        {
            audioSource.PlayOneShot(playSkillSfx);
            m_health.Revive();
            canUseAbility = true;
            if (invincibleOnShow)
            {
                m_health.Invincible = true;
            }
            await ShowWeaknessPerform(token);

            await UniTask.Delay(TimeSpan.FromSeconds(useSkillDelay), cancellationToken: token);
            
            m_health.Invincible = false;
            
            UseAsync(token).Forget();
        }
        
        /// <summary>
        /// 停止放技能，收回弱点
        /// </summary>
        [Button]
        public void HideWeakness(CancellationToken token, bool perform = true)
        {
            if (m_health != null)
            {
                m_health.Hide();
            }

            if (perform)
            {
                HideWeaknessPerform(token).Forget();
            }
        }

        protected virtual async UniTask ShowWeaknessPerform(CancellationToken token)
        {
            await UniTask.CompletedTask;
        }
        
        
        protected virtual async UniTask HideWeaknessPerform(CancellationToken cancellationToken)
        {
            await UniTask.CompletedTask;
        }

        private async UniTaskVoid UseAsync(CancellationToken token)
        {
            complete = false;
            OnUse();
            LastStartUseSkillTime = Time.time;
            skillDuration = skillDurationRange.GetRandomValue();
            while (Time.time - LastStartUseSkillTime <= skillDuration && canUseAbility)
            {
                OnUsing();  
                await UniTask.NextFrame(token);
            }
            
            // 如果是时间到了而不是击毁，就需要主动去隐藏，走Hide的事件
            if (!canUseAbility)
                return;
            
            OnComplete();
            ResetSkillCooldown();
            complete = true;

            canUseAbility = false;
            HideWeakness(token);
        }

        // 如果是被摧毁的，就不需要再主动去隐藏弱点了，因为已经坏了。这个时候会走生命为0的事件。
        private void OnDie()
        {
            canUseAbility = false;
            m_weaknessBrokenCts?.Cancel();
            //HideWeaknessPerform(m_linkedCts.Token).Forget();
            ResetSkillCooldown();
            complete = true;
            OnComplete();
        }

        private void ResetSkillCooldown()
        {
            lastEndUseSkillTime = Time.time;
            m_skillInterval = skillIntervalRange.GetRandomValue();
            m_skillInterval *= bonusIntervalMultiplier;
        }

        protected abstract void OnUsing();

        protected abstract void OnUse();

        protected abstract void OnComplete();
    }
}