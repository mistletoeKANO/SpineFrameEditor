using System.Collections.Generic;
using UnityEngine;

namespace ActionFrame.Runtime
{
    /// <summary>
    /// 跳转动画
    /// </summary>
    [LabelName("状态跳转基本信息")]
    [BehaviourConfig(typeof(ChangeState))]
    public class ChangeStateConfig : BehaviourData
    {
        [LabelName("跳转状态名称", "下一个状态名称.")]
        public string StateName;

        [LabelName("过渡时间")]
        public float Duration;

        [LabelName("输入列表")]
        [SerializeReference]
        public List<KeyCodeCheck> KeyCode;
    }

    public class ChangeState : BaseHandle
    {
        public override void StartHandle(ESkeletonAnimation hero)
        {
            
        }

        public override void UpdateHandle(ESkeletonAnimation hero, float dealtTime)
        {
            ChangeStateConfig stateConfig = (ChangeStateConfig) this.config;
            if (this.IsSatisfyCondition(stateConfig.KeyCode))
            {
                StateData data = hero.GetStateData(stateConfig.StateName);
                hero.ChangeStateWithMix(data.StateName, data.IsLoop, stateConfig.Duration);
            }
        }

        private bool IsSatisfyCondition(List<KeyCodeCheck> codeList)
        {
            foreach (var code in codeList)
            {
                if (!InputEventCache.IsHasInput(code.EventType))
                {
                    return false;
                }
            }
            return true;
        }

        public override void ExitHandle(ESkeletonAnimation hero)
        {
            
        }
    }
}