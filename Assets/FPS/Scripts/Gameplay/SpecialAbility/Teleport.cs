using UnityEngine;

namespace FPS.Scripts.Gameplay
{
    public class Teleport : AbilityBase
    {
        public float range = 6;
        
        protected override void Init()
        {
            base.Init();
            var cameraTr  = PlayerWeaponsManager.WeaponCamera.transform;
            var isHit     = Physics.Raycast(cameraTr.position, cameraTr.forward, out var hit, 1000, -1, QueryTriggerInteraction.Ignore);
            var targetPos = hit.point;
            if (!isHit)
            {
                targetPos = cameraTr.forward * range + cameraTr.position;
                targetPos.y = cameraTr.position.y;
            }
            PlayerCharacterController.Teleport(targetPos);
        }
    }
}