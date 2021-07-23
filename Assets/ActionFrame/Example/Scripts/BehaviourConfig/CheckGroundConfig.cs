using UnityEngine;

namespace ActionFrame.Runtime
{
    [LabelName("地面状态检查")]
    [BehaviourConfig(typeof(CheckGroundHandle))]
    public class CheckGroundConfig : BehaviourData
    {
        [LabelName("下一阶段状态名称")]
        public string NextStateName;
    }

    public class CheckGroundHandle : BaseHandle
    {
        public override void StartHandle(ESkeletonAnimation hero)
        {
            
        }

        public override void UpdateHandle(ESkeletonAnimation hero, float dealtTime)
        {
            CheckGroundConfig groundConfig = (CheckGroundConfig) this.config;
            if (!hero.IsInGround)
            {
                hero.ChangeStateWithMix(groundConfig.NextStateName, false, 0.2f);
            }
        }

        public override void ExitHandle(ESkeletonAnimation hero)
        {
            
        }
    }
}