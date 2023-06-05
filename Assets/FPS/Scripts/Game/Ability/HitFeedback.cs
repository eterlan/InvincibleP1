using System;
using FPS.Scripts.Game.Shared;
using UnityEngine;

namespace FPS.Scripts.Game
{
    [RequireComponent(typeof(Health))]
    public class HitFeedback : MonoBehaviour
    {
        [GradientUsage(true)]
        public Gradient  hitGradient;
        public float     hitFlashDuration;
        public AudioClip hitSFX;
        
        public GameObject deathVFX;
        public GameObject model;
        public Vector3    deathVFXSpawnOffset = new(0, -0.1f, 0);

        private Health                m_health;
        private Renderer              m_renderer;
        private MaterialPropertyBlock m_weaknessPropertyBlock;
        private float                 m_lastTimeDamaged = Mathf.NegativeInfinity;
        private bool                  m_wasDamagedThisFrame;
        private int                   m_emissionColorID;
        private Collider              m_collider;

        private void Update()
        {
            var sinceLastTimeDamaged = Time.time - m_lastTimeDamaged;

            var color = hitGradient.Evaluate(sinceLastTimeDamaged / hitFlashDuration);
            m_weaknessPropertyBlock.SetColor(m_emissionColorID, color);
            m_renderer.SetPropertyBlock(m_weaknessPropertyBlock);
        
            
            m_wasDamagedThisFrame = false;
        }

        private void Awake()
        {
            m_emissionColorID = Shader.PropertyToID("_EmissionColor");
            m_health = GetComponent<Health>();
            m_renderer = GetComponentInChildren<Renderer>();
            m_weaknessPropertyBlock = new MaterialPropertyBlock();
            m_health.OnHide += OnHide;
            m_health.OnDamaged += OnDamaged;
            m_health.OnDie += OnDie;
            m_health.OnRevive += OnRevive;
            m_collider = GetComponent<Collider>();
        }

        private void OnHide()
        {
            m_collider.enabled = false;
        }

        private void OnDestroy()
        {
            m_health.OnDamaged -= OnDamaged;
            m_health.OnDie -= OnDie;
            m_health.OnRevive -= OnRevive;
            m_health.OnHide -= OnHide;
        }

        private void OnRevive()
        {
            m_collider.enabled = true;
            m_renderer.enabled = true;
        }

        private void OnDie()
        {
            // spawn a particle system when dying
            var vfx = Instantiate(deathVFX, transform.position + deathVFXSpawnOffset, Quaternion.identity);
            Destroy(vfx, 5f);
            OnHide();
            m_renderer.enabled = false;
        }

        private void OnDamaged(float dmg, GameObject damageSource)
        {
            // test if the damage source is the player
            if (damageSource)
            {
                m_lastTimeDamaged = Time.time;
            
                // play the damage tick sound and dont play it again in this frame.
                if (hitSFX && !m_wasDamagedThisFrame)
                    AudioUtility.CreateSFX(hitSFX, transform.position, AudioUtility.AudioGroups.DamageTick, 0f);
            
                m_wasDamagedThisFrame = true;
            }
        }
    }
}