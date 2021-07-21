﻿using UnityEngine;

namespace ActionFrame.Runtime
{
    [LabelName("状态跳转基本信息")]
    [BehaviourConfig(typeof(HitAnimHandle))]
    public class HitAnimConfig : BehaviourData
    {
        [LabelName("击中受力")]
        public UnityEngine.Vector2 HitForce;
    }

    public class HitAnimHandle : BaseHandle
    {
        public override void StartHandle(ESkeletonAnimation hero)
        {
            
        }

        public override void UpdateHandle(ESkeletonAnimation hero, float dealtTime)
        {
            HitAnimConfig force = (HitAnimConfig) this.config;
            EventManager.Instance.InvokeHandle(new Event_HitAnim(force.HitForce,
                Mathf.RoundToInt(force.BehaviourFrameStartTime * hero.FrameRate)));
        }

        public override void ExitHandle(ESkeletonAnimation hero)
        {
            
        }
    }
}