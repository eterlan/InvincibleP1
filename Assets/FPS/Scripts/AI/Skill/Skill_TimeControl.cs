using System.Threading;
using Cysharp.Threading.Tasks;
using FPS.Scripts.Game;
using FPS.Scripts.Game.Managers;
using UnityEngine;

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

            var color = m_propertyBlock.GetColor(m_colorID);
            color.a = 0;
            m_propertyBlock.SetColor(m_colorID, color);
            m_renderer.SetPropertyBlock(m_propertyBlock);
        }
        
        protected override async UniTask ShowWeaknessPerform(CancellationToken token)
        {
            var elapsedTime = 0f;
            while (elapsedTime < showDuration)
            {
                var color = ShowGradient.Evaluate(elapsedTime / showDuration);
                // m_renderer.material.SetColor(m_colorID, color);
                m_propertyBlock.SetColor(m_colorID, color);
                m_renderer.SetPropertyBlock(m_propertyBlock);
                elapsedTime += Time.deltaTime;
                await UniTask.NextFrame(token);
            }
        }

        protected override async UniTask HideWeaknessPerform(CancellationToken token)
        {
            await base.HideWeaknessPerform(token);
            var elapsedTime = showDuration;
            while (elapsedTime > 0)
            {
                var color = ShowGradient.Evaluate(elapsedTime / showDuration);
                // m_renderer.material.SetColor(m_colorID, color); 
                m_propertyBlock.SetColor(m_colorID, color);
                m_renderer.SetPropertyBlock(m_propertyBlock); 
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