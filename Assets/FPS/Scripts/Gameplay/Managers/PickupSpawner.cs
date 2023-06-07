using System;
using FPS.Scripts.Game;
using FPS.Scripts.Game.Managers;
using Unity.FPS.Gameplay;
using UnityEngine;

namespace FPS.Scripts.Gameplay.Managers
{
    public class PickupSpawner : SimpleSpawner
    {
        private Type pickupType;

        protected override void Start()
        {
            base.Start();
            EventManager.AddListener<PickupEvent>(OnPickup);
            pickupType = prefabs[0].GetComponent<Pickup>().GetType();
        }

        public override bool TrySpawn(out GameObject spawnInstance)
        {
            if (!base.TrySpawn(out spawnInstance))
                return false;

            var pickup = spawnInstance.GetComponent<Pickup>();
            pickup.generationType = GeneratedType.Spawn;
            return true;
        }

        private void OnPickup(PickupEvent go)
        {
            if (go.Pickup.TryGetComponent<Pickup>(out var pickup))
            {
                if (pickup.GetType() == pickupType && pickup.generationType == GeneratedType.Spawn)
                {
                    m_spawnAmountInLevel--;
                }
            }
        }
        
    }
}