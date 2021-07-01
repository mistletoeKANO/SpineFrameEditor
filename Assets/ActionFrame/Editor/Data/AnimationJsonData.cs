using System;
using System.IO;
using ActionFrame.Runtime;
using UnityEditor;
using UnityEngine;

namespace ActionFrame.Editor
{
    public class AnimationJsonData
    {
        private TextAsset m_CurJsonData;
        private ESpineControllerData m_SpineCtr;

        public ESpineControllerData SpineController
        {
            get => this.m_SpineCtr;
        }

        public AnimationJsonData(TextAsset txt)
        {
            this.m_CurJsonData = txt;
            this.m_SpineCtr = Runtime.JsonHelper.ToObject<ESpineControllerData>(m_CurJsonData.text);
            if (this.m_SpineCtr == null)
            {
                this.m_SpineCtr = new ESpineControllerData();
            }
        }

        public StateData GetStateWithName(string stateName)
        {
            foreach (var stateData in this.m_SpineCtr.m_StateDataList)
            {
                if (stateData.StateName == stateName)
                {
                    return stateData;
                }
            }
            return null;
        }

        /// <summary>
        /// 重置入口状态
        /// </summary>
        /// <param name="stateData"></param>
        public void ResetEntryState(StateData stateData)
        {
            this.m_SpineCtr.m_EntryState = stateData;
        }

        public void ResetEntryState(string stateName)
        {
            this.m_SpineCtr.m_EntryState = this.GetStateWithName(stateName);
        }

        public bool IsStateExist(string stateName)
        {
            foreach (var stateData in this.m_SpineCtr.m_StateDataList)
            {
                if (stateData.StateName == stateName)
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// 新增状态
        /// </summary>
        /// <param name="stateData"></param>
        public void AddState(StateData stateData)
        {
            this.m_SpineCtr.m_StateDataList.Add(stateData);
        }

        /// <summary>
        /// 移除状态
        /// </summary>
        /// <param name="state"></param>
        public void RemoveState(StateData state)
        {
            this.m_SpineCtr.m_StateDataList.Remove(state);
        }

        public void RemoveState(string stateName)
        {
            int stateCount = this.m_SpineCtr.m_StateDataList.Count;
            for (int i = 0; i < stateCount; i++)
            {
                if (this.m_SpineCtr.m_StateDataList[i].StateName == stateName)
                {
                    this.m_SpineCtr.m_StateDataList.RemoveAt(i);
                    break;
                }
            }
        }

        public bool Save()
        {
            try
            {
                string str = Runtime.JsonHelper.ToJson(this.m_SpineCtr);
                string path = AssetDatabase.GetAssetPath(this.m_CurJsonData.GetInstanceID());
                using (StreamWriter file = new StreamWriter(path))
                {
                    file.Write(str);
                    file.Close();
                }
                AssetDatabase.Refresh();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }
    }
}