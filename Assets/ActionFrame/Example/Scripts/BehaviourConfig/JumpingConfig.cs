using UnityEngine;

namespace ActionFrame.Runtime
{
    [LabelName("跳跃中状态判定")]
    [BehaviourConfig(typeof(JumpingHandle))]
    public class JumpingConfig : BehaviourData
    {
        [LabelName("下一阶段状态名称")]
        public string NextStateName;
    }

    public class JumpingHandle : BaseHandle
    {
        public override void StartHandle(ESkeletonAnimation hero)
        {
            
        }

        public override void UpdateHandle(ESkeletonAnimation hero, float dealtTime)
        {
            hero.AttachMoveSpeed(InputEventCache.InputAxis);
            JumpingConfig groundConfig = (JumpingConfig) this.config;
            if (hero.IsInGround)
            {
                hero.ChangeStateWithMix(groundConfig.NextStateName, false, 0.2f);
                hero.AttachMoveSpeed(Vector2.zero);
            }
        }

        public override void ExitHandle(ESkeletonAnimation hero)
        {
            
        }
    }
}