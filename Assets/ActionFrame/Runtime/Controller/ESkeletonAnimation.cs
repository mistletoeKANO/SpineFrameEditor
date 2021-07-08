using System;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace ActionFrame.Runtime
{
    [AddComponentMenu("Spine/ESkeletonAnimation")]
    public sealed partial class ESkeletonAnimation : SkeletonAnimation
    {
        [SerializeField]
        private TextAsset eSpineCtrJsonFile;

        private ESpineControllerData m_ESpineCtrData;

        private Spine.TrackEntry m_ForwardTrack;
        private Spine.TrackEntry m_CurrentTrack;

        public Spine.TrackEntry ForwardTrack
        {
            get => this.m_ForwardTrack;
            set => this.m_ForwardTrack = value;
        }
        public Spine.TrackEntry CurrentTrack
        {
            get => this.m_CurrentTrack;
            set => this.m_CurrentTrack = value;
        }

        /// <summary>
        /// 延迟帧数
        /// </summary>
        private int m_DelayFrame = 0;
        
        /// <summary>
        /// 执行的总帧数
        /// </summary>
        private int m_RunFrameCount = 0;

        public TextAsset ESpineCtrJsonFile
        {
            get => this.eSpineCtrJsonFile;
        }

        public override void Start()
        {
            base.Start();
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            this.Init();
        }

        public void Init()
        {
            if (this.AnimationState == null)
            {
                return;
            }
            this.AnimationState.ClearTracks();
            if (this.eSpineCtrJsonFile == null || string.IsNullOrEmpty(eSpineCtrJsonFile.text))
            {
                return;
            }
            try
            {
                this.m_ESpineCtrData = JsonHelper.ToObject<ESpineControllerData>(eSpineCtrJsonFile.text);
                this.InitJsonData(this.m_ESpineCtrData);
            }
            catch (Exception e)
            {
                throw new Exception($"Deserialize json data failure with {e.Message}");
            }
            this.InitState();
        }
        
        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            if (this.m_CurrentTrack == null)
            {
                return;
            }
            if (m_DelayFrame > 0)
            {
                m_DelayFrame--;
                return;
            }
            this.m_RunFrameCount++;
            this.Update(Time.deltaTime);
        }

        private void InitState()
        {
            if (this.m_DefaultState == null)
            {
                Debug.LogError($"State init failure.");
                return;
            }
            this.m_CurrentTrack =
                this.AnimationState.SetAnimation(0, this.m_DefaultState.StateName, this.m_DefaultState.IsLoop);
            this.m_CurrentState = this.m_DefaultState;
            this.m_RunFrameCount = 0;
        }

        public Spine.TrackEntry ChangeState(string stateName, bool isLoop)
        {
            if (this.AnimationState.GetCurrent(0) != null && this.AnimationState.GetCurrent(0).Animation.Name == stateName)
            {
                return this.AnimationState.GetCurrent(0);
            }
            this.skeleton.SetToSetupPose();
            this.AnimationState.ClearTracks();
            m_CurrentTrack = this.AnimationState.SetAnimation(0, stateName, isLoop);
            m_CurrentTrack.Complete += this.StateComplete;
            this.m_CurrentState = this.GetStateData(stateName);
            return m_CurrentTrack;
        }

        public Spine.TrackEntry ChangeStateWithMix(string stateName, bool isLoop, float mixTime)
        {
            if (this.AnimationState.GetCurrent(0) != null && this.AnimationState.GetCurrent(0).Animation.Name == stateName)
            {
                return this.AnimationState.GetCurrent(0);
            }
            this.m_ForwardTrack = this.m_CurrentTrack;
            this.m_CurrentTrack = this.AnimationState.SetAnimation(0, stateName, isLoop);
            m_CurrentTrack.Complete += this.StateComplete;
            this.m_CurrentTrack.MixBlend = MixBlend.Setup;
            this.m_CurrentTrack.MixDuration = mixTime;
            this.m_CurrentState = this.GetStateData(stateName);
            return this.m_CurrentTrack;
        }

        private void StateComplete(TrackEntry entry)
        {
            if (!entry.Loop)
            {
                StateData next = this.GetStateData(this.m_CurrentState.NextStateName);
                if (this.m_CurrentState.TransitionTime > 0)
                {
                    this.ChangeStateWithMix(next.StateName, next.IsLoop, this.m_CurrentState.TransitionTime);
                }
                else
                {
                    this.ChangeState(next.StateName, next.IsLoop);
                }
            }
        }
    }
}