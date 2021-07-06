namespace ActionFrame.Runtime
{
    public sealed partial class ESkeletonAnimation
    {
        private StateData m_DefaultState;
        private StateData m_CurrentState;
        private StateData m_NextState;
        
        private void InitJsonData(ESpineControllerData ctrData)
        {
            this.m_DefaultState = this.m_ESpineCtrData.m_EntryState;
            
        }

        private void UpdateInput(float dealtTime)
        {
            
        }
    }
}