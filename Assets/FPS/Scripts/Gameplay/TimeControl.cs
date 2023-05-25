using System;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class TimeControl : MonoBehaviour
    {
        public static float BossTimeScale;
        
        public float BossSlowTimeScale    = 0.8f;
        public float BossNormalTimeScale  = 1f;
        public float otherSlowTimeScale   = 0.3f;
        public float otherNormalTimeScale = 1f;

        public void EnterBossSlowMotion()
        {
            BossTimeScale = 0.8f;
            Time.timeScale = otherSlowTimeScale;
        }

        public void ExitBossSlowMotion()
        {
            BossTimeScale = BossNormalTimeScale;
            Time.timeScale = otherNormalTimeScale;
        }
        
    }
}