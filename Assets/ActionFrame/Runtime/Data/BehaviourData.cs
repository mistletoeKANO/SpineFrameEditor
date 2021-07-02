
namespace ActionFrame.Runtime
{
    public class BehaviourData
    {
        [LabelName("行为名称")]
        public string BehaviourName;
        /// <summary>
        /// 起始时间
        /// </summary>
        [HideInView]
        public float BehaviourFrameStartTime;
        /// <summary>
        /// 终止时间
        /// </summary>
        [HideInView]
        public float BehaviourFrameEndTime;

        public BehaviourData()
        {
            this.BehaviourFrameStartTime = 0;
            this.BehaviourFrameEndTime = 0;
        }
    }
}