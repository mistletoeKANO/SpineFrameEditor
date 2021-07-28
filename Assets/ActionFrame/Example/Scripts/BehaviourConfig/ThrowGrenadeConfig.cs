using UnityEngine;

namespace ActionFrame.Runtime
{
    [LabelName("投掷炸弹")]
    [BehaviourConfig(typeof(ThrowGrenadeHandle))]
    public class ThrowGrenadeConfig : BehaviourData
    {
        [LabelName("初始速度")]
        public Vector2 StartSpeed;

        [LabelName("生成位置")]
        public Vector2 Offset;
        
    }
    
    public class ThrowGrenadeHandle : BaseHandle
    {
        public override void StartHandle(ESkeletonAnimation hero)
        {
            
        }

        public override void UpdateHandle(ESkeletonAnimation hero, float dealtTime)
        {
            ThrowGrenadeConfig grenadeConfig = (ThrowGrenadeConfig) this.config;
            GameObject obj = new GameObject("Grenade");
            obj.transform.position = new Vector3(grenadeConfig.Offset.x * hero.skeleton.ScaleX, grenadeConfig.Offset.y) +
                                     hero.transform.position;
            var eBox = obj .AddComponent<EBox2DCollider>();
            eBox.Init(Vector2.one, hero);
            var eGravity = obj.AddComponent<ERigidbody2D>();
            eGravity.Init(new Vector2(grenadeConfig.StartSpeed.x * hero.skeleton.ScaleX, grenadeConfig.StartSpeed.y),
                hero.transform.parent.GetChild(2).position.y);
        }

        public override void ExitHandle(ESkeletonAnimation hero)
        {
            
        }
    }
}