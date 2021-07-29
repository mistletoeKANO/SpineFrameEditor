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
            AttackMoveConfig atkConfig = (AttackMoveConfig) this.config;
            hero.transform.parent.position += new Vector3(atkConfig.MoveValue * hero.skeleton.ScaleX, 0);
        }

        public override void ExitHandle(ESkeletonAnimation hero)
        {
            
        }
    }
}