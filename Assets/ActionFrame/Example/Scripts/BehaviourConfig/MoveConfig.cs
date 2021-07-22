using UnityEngine;

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
            if (!InputEventCache.IsOnlyInput(InputEventType.Walk) && !InputEventCache.IsOnlyInput(InputEventType.Run))
            {
                return;
            }
            MoveConfig moveConfig = (MoveConfig) this.config;
            Vector2 moveVec = new Vector2(InputEventCache.InputAxis.x, InputEventCache.InputAxis.y * Mathf.Pow(2, 0.5f) / 2f);
            hero.AttachMoveSpeed(moveVec * moveConfig.MoveSpeed);
            hero.skeleton.ScaleX = (int) (InputEventCache.InputAxis.x * 100) == 0 ? hero.skeleton.ScaleX : InputEventCache.InputAxis.x > 0 ? 1 : -1;
            hero.GetComponent<Renderer>().sortingOrder = (int) (-hero.transform.parent.position.y * 1000);
        }

        public override void ExitHandle(ESkeletonAnimation hero)
        {
            
        }
    }
}