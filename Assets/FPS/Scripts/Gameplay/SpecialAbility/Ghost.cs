using System;
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
        }

        protected override void Finish()
        {
            base.Finish();
            PlayerCharacterController.gameObject.layer = m_originLayer;
        }
    }
}