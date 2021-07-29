using UnityEngine;

namespace ActionFrame.Runtime
{
    [LabelName("混合动画")]
    [BehaviourConfig(typeof(MixAnimHandle))]
    public class MixAnimConfig : BehaviourData
    {
        [LabelName("混合动画名称")]
        public string StateName;
    }
    
    public class MixAnimHandle : BaseHandle
    {
        public override void StartHandle(ESkeletonAnimation hero)
        {
            
        }

        public override void UpdateHandle(ESkeletonAnimation hero, float dealtTime)
        {
            MixAnimConfig mixAnimConfig = (MixAnimConfig) this.config;
            if (Mathf.Abs(hero.CurAnimProcess.SpeedX) > 0 || Mathf.Abs(hero.CurAnimProcess.SpeedRootY) > 0)
            {
                hero.MixState(mixAnimConfig.StateName, 0.2f, true);
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