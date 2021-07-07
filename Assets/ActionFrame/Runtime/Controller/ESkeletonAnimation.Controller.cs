using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ActionFrame.Runtime
{
    public sealed partial class ESkeletonAnimation
    {
        private StateData m_DefaultState;
        private StateData m_CurrentState;
        private StateData m_NextState;
        private Dictionary<string, List<BaseHandle>> m_HandleDic;
        
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

        private void UpdateInput(float dealtTime)
        {
            List<BaseHandle> handles = this.m_HandleDic[this.m_CurrentState.StateName];
            foreach (var item in handles)
            {
                BehaviourData data = (BehaviourData) item.config;
                int curFrame = Mathf.RoundToInt(this.m_CurrentTrack.AnimationTime * 30);
                int startFrame = Mathf.RoundToInt(data.BehaviourFrameStartTime * 30);
                int endFrame = Mathf.RoundToInt(data.BehaviourFrameEndTime * 30);
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
            return null;
        }
    }
}