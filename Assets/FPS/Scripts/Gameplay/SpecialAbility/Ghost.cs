using System;
using FPS.Scripts.Game;
using FPS.Scripts.Game.Managers;
using UnityEngine;

namespace FPS.Scripts.Gameplay
{
    public class Ghost : AbilityBase 
    {
        private int m_originLayer;
        private int m_ghostLayer;

        private void Awake()
        {
            m_ghostLayer = LayerMask.NameToLayer("IgnoreObstacle");
        }

        protected override void Init()
        {
            base.Init();
            var playerGO = PlayerCharacterController.gameObject;
            m_originLayer = playerGO.layer;
            playerGO.layer = m_ghostLayer;
            var args = Events.PlayerGhostEvent;
            args.enable = true;
            EventManager.Broadcast(args);
        }

        protected override void Finish()
        {
            base.Finish();
            PlayerCharacterController.gameObject.layer = m_originLayer;
            var args = Events.PlayerGhostEvent;
            args.enable = false;
            EventManager.Broadcast(args);
        }
    }
}