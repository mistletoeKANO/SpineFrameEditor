
using System;
#if UNITY_EDITOR
using Spine.Unity;
using UnityEditor;
using UnityEngine;

#endif

namespace ActionFrame.Runtime
{
    public sealed partial class ESkeletonAnimation
    {
#if UNITY_EDITOR
        [NonSerialized]
        public Action<object> JsonFileChangeEvent;

        public int RunFrameCount
        {
            get => Mathf.RoundToInt(this.m_CurrentTrack.AnimationTime * 30);
        }

        public int DelayFrame
        {
            get => this.m_DelayFrame;
            set => this.m_DelayFrame = value;
        }

        public void InitAnimState(StateData stateData, StateData defaultData, float timeSca)
        {
            this.AnimationState.ClearTracks();
            this.skeleton.SetToSetupPose();
            this.m_RunFrameCount = 0;
            if (defaultData == null)
            {
                Debug.LogWarning("初始状态为空 ...");
            }
            this.m_DefaultState = defaultData;
            this.m_CurrentTrack = this.ChangeState(stateData.StateName, stateData.IsLoop);
            this.m_CurrentTrack.TimeScale = timeSca;
        }
        
        public void UpdateFrame(float dealtTime)
        {
            if (m_DelayFrame > 0)
            {
                m_DelayFrame--;
                return;
            }
            
            this.Update(dealtTime);
        }

        public void SetCurTrackTimeScale(float value)
        {
            this.m_CurrentTrack.TimeScale = value;
        }

        public void ResetSelectFramePosture(string stateName, float time)
        {
            this.m_CurrentTrack = this.AnimationState.SetAnimation(0, stateName, false);
            this.m_CurrentTrack.AnimationEnd = time;
            this.Update(int.MaxValue);
            
        }

        [MenuItem("GameObject/Spine/ESkeletonAnimation")]
        private static void CreateESkeletonAnimation()
        {
            GameObject obj = new GameObject("ESkeletonAnimation", typeof(MeshFilter), 
                typeof(MeshRenderer),
                typeof(ESkeletonAnimation),
                typeof(SkeletonUtility));
            string filePath = UnityEditor.EditorUtility.OpenFilePanel("选择Spine资源文件", Application.dataPath, "asset");
            if (string.IsNullOrEmpty(filePath))
            {
                DestroyImmediate(obj);
                return;
            };
            int index = filePath.IndexOf("Assets", StringComparison.Ordinal);
            string result = filePath.Substring(index);
            Spine.Unity.SkeletonDataAsset dataAsset = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(result);
            if (dataAsset == null)
            {
                DestroyImmediate(obj);
                UnityEditor.EditorUtility.DisplayDialog("文件错误", "选择的资源文件类型错误...", "确定");
                return;
            };
            obj.GetComponent<ESkeletonAnimation>().skeletonDataAsset = dataAsset;
            obj.GetComponent<SkeletonUtility>().SpawnHierarchy(SkeletonUtilityBone.Mode.Follow, true, true, true);
            obj.GetComponent<SkeletonUtility>().boneRoot = obj.transform.GetChild(0);
        }
#endif
    }
}