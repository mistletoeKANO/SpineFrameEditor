namespace ActionFrame.Runtime
{
    [LabelName("附加速度")]
    [BehaviourConfig(typeof(AttackSpeedHandle))]
    public class AttackSpeedConfig : BehaviourData
    {
        
    }
    
    public class AttackSpeedHandle : BaseHandle
    {
        public override void StartHandle(ESkeletonAnimation hero)
        {
            
        }

        public override void UpdateHandle(ESkeletonAnimation hero, float dealtTime)
        {
            hero.AttachSpeedSelfY(0);
        }

        public override void ExitHandle(ESkeletonAnimation hero)
        {
            
        }
    }
}