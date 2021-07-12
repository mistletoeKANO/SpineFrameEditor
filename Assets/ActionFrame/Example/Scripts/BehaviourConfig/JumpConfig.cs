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
            
        }

        public override void UpdateHandle(ESkeletonAnimation hero, float dealtTime)
        {
            if (!InputEventCache.IsHasInput(InputEventType.Jump))
            {
                return;
            }
            JumpConfig jumpConfig = (JumpConfig) this.config;
            hero.transform.parent.GetComponent<Rigidbody2D>().velocity = new Vector2(0 , jumpConfig.JumpSpeed);
        }

        public override void ExitHandle(ESkeletonAnimation hero)
        {
            
        }
    }
}