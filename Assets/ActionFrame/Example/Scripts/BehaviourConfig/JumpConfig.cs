
namespace ActionFrame.Runtime
{
    [LabelName("状态跳转基本信息")]
    [BehaviourConfig(typeof(JumpHandle))]
    public class JumpConfig : BehaviourData
    {
        [LabelName("起跳速度")]
        public UnityEngine.Vector2 JumpSpeed;
    }
    
    public class JumpHandle : BaseHandle
    {
        public override void StartHandle(ESkeletonAnimation hero)
        {
            JumpConfig jumpConfig = (JumpConfig) this.config;
            hero.AttachNormalSpeed(jumpConfig.JumpSpeed);
        }

        public override void UpdateHandle(ESkeletonAnimation hero, float dealtTime)
        {
            
        }

        public override void ExitHandle(ESkeletonAnimation hero)
        {
            
        }
    }
}