﻿using Spine;
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

        private StateData m_DefaultState;
        private StateData m_CurrentState;
        private StateData m_NextState;
        
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
            this.Init();
        }

        public void Init()
        {
            this.AnimationState.ClearTracks();
            if (this.eSpineCtrJsonFile == null || string.IsNullOrEmpty(eSpineCtrJsonFile.text))
            {
                return;
            }
            this.m_ESpineCtrData = JsonHelper.ToObject<ESpineControllerData>(eSpineCtrJsonFile.text);
            this.m_DefaultState = this.m_ESpineCtrData.m_EntryState;
            if (this.m_DefaultState == null)
            {
                return;
            }
            this.m_CurrentTrack = this.ChangeState(this.m_DefaultState.StateName, this.m_DefaultState.IsLoop);
        }
        
        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            
            if (m_DelayFrame > 0)
            {
                m_DelayFrame--;
                return;
            }
            this.m_RunFrameCount++;
            this.Update(Time.deltaTime);
        }

        private Spine.TrackEntry ChangeState(string stateName, bool isLoop)
        {
            if (this.AnimationState.GetCurrent(0) != null && this.AnimationState.GetCurrent(0).Animation.Name == stateName)
            {
                return this.AnimationState.GetCurrent(0);
            }
            this.skeleton.SetToSetupPose();
            this.AnimationState.ClearTracks();
            m_CurrentTrack = this.AnimationState.SetAnimation(0, stateName, isLoop);
            this.loop = isLoop;
            m_CurrentTrack.Event += this.TriggerCustomEvent;
            if (!isLoop)
            {
                m_CurrentTrack.Complete += entry =>
                {
                    if (this.m_DefaultState == null)
                    {
                        return;
                    }
                    this.ChangeState(this.m_DefaultState.StateName, this.m_DefaultState.IsLoop);
                };
            }
            return m_CurrentTrack;
        }

        private void TriggerCustomEvent(TrackEntry trackEntry, Spine.Event e)
        {
            
        }
    }
}