using System.Collections.Generic;

namespace ActionFrame.Runtime
{
    /// <summary>
    /// Spine 动画状态控制器 数据协议类
    /// </summary>
    public class ESpineControllerData
    {
        /// <summary>
        /// 初始状态
        /// </summary>
        public StateData m_EntryState;
        /// <summary>
        /// 状态列表
        /// </summary>
        public List<StateData> m_StateDataList = new List<StateData>();
    }
}