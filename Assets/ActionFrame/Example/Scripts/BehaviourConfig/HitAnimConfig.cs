using UnityEngine;

namespace ActionFrame.Runtime
{
    [LabelName("攻击目标帧信息")]
    [BehaviourConfig(typeof(HitAnimHandle))]
    public class HitAnimConfig : BehaviourData
    {
        [LabelName("击中速度")]
        public UnityEngine.Vector2 HitSpeed;
    }

    public class HitAnimHandle : BaseHandle
    {
        public override void StartHandle(ESkeletonAnimation hero)
        {
            
        }

        public override void UpdateHandle(ESkeletonAnimation hero, float dealtTime)
        {
            HitAnimConfig force = (HitAnimConfig) this.config;
            EventManager.Instance.InvokeHandle(new Event_HitAnim(force.HitSpeed,
                Mathf.RoundToInt(force.BehaviourFrameStartTime * hero.FrameRate)));
            hero.AttachNormalSpeed(new Vector2(1,0));
        }

        public override void ExitHandle(ESkeletonAnimation hero)
        {
            
        }
    }
}