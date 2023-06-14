using System;
using UnityEngine;

namespace FPS.Scripts.Gameplay
{
    public class Teleport : AbilityBase
    {
        public float          range = 6;
        public AnimationCurve positionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Vector3 m_startPos;
        private Vector3 m_targetPos;

        protected override void Init() 
        {
            base.Init();
            var cameraTr  = PlayerWeaponsManager.WeaponCamera.transform;
            var isHit     = Physics.Raycast(cameraTr.position, cameraTr.forward, out var hit, range, -1, QueryTriggerInteraction.Ignore);
            m_targetPos = hit.point;
            if (!isHit)
            {
                m_targetPos = cameraTr.forward * range + cameraTr.position;
                m_targetPos.y = cameraTr.position.y;
            }

            m_startPos = transform.position;
            PlayerCharacterController.enabled = false;
            PlayerCharacterController.Controller.enabled = false;
            //PlayerCharacterController.Teleport(targetPos, duration);
        }

        protected override void Tick()
        {
            base.Tick();
            var lerpValue = positionCurve.Evaluate(ElapsedTime / duration);
            var newPos    = m_startPos + (m_targetPos - m_startPos) * lerpValue;
            transform.position = newPos;
        }

        protected override void Finish()
        {
            base.Finish();
            PlayerCharacterController.enabled = true;
            PlayerCharacterController.Controller.enabled = true;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}