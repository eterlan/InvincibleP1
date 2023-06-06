using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FPS.Scripts.Game.Shared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FPS.Scripts.AI
{
    public class Skill_SmartMissile : EnemySkillBase
    {
        public ProjectileBase ProjectileBase;
        public int            minAmount = 4;
        public int            maxAmount = 10;
        public Transform      muzzlePos;

        public  float   shootInterval = 2f;
        private float lastShootTime;

        [Header("Show")]
        public float initHeight;
        public float completeHeight;

        protected override void Start()
        {
            base.Start();
            var startPos = transform.localPosition;
            startPos.y = initHeight;
            transform.localPosition = startPos;
        }

        protected override void OnUsing()
        {
            if (Time.time - lastShootTime < shootInterval)
                return;
            
            lastShootTime = Time.time;
            lastShootTime = Time.time;
            var amount = Random.Range(minAmount, maxAmount);
            for (var i = 0; i < amount; i++)
            {
                var instance   = Instantiate(ProjectileBase, muzzlePos.position, Quaternion.identity);
                var newMissile = instance.GetComponent<ProjectileBase>();
                newMissile.Shoot(Owner.gameObject);
            }
        }

        protected override async UniTask ShowWeaknessPerform(CancellationToken token)
        {
            await base.ShowWeaknessPerform(token);
            var startPos = transform.localPosition;
            startPos.y = initHeight;
            transform.localPosition = startPos;
            await transform.DOLocalMoveY(completeHeight, showDuration).WithCancellation(token);
        }

        protected override async UniTask HideWeaknessPerform(CancellationToken token)
        {
            await base.HideWeaknessPerform(token);
            var startPos = transform.localPosition;
            startPos.y = completeHeight;
            transform.localPosition = startPos;
            transform.DOKill();
            transform.DOLocalMoveY(initHeight, showDuration);
            UniTask.Delay(TimeSpan.FromSeconds(showDuration));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnUse()
        {
            
        }

        protected override void OnComplete()
        {
            
        }
    }
}