using System;
#if UNITY_EDITOR
using System.Collections.Generic;
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
        
        private readonly Color m_GizMorBeHit = new Color(0f, 0f, 1f, 0.4f);
        private readonly Color m_GizMorAttack = new Color(1f, 0f, 0.02f, 0.4f);

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
            this.m_CurrentTrack = this.ChangeStateWithMix(stateData.StateName, stateData.IsLoop, stateData.TransitionTime);
            this.m_CurrentTrack.TimeScale = timeSca;
        }
        
        public void UpdateFrameEditor(float dealtTime)
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
        
        private void OnDrawGizmos()
        {
            if (this.m_CurrentState != null && this.m_CurrentState.UseCurSBeHitBox)
            {
                List<BoxItem> boxItems = null;
                if (this.m_CurrentState.FrameDic.ContainsKey(0) && this.m_CurrentState.FrameDic[0].ApplyBeHitBoxToAllFrame)
                {
                    boxItems = this.m_CurrentState.FrameDic[0].BeHitRangeList;
                }
                else if (this.m_CurrentState.FrameDic.ContainsKey(this.m_RunFrameCount))
                {
                    boxItems = this.m_CurrentState.FrameDic[this.m_RunFrameCount].BeHitRangeList;
                }
                if (boxItems != null)
                {
                    this.DrawBodyBox(boxItems, this.m_GizMorBeHit);
                }
            }
            else if (this.m_DefaultState != null && this.m_DefaultState.FrameDic[0] != null)
            {
                FrameData data = this.m_DefaultState.FrameDic[0];
                if (data != null)
                {
                    this.DrawBodyBox(data.BeHitRangeList, this.m_GizMorBeHit);
                }
            }

            if (this.m_CurrentState != null)
            {
                int frameIndex = Mathf.RoundToInt(this.m_CurrentTrack.AnimationTime * this.FrameRate);
                if (!this.m_CurrentState.FrameDic.ContainsKey(frameIndex))
                {
                    return;
                }
                FrameData data = this.m_CurrentState.FrameDic[frameIndex];
                if (data == null) return;
                this.DrawBodyBox(data.AttackedRangeList, this.m_GizMorAttack);
            }
        }

        private void DrawBodyBox(List<BoxItem> boxItems, Color color)
        {
            HandlesDrawer.H.PushColor(color);
            HandlesDrawer.H.fillPolygon = true;
            HandlesDrawer.H.useFillPolygonOutline = true;
            foreach (var boxItem in boxItems)
            {
                Vector3 pos = new Vector3(boxItem.Offset.x * this.skeleton.ScaleX, boxItem.Offset.y);
                Matrix4x4 matrix = Matrix4x4.Translate(pos + this.transform.position);
                matrix = MathUtility.MatrixRotate(matrix, boxItem.Rotation);
                HandlesDrawer.H.DrawRect(boxItem.Size, matrix);
            }
            HandlesDrawer.H.PopColor();
        }

        [MenuItem("GameObject/Spine/ESkeletonAnimation", false, 10)]
        private static void CreateESkeletonAnimation()
        {
            GameObject obj = new GameObject("ESkeletonAnimation", typeof(ESkeletonAnimation), typeof(SkeletonUtility));
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