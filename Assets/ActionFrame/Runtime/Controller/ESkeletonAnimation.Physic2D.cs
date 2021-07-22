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
        public bool IsUseSimulate = true;
        public bool IsInGround = true;

        private void FixedUpdate()
        {
            if (this.DelayFrame > 0f || !IsUseSimulate) return;
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
            Vector3 pos = this.transform.localPosition;
            this.IsInGround = !(pos.y > 0f);
            
            if (Mathf.Abs(this.m_CurAnimProcess.SpeedSelfY) > 0f)
            {
                float moveX = this.m_CurAnimProcess.SpeedX * Time.fixedDeltaTime;
                float moveY = this.m_CurAnimProcess.SpeedSelfY * Time.fixedDeltaTime;
                Transform curTrans = this.transform;
                curTrans.localPosition += new Vector3(0, moveY);
                curTrans.parent.position += new Vector3(moveX, 0);
                if (pos.y + moveY < 0f)
                {
                    curTrans.localPosition = Vector3.zero;
                    this.m_CurAnimProcess.SpeedSelfY = 0;
                    this.m_CurAnimProcess.SpeedX = 0;
                    this.m_CurAnimProcess.IsJumping = false;
                }
            }
            this.m_CurAnimProcess.SpeedSelfY -= Time.fixedDeltaTime * this.m_G;
        }

        private void UpdateInGroundPos()
        {
            Vector3 moveValue = new Vector3(this.m_CurAnimProcess.SpeedX * Time.fixedDeltaTime, this.m_CurAnimProcess.SpeedRootY * Time.fixedDeltaTime);
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
            this.m_CurAnimProcess.SpeedX = speed.x;
            this.m_CurAnimProcess.SpeedRootY = speed.y;
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