using System.Collections.Generic;

namespace ActionFrame.Runtime
{
    public class StateData
    {
        /// <summary>
        /// 当前状态名称
        /// </summary>
        public string StateName;
        /// <summary>
        /// 当前状态执行时长
        /// </summary>
        public float Duration;
        /// <summary>
        /// 当前状态帧总数
        /// </summary>
        public int FrameCount;
        /// <summary>
        /// 当前状态是否循环播放
        /// </summary>
        public bool IsLoop = true;
        /// <summary>
        /// 状态事件列表
        /// </summary>
        public List<object> EventList = new List<object>();
        /// <summary>
        /// 帧数据
        /// </summary>
        public Dictionary<int, FrameData> FrameDic = new Dictionary<int, FrameData>();
        /// <summary>
        /// 下一个状态名称
        /// </summary>
        public string NextStateName;
        /// <summary>
        /// 动画过渡时长
        /// </summary>
        public float TransitionTime;

        public StateData(string stateName, float duration, float rate)
        {
            this.StateName = stateName;
            this.Duration = duration;
            this.FrameCount = (int) (duration * rate);
            this.IsLoop = false;
        }
    }
}