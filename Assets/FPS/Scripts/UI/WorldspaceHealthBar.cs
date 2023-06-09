﻿using System;
using FPS.Scripts.Game.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class WorldspaceHealthBar : MonoBehaviour
    {
        [Tooltip("Health component to track")] public Health Health;

        [Tooltip("Image component displaying health left")]
        public Image HealthBarImage;

        [Tooltip("The floating healthbar pivot transform")]
        public Transform HealthBarPivot;

        [Tooltip("Whether the health bar is visible when at full health or not")]
        public bool HideFullHealthBar = true;

        private void Awake()
        {
            Health.OnRevive += OnRevive;
            Health.OnHide += OnDie;
            Health.OnDie += OnDie;
        }

        private void OnDestroy()
        {
            Health.OnRevive -= OnRevive;
            Health.OnHide -= OnDie;
            Health.OnDie -= OnDie;
        }

        private void OnRevive()
        {
            ToggleVisibility(true);
        }

        private void OnDie()
        {
            ToggleVisibility(false);
        }

        void Update()
        {
            // update health bar value
            HealthBarImage.fillAmount = Health.CurrentHealth / Health.MaxHealth;

            // rotate health bar to face the camera/player
            HealthBarPivot.LookAt(Camera.main.transform.position);

            // hide health bar if needed
            if (HideFullHealthBar)
                HealthBarPivot.gameObject.SetActive(HealthBarImage.fillAmount != 1);
        }

        public void ToggleVisibility(bool show)
        {
            HealthBarPivot.gameObject.SetActive(show);
        }
    }
}