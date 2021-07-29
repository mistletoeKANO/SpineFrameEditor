using System;
using UnityEngine;

namespace ActionFrame.Runtime
{
    public partial class ESkeletonAnimation
    {
        /// <summary>
        /// 重力加速度 G
        /// </summary>
        private readonly float m_G = 9.8f;

        private AnimInfo m_CurAnimProcess = new AnimInfo();
        public AnimInfo CurAnimProcess => this.m_CurAnimProcess;
        
        public bool IsUseSimulate = true;
        public bool IsInGround = true;

        public Action FixedUpdateAction;

        private void FixedUpdate()
        {
            this.FixedUpdateAction?.Invoke();
            if (this.m_DelayFrame > 0f || !IsUseSimulate) return;
            if (this.transform.parent == null)
            {
                return;
            }
            if (this.m_CurAnimProcess.IsJumping)
            {
                this.UpdateNoGroundPos();
            }

            {
                this.UpdateInGroundPos();
            }
        }

        private void UpdateNoGroundPos()
        {
            Transform curTrans = this.transform;
            Vector3 pos = this.transform.localPosition;
            Vector3 groundPos = this.transform.parent.GetChild(1).localPosition;
            this.IsInGround = !(pos.y > groundPos.y);
            
            float moveX = this.m_CurAnimProcess.SpeedX * Time.fixedDeltaTime;
            float moveY = this.m_CurAnimProcess.SpeedSelfY * Time.fixedDeltaTime;
            
            curTrans.localPosition += new Vector3(0, moveY);
            curTrans.parent.position += new Vector3(moveX, 0);
            if (pos.y + moveY < groundPos.y)
            {
                curTrans.localPosition = groundPos;
                this.m_CurAnimProcess.SpeedSelfY = 0;
                this.m_CurAnimProcess.SpeedX = 0;
                this.m_CurAnimProcess.IsJumping = false;
                this.IsInGround = true;
            }
            this.m_CurAnimProcess.SpeedSelfY -= Time.fixedDeltaTime * this.m_G;
        }

        private void UpdateInGroundPos()
        {
            Vector3 moveValue = new Vector3(this.m_CurAnimProcess.SpeedX, this.m_CurAnimProcess.SpeedRootY) * Time.fixedDeltaTime;
            this.transform.parent.position += moveValue;
        }

        public void AttachNormalSpeed(Vector2 speed)
        {
            this.m_CurAnimProcess.SpeedX = speed.x;
            this.m_CurAnimProcess.SpeedSelfY = speed.y;
            this.m_CurAnimProcess.IsJumping = true;
        }
        
        public void AttachMoveSpeed(Vector2 speed)
        {
            this.skeleton.ScaleX = (int) (speed.x * 100) == 0 ? this.skeleton.ScaleX : speed.x > 0 ? 1 : -1;
            this.GetComponent<Renderer>().sortingOrder = (int) (-this.transform.parent.position.y * 1000);
            this.m_CurAnimProcess.SpeedX = speed.x;
            this.m_CurAnimProcess.SpeedRootY = speed.y;
        }

        public void AttachSpeedSelfY(float speedY)
        {
            this.m_CurAnimProcess.SpeedSelfY = speedY;
        }
    }

    public class AnimInfo
    {
        public float SpeedX;
        public float SpeedSelfY;
        public float SpeedRootY;
        public bool IsJumping = false;
    }
}