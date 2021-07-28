using System;
using System.Collections.Generic;
using UnityEngine;

namespace ActionFrame.Runtime
{
    public class EBox2DCollider : MonoBehaviour
    {
        private readonly Color m_Color = new Color(1f, 0f, 0f, 0.35f);
        [HideInInspector]
        public Vector2 Size;

        [HideInInspector]
        public ESkeletonAnimation Owner;
        public BoxItem BoxItem;
        public float RootPosY = 0f;

        private List<ESkeletonAnimation> m_LastFrameSkeleton;
        private List<ESkeletonAnimation> m_CurFrameSkeleton;
        private List<ESkeletonAnimation> m_TempFrameSkeleton;

        private void Start()
        {
            this.CheckAndResetCollider();
        }

        public void Init(Vector2 size, ESkeletonAnimation owner)
        {
            this.m_LastFrameSkeleton = new List<ESkeletonAnimation>();
            this.m_CurFrameSkeleton = new List<ESkeletonAnimation>();
            this.m_TempFrameSkeleton = new List<ESkeletonAnimation>();
            this.Owner = owner;
            this.Size = size;
            this.RootPosY = this.Owner.transform.parent.position.y;
            this.BoxItem = new BoxItem()
            {
                Offset = Vector2.zero,
                Size = this.Size,
                Rotation = 0,
            };
        }

        private void CheckAndResetCollider()
        {
            ERigidbody2D rigidbody2D = this.GetComponent<ERigidbody2D>();
            if (rigidbody2D == null)
            {
                return;
            }
            float width = Mathf.Abs(rigidbody2D.Velocity.x * Time.fixedDeltaTime);
            if (width > this.Size.x)
            {
                this.Size.x = width;
                this.BoxItem.Size.x = width;
            }
        }
        
        private void FixedUpdate()
        {
            this.m_CurFrameSkeleton.Clear();
            this.m_CurFrameSkeleton = CheckBoxColliderScene.EBox2DColliderOverlap(this);
            this.m_TempFrameSkeleton = this.m_CurFrameSkeleton.SubList(this.m_LastFrameSkeleton);
            foreach (var item in this.m_TempFrameSkeleton)
            {
                this.OnBoxEnter2D(item);
            }
            this.m_TempFrameSkeleton = this.m_LastFrameSkeleton.SubList(this.m_CurFrameSkeleton);
            foreach (var item in this.m_TempFrameSkeleton)
            {
                this.OnBoxExit2D(item);
            }
            this.m_LastFrameSkeleton.Clear();
            this.m_LastFrameSkeleton.AddRange(this.m_CurFrameSkeleton);
        }

        private void OnBoxEnter2D(ESkeletonAnimation anim)
        {
            anim.ChangeStateNoCtr("Damage_1", false, 0, 0.099f);
            float speed = this.GetComponent<ERigidbody2D>().Velocity.normalized.x;
            anim.AttachNormalSpeed(new Vector2(speed * 0.2f, 1));
            anim.SetBullet(1);
        }

        private void OnBoxExit2D(ESkeletonAnimation anim)
        {
            
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            HandlesDrawer.H.PushColor(this.m_Color);
            HandlesDrawer.H.fillPolygon = true;
            HandlesDrawer.H.useFillPolygonOutline = true;
            Matrix4x4 matrix = Matrix4x4.Translate(this.transform.position);
            HandlesDrawer.H.DrawRect(this.Size, matrix);
            HandlesDrawer.H.PopColor();
        }
#endif
        
    }
}