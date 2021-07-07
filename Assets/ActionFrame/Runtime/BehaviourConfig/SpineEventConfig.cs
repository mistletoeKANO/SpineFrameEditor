namespace ActionFrame.Runtime
{
    /// <summary>
    /// Spine 动画 配置的 事件
    /// </summary>
    [BehaviourConfig(typeof(SpineEventHandle), false)]
    public class SpineEventConfig : BehaviourData
    {
        
    }

    public class SpineEventHandle : BaseHandle
    {
        public override void StartHandle(ESkeletonAnimation hero)
        {
            
        }

        public override void UpdateHandle(ESkeletonAnimation hero, float dealtTime)
        {
            
        }

        public override void ExitHandle(ESkeletonAnimation hero)
        {
            
        }
    }
}