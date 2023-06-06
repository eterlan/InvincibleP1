using System;
using FPS.Scripts.Game;
using FPS.Scripts.Game.Managers;
using Unity.FPS.Gameplay;

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
        private void OnPickup(PickupEvent go)
        {
            if (go.Pickup.TryGetComponent<Pickup>(out var pickup))
            {
                if (pickup.GetType() == pickupType)
                {
                    m_spawnAmountInLevel--;
                }
            }
        }
        
    }
}