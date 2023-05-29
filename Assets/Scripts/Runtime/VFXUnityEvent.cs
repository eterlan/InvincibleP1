using System;
using UnityEngine;
using UnityEngine.Events;

namespace Lib
{
    public class VFXUnityEvent : MonoBehaviour
    {
        public UnityEvent onStopped;
        public UnityEvent<GameObject> onCollision;
        public UnityEvent onTriggered;
        private void OnParticleSystemStopped()
        {
            onStopped?.Invoke();
        }

        private void OnParticleCollision(GameObject other)
        {
            onCollision?.Invoke(other);
        }

        private void OnParticleTrigger()
        {
            onTriggered?.Invoke();
        }
    }
}