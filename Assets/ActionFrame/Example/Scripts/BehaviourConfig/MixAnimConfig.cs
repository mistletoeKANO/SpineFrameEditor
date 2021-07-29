using UnityEngine;

namespace ActionFrame.Runtime
{
    [LabelName("混合动画")]
    [BehaviourConfig(typeof(MixAnimHandle))]
    public class MixAnimConfig : BehaviourData
    {
        [LabelName("混合动画名称")]
        public string StateName;
        [LabelName("混合动画移速")]
        public float MoveSpeed;
        [LabelName("过渡时长")]
        public float MixDuration;
    }
    
    public class MixAnimHandle : BaseHandle
    {
        public override void StartHandle(ESkeletonAnimation hero)
        {
            
        }

        public override void UpdateHandle(ESkeletonAnimation hero, float dealtTime)
        {
            MixAnimConfig mixAnimConfig = (MixAnimConfig) this.config;
            if (Mathf.Abs(InputEventCache.InputAxis.x) > 0 || Mathf.Abs(InputEventCache.InputAxis.y) > 0)
            {
                Vector2 moveVec = new Vector2(InputEventCache.InputAxis.x, InputEventCache.InputAxis.y * Mathf.Pow(2, 0.5f) / 2f);
                hero.AttachMoveSpeed(moveVec * mixAnimConfig.MoveSpeed);
                hero.MixState(mixAnimConfig.StateName, mixAnimConfig.MixDuration, true);
            }
            else
            {
                hero.ClearMixTrack();
            }
        }

        public override void ExitHandle(ESkeletonAnimation hero)
        {
            
        }
    }
}