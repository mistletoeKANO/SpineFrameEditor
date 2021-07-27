using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ActionFrame.Runtime
{
    public sealed partial class ESkeletonAnimation
    {
        public float FrameTime
        {
            get => 1f / this.m_FrameRate;
        }

        public int FrameRate
        {
            get => this.m_FrameRate;
        }
        private float m_CurLogicTime = 0f;
        
        private StateData m_DefaultState;
        private StateData m_CurrentState;
        private StateData m_NextState;
        private Dictionary<string, List<BaseHandle>> m_HandleDic;

        public StateData DefaultState
        {
            get => this.m_DefaultState;
        }

        public StateData CurrentState
        {
            get => this.m_CurrentState;
        }
        
        private void InitJsonData(ESpineControllerData ctrData)
        {
            this.m_DefaultState = this.m_ESpineCtrData.m_EntryState;
            this.m_HandleDic = new Dictionary<string, List<BaseHandle>>();
            foreach (var stateData in this.m_ESpineCtrData.m_StateDataList)
            {
                List<BaseHandle> handles = new List<BaseHandle>();
                this.m_HandleDic.Add(stateData.StateName, handles);
                this.InitStateDataInternal(stateData, handles);
            }
        }

        private void InitStateDataInternal(StateData stateData, List<BaseHandle> handles)
        {
            if (stateData == null)
            {
                return;
            }
            foreach (var obj in stateData.EventList)
            {
                System.Type type = obj.GetType();
                BehaviourConfigAttribute attr = (BehaviourConfigAttribute)type.GetCustomAttribute(typeof(BehaviourConfigAttribute), false);
                BaseHandle baseHandle = (BaseHandle)Activator.CreateInstance(attr.HandleType);
                baseHandle.InitConfig(obj);
                handles.Add(baseHandle);
            }
        }

        private void StartHandle()
        {
            if (this.m_CurrentState == null || !this.m_HandleDic.ContainsKey(this.m_CurrentState.StateName))
            {
                return;
            }
            List<BaseHandle> handles = this.m_HandleDic[this.m_CurrentState.StateName];
            foreach (var item in handles)
            {
                item.StartHandle(this);
            }
        }
        
        private void ExitHandle()
        {
            if (this.m_CurrentState == null || !this.m_HandleDic.ContainsKey(this.m_CurrentState.StateName))
            {
                return;
            }
            List<BaseHandle> handles = this.m_HandleDic[this.CurrentState.StateName];
            foreach (var item in handles)
            {
                item.ExitHandle(this);
            }
        }

        private void UpdateHandle(float dealtTime, float updateDealt)
        {
            if (this.m_CurrentState == null || !this.m_HandleDic.ContainsKey(this.m_CurrentState.StateName))
            {
                return;
            }
            List<BaseHandle> handles = this.m_HandleDic[this.m_CurrentState.StateName];
            int curFrame = Mathf.RoundToInt((this.m_CurrentTrack.AnimationTime + updateDealt) * this.m_FrameRate);

            foreach (var item in handles)
            {
                BehaviourData data = (BehaviourData) item.config;
                int startFrame = Mathf.RoundToInt(data.BehaviourFrameStartTime * this.m_FrameRate);
                int endFrame = Mathf.RoundToInt(data.BehaviourFrameEndTime * this.m_FrameRate);
                if (curFrame < startFrame || curFrame > endFrame)
                {
                    continue;
                }
                item.UpdateHandle(this, dealtTime);
            }
        }

        public StateData GetStateData(string stateName)
        {
            foreach (var value in this.m_ESpineCtrData.m_StateDataList)
            {
                if (value.StateName == stateName)
                {
                    return value;
                }
            }
            Debug.LogError($"{stateName} is not exist in current animController.");
            return null;
        }
    }
}