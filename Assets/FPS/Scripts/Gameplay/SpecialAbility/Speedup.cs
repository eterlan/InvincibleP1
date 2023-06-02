namespace FPS.Scripts.Gameplay
{
    public class Speedup : AbilityBase
    {
        private float m_originMultiplier;
        public  float speedMultiplier = 3;
        protected override void Init()
        {
            base.Init();
            m_originMultiplier = PlayerCharacterController.externalSpeedModifier;
            PlayerCharacterController.externalSpeedModifier = speedMultiplier;
        }

        protected override void Finish()
        {
            base.Finish();
            PlayerCharacterController.externalSpeedModifier = m_originMultiplier;
        }
    }
}