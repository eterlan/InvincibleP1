using FPS.Scripts.Game.Shared;
using UnityEngine;

namespace FPS.Scripts.Game
{
    [RequireComponent(typeof(Health))]
    public abstract class EnemySkillBase : MonoBehaviour
    {
        public  bool   canUseAbility;
        private Health m_health;

        private void Start()
        {
            m_health = GetComponent<Health>();
            m_health.OnHealed += OnHealed;
            m_health.OnDie += OnDie;
        }

        private void OnHealed(float _)
        {
            canUseAbility = true;
        }

        private void OnDie()
        {
            canUseAbility = false;
        }

        public bool TryUse()
        {
            if (!canUseAbility)
            {
                return false;
            }

            Use();
            return true;
        }
        protected virtual void Use()
        {
            

        }
    }
}