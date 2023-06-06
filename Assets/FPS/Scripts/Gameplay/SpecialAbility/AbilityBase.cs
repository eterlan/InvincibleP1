using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.FPS.Gameplay;
using UnityEngine;

namespace FPS.Scripts.Gameplay
{
    public abstract class AbilityBase : MonoBehaviour
    {
        public float duration;

        protected PlayerWeaponsManager      PlayerWeaponsManager;
        protected PlayerCharacterController PlayerCharacterController;
        protected float                     ElapsedTime;

        public async UniTask Use(PlayerWeaponsManager playerWeaponsManager)
        {
            PlayerWeaponsManager = playerWeaponsManager;
            PlayerCharacterController = playerWeaponsManager.GetComponent<PlayerCharacterController>();
            Init();
            while (ElapsedTime < duration)
            {
                ElapsedTime += Time.deltaTime * PlayerCharacterController.timeScale;
                Tick();
                await UniTask.NextFrame();
            }
            Finish();
        }

        protected virtual void Tick()
        {
            
        }
        
        protected virtual void Init()
        {
            
        }

        protected virtual void Finish()
        {
            
        }
    }
}