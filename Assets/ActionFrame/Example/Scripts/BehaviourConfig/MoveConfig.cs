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
            if (!InputEventCache.IsHasInput(InputEventType.Walk))
            {
                return;
            }
            MoveConfig moveConfig = (MoveConfig) this.config;
            Vector3 moveVec = new Vector3(InputEventCache.InputAxis.x, InputEventCache.InputAxis.y * Mathf.Pow(2, 0.5f) / 2f, 0);
            hero.transform.position += moveVec * moveConfig.MoveSpeed;
        }

        public override void ExitHandle(ESkeletonAnimation hero)
        {
            
        }
    }
}