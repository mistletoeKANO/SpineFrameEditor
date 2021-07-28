using UnityEngine;

namespace ActionFrame.Runtime
{
    [LabelName("远程弹道攻击")]
    [BehaviourConfig(typeof(ShootFpxHandle))]
    public class ShootFpxConfig : BehaviourData
    {
        [LabelName("特效名称")]
        public string FpxName;

        [LabelName("子弹速度")]
        public float Speed;

        [LabelName("生成位置")]
        public Vector2 InstantiatePos;
    }
    
    public class ShootFpxHandle : BaseHandle
    {
        public override void StartHandle(ESkeletonAnimation hero)
        {
            
        }

        public override void UpdateHandle(ESkeletonAnimation hero, float dealtTime)
        {
            ShootFpxConfig fpxConfig = (ShootFpxConfig) this.config;
            UnityEngine.GameObject obj = new GameObject("Bullet");
            obj.transform.position =
                new Vector3(fpxConfig.InstantiatePos.x * hero.skeleton.ScaleX, fpxConfig.InstantiatePos.y) +
                hero.transform.position;
            var eBox = obj.AddComponent<EBox2DCollider>();
            eBox.Init(Vector2.one, hero);
            var eRig = obj.AddComponent<ERigidbody2D>();
            eRig.Init(new Vector2(fpxConfig.Speed * hero.skeleton.ScaleX, 0), -int.MaxValue, 0);
        }

        public override void ExitHandle(ESkeletonAnimation hero)
        {
            
        }
    }
}