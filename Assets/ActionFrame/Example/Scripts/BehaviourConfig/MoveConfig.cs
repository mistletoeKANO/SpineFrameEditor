﻿using UnityEngine;

namespace ActionFrame.Runtime
{
    [LabelName("状态跳转基本信息")]
    [BehaviourConfig(typeof(MoveHandle))]
    public class MoveConfig : BehaviourData
    {
        [LabelName("移动速度")]
        public float MoveSpeed;
    }

    public class MoveHandle : BaseHandle
    {
        public override void StartHandle(ESkeletonAnimation hero)
        {
            
        }

        public override void UpdateHandle(ESkeletonAnimation hero, float dealtTime)
        {
            MoveConfig moveConfig = (MoveConfig) this.config;
            hero.transform.position += new Vector3(hero.skeleton.ScaleX * moveConfig.MoveSpeed, 0, 0);
        }

        public override void ExitHandle(ESkeletonAnimation hero)
        {
            
        }
    }
}