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

        [LabelName("是否循环")]
        public bool IsLoop;

        [LabelName("循环次数", "次数为-1时,则为运行时自定义次数")]
        public int LoopCount;

        [LabelName("输入列表")]
        [SerializeReference]
        public List<ICheckInput> KeyCode;
    }

    public class ICheckInput
    {
        [LabelName("按键")]
        public KeyCode Code;
    }

    public class ChangeState : IHandle
    {
        public void BeginHandle()
        {
            
        }
    }
}