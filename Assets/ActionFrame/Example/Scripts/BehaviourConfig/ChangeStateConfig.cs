namespace ActionFrame.Runtime
{
    /// <summary>
    /// 跳转动画
    /// </summary>
    [LabelName("状态跳转基本信息")]
    [BehaviourConfig(typeof(ChangeState))]
    public class ChangeStateConfig : BehaviourData
    {
        [LabelName("跳转状态名称")]
        public string StateName;

        [LabelName("是否循环")]
        public bool IsLoop;

        [LabelName("循环次数")]
        public int LoopCount;
    }

    public class ChangeState
    {
        
    }
}