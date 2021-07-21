using UnityEngine;

namespace ActionFrame.Runtime
{
    [LabelName("状态跳转基本信息")]
    [BehaviourConfig(typeof(JumpHandle))]
    public class JumpConfig : BehaviourData
    {
        [LabelName("跳的高度")]
        public float JumpSpeed;
    }
    
    public class JumpHandle : BaseHandle
    {
        public override void StartHandle(ESkeletonAnimation hero)
        {
            JumpConfig jumpConfig = (JumpConfig) this.config;
            hero.transform.MoveSin(jumpConfig.JumpSpeed, 0.467f);
        }

        public override void UpdateHandle(ESkeletonAnimation hero, float dealtTime)
        {
            
        }

        public override void ExitHandle(ESkeletonAnimation hero)
        {
            
        }
    }
}