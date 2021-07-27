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
            if (this.m_DelayFrame > 0f || !IsUseSimulate) return;
            this.UpdateInGroundPos();
        }

        private void UpdateInGroundPos()
        {
            Vector3 moveValue = new Vector3(this.m_CurAnimProcess.SpeedX, this.m_CurAnimProcess.SpeedRootY) * Time.fixedDeltaTime;
            this.transform.position += moveValue;
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