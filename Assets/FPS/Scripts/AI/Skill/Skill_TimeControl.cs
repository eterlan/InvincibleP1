using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FPS.Scripts.Game;
using FPS.Scripts.Game.Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace FPS.Scripts.AI
{
    public class Skill_TimeControl : EnemySkillBase
    {
        [Range(0.1f, 1)]
        public float timeScale = 0.3f;

        [GradientUsage(true)]
        public  Gradient              ShowGradient;


        private int                   m_colorID;
        private MeshRenderer          m_renderer;
        private MaterialPropertyBlock m_propertyBlock;

        protected override void Awake()
        {
            base.Awake();

            m_propertyBlock = new MaterialPropertyBlock();
            m_renderer = GetComponentInChildren<MeshRenderer>();
            m_colorID = Shader.PropertyToID("_BaseColor");
        }
        
        protected override async UniTask ShowWeaknessPerform()
        {
            var elapsedTime = 0f;
            var token       = this.GetCancellationTokenOnDestroy();
            while (elapsedTime < showDuration && canUseAbility)
            {
                var color = ShowGradient.Evaluate(elapsedTime / showDuration);
                m_renderer.material.SetColor(m_colorID, color);
                //m_renderer.SetPropertyBlock(m_propertyBlock);
                elapsedTime += Time.deltaTime;
                await UniTask.NextFrame(token);
            }
        }

        protected override async UniTask HideWeaknessPerform()
        {
            await base.HideWeaknessPerform();
            var token       = this.GetCancellationTokenOnDestroy();
            var elapsedTime = showDuration;
            while (elapsedTime > 0 && canUseAbility)
            {
                var color = ShowGradient.Evaluate(elapsedTime / showDuration);
                m_renderer.material.SetColor(m_colorID, color); 
                //m_renderer.SetPropertyBlock(m_propertyBlock); 
                elapsedTime -= Time.deltaTime;
                await UniTask.NextFrame(token);
            }
        }

        protected override void OnUsing()
        {
            
        }

        protected override void OnUse()
        {
            var arg = Events.ChangeTimeScaleEvent;
            arg.timeScale = timeScale; 
            EventManager.Broadcast(arg);
        }

        protected override void OnComplete()
        {
            var arg = Events.ChangeTimeScaleEvent;
            arg.timeScale = 1; 
            EventManager.Broadcast(arg);
        }
    }
}