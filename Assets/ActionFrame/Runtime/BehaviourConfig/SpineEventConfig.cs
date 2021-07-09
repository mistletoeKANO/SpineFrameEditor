using UnityEngine;

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
            int frame = Mathf.RoundToInt(hero.CurrentTrack.AnimationTime * 30);
            UnityEngine.Debug.Log($"Spine内置事件. 当前帧{frame}");
        }

        public override void ExitHandle(ESkeletonAnimation hero)
        {
            
        }
    }
}