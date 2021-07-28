using UnityEngine;

namespace ActionFrame.Runtime
{
    public class ERigidbody2D : MonoBehaviour
    {
        private float m_G = 9.8f;
        private Vector2 m_Velocity;
        public Vector2 Velocity => m_Velocity;

        private float m_GroundPosY;
        private bool m_IsSimulate;

        private float m_LiveTime = 3f;
        private float m_RunTime = 0f;
        private void FixedUpdate()
        {
            this.m_RunTime += Time.fixedDeltaTime;
            if (this.m_RunTime >= this.m_LiveTime)
            {
                Destroy(this.gameObject);
            }
            if (!m_IsSimulate) return;
            Vector3 moveValue = this.m_Velocity * Time.fixedDeltaTime;
            if (this.transform.position.y + moveValue.y <= this.m_GroundPosY)
            {
                this.m_IsSimulate = false;
            }
            else
            {
                this.transform.position += moveValue;
            }
            this.m_Velocity.y -= Time.fixedDeltaTime * this.m_G;
        }

        public void Init(Vector2 initSpeed, float groundPos = -int.MaxValue, float g = 9.8f)
        {
            this.m_Velocity = initSpeed;
            this.m_GroundPosY = groundPos;
            this.m_G = g;
            this.m_IsSimulate = true;
        }
    }
}