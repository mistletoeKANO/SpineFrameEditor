using System;
using UnityEngine;

namespace ActionFrame.Runtime
{
    public class GUIManager : MonoBehaviour
    {
        private GUIStyle topAreaSty;
        private GUIStyle labelSty;
        private GUIStyle valueSty;
        
        private int m_RenderFrameRate;
        private int m_RunRenderFrameIndex;
        private float m_CurRenderRunTime = 0f;
        
        //逻辑帧时长
        private readonly float m_LogicFrameTime = 1 / 30f;
        private float m_CurLogicTime = 0f;
        private int m_LogicFrameRate = 0;
        private int m_LogicFrameIndex = 0;
        private float m_CurLogicRunTime = 0f;

        private void Update()
        {
            this.RenderUpdate();
            this.LogicUpdate();
        }

        private void RenderUpdate()
        {
            if (this.m_CurRenderRunTime <= 1f)
            {
                this.m_CurRenderRunTime += Time.deltaTime;
                this.m_RunRenderFrameIndex++;
                return;
            }
            this.m_CurRenderRunTime = 0f;
            this.m_RenderFrameRate = m_RunRenderFrameIndex;
            this.m_RunRenderFrameIndex = 0;
        }

        private void LogicUpdate()
        {
            if (this.m_CurLogicRunTime <= 1f)
            {
                this.m_CurLogicRunTime += Time.deltaTime;
                this.m_CurLogicTime += Time.deltaTime;
                if (this.m_CurLogicTime >= this.m_LogicFrameTime)
                {
                    this.m_CurLogicTime -= this.m_LogicFrameTime;
                    this.m_LogicFrameIndex++;
                }
                return;
            }
            this.m_CurLogicRunTime = 0f;
            this.m_LogicFrameRate = this.m_LogicFrameIndex;
            this.m_LogicFrameIndex = 0;
        }

        private void OnGUI()
        {
            this.InitStyle();
            GUILayout.BeginHorizontal(topAreaSty,GUILayout.Width(Screen.width), GUILayout.Height(80));
            
            GUILayout.Label($"渲染帧率 {this.m_RenderFrameRate} FPS", this.labelSty, GUILayout.Width(260));
            GUILayout.Label($"逻辑帧率 {this.m_LogicFrameRate} FPS", this.labelSty, GUILayout.Width(260));
            GUILayout.FlexibleSpace();
            GUILayout.Label("操作: ↑↓←→ 移动(加LeftShift 奔跑), A 攻击, Space 跳跃", this.valueSty);

            GUILayout.EndHorizontal();
        }

        private void InitStyle()
        {
            if (this.topAreaSty == null)
            {
                topAreaSty = new GUIStyle(GUI.skin.box);
                topAreaSty.padding = new RectOffset(20,20,20,20);
            }

            if (this.labelSty == null)
            {
                labelSty = new GUIStyle(GUI.skin.label);
                labelSty.fontSize = 30;
                labelSty.normal.textColor = new Color(0f, 1f, 1f, 0.57f);
                labelSty.alignment = TextAnchor.MiddleLeft;
                
                valueSty = new GUIStyle(labelSty);
                valueSty.normal.textColor = new Color(1f, 0.92f, 0.02f, 0.56f);
            }
        }
    }
}