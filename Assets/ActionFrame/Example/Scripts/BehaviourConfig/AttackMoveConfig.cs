using UnityEngine;

namespace ActionFrame.Runtime
{
    [LabelName("攻击移动")]
    [BehaviourConfig(typeof(AttackMoveHandle))]
    public class AttackMoveConfig : BehaviourData
    {
        [LabelName("移动值")]
        public float MoveValue;
    }

    public class AttackMoveHandle : BaseHandle
    {
        public override void StartHandle(ESkeletonAnimation hero)
        {
            
        }

        public override void UpdateHandle(ESkeletonAnimation hero, float dealtTime)
        {
            AttackMoveConfig config = (AttackMoveConfig) this.config;
            Vector3 startPos = hero.transform.position;
            Vector3 endPos = startPos + new Vector3(config.MoveValue * hero.skeleton.ScaleX, 0, 0);
            hero.transform.parent.position = Vector3.Lerp(startPos, endPos, 0.2f);
        }

        public override void ExitHandle(ESkeletonAnimation hero)
        {
            
        }
    }
}