using System;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace ActionFrame.Runtime
{
    [AddComponentMenu("Spine/ESkeletonAnimation")]
    [RequireComponent(typeof(ESkeletonState))]
    public sealed partial class ESkeletonAnimation : SkeletonAnimation
    {
        private readonly int m_FrameRate = 30;
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
        
        [SerializeField]
        private float m_TimeScale = 1f;
        public float TimeScale
        {
            get => this.m_TimeScale;
            set => this.m_TimeScale = value;
        }

        /// <summary>
        /// 延迟帧数
        /// </summary>
        private int m_DelayFrame = 0;
        
        /// <summary>
        /// 执行的总帧数
        /// </summary>
        private int m_RunFrameCount = -1;
        public int CurrentFrameCount { get => this.m_RunFrameCount; }
        
        /// <summary>
        /// 当前 track 执行到的 帧数
        /// </summary>
        public int RunFrameCount
        {
            get => Mathf.RoundToInt(this.m_CurrentTrack.AnimationTime * 30);
        }
        
        private float m_RunAnimTime = 0;
        private float m_UpdateLogicTime = 0;

        public TextAsset ESpineCtrJsonFile
        {
            get => this.eSpineCtrJsonFile;
        }

        public override void Start()
        {
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
            this.UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            if (this.m_RunAnimTime <= 0)
            {
                return;
            }
            float updateTime = 0f;
            if (this.m_RunAnimTime - Time.deltaTime * this.m_TimeScale > 0)
            {
                updateTime = Time.deltaTime * this.m_TimeScale;
                this.m_RunAnimTime -= Time.deltaTime * this.m_TimeScale;
            }
            else
            {
                updateTime = this.m_RunAnimTime;
                this.m_RunAnimTime = 0f;
            }
            this.Update(updateTime);
        }

        public void UpdateLogic(float dealtTime)
        {
            if (this.m_CurrentTrack == null)
            {
                return;
            }
            if (m_DelayFrame > 0)
            {
                m_DelayFrame--;
                return;
            }
            this.m_RunAnimTime += dealtTime * this.m_TimeScale;
            this.m_RunFrameCount += 1;
            //处理当前状态行为
            this.m_UpdateLogicTime += dealtTime * this.m_TimeScale;
            if (this.m_UpdateLogicTime >= dealtTime)
            {
                float updateDealt = 0;
                while (this.m_UpdateLogicTime - dealtTime >= 0)
                {
                    updateDealt += dealtTime;
                    this.m_UpdateLogicTime -= dealtTime;
                    this.UpdateHandle(dealtTime, updateDealt);
                }
            }
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
            this.m_CurrentTrack.Complete += this.StateComplete;
            this.m_CurrentState = this.m_DefaultState;
            this.m_RunFrameCount = 0;
            this.m_RunAnimTime = 0;
            this.m_UpdateLogicTime = 0;
        }

        public Spine.TrackEntry ChangeStateWithMix(string stateName, bool isLoop, float mixTime = 0f, float animStartTime = 0f)
        {
            if (this.AnimationState.GetCurrent(0) != null && this.AnimationState.GetCurrent(0).Animation.Name == stateName)
            {
                return this.AnimationState.GetCurrent(0);
            }
            return this.ChangeStateInternal(stateName, isLoop, mixTime, animStartTime);
        }

        public Spine.TrackEntry ChangeStateNoCtr(string stateName, bool isLoop, float mixTime = 0f, float animStartTime = 0f)
        {
            return this.ChangeStateInternal(stateName, isLoop, mixTime, animStartTime);
        }

        private Spine.TrackEntry ChangeStateInternal(string stateName, bool isLoop, float mixTime = 0f, float animStartTime = 0f)
        {
            this.ExitHandle();
            this.m_RunFrameCount = 0;
            this.m_RunAnimTime = 0;
            this.m_UpdateLogicTime = 0;
            if (mixTime > 0)
            {
                this.m_ForwardTrack = this.m_CurrentTrack;
                this.m_CurrentTrack = this.AnimationState.SetAnimation(0, stateName, isLoop);
                this.m_CurrentTrack.MixBlend = MixBlend.Setup;
                this.m_CurrentTrack.MixDuration = mixTime;
            }
            else
            {
                this.skeleton.SetBonesToSetupPose();
                this.AnimationState.ClearTracks();
                this.m_CurrentTrack = this.AnimationState.SetAnimation(0, stateName, isLoop);
            }
            this.m_RunFrameCount = Mathf.RoundToInt(animStartTime * this.FrameTime);
            this.m_CurrentTrack.AnimationStart = animStartTime;
            this.m_CurrentState = this.GetStateData(stateName);
            this.m_CurrentTrack.Complete += this.StateComplete;
            this.StartHandle();
            return this.m_CurrentTrack;
        }

        private void StateComplete(TrackEntry entry)
        {
            if (!entry.Loop)
            {
                StateData next = this.GetStateData(this.m_CurrentState.NextStateName);
                if (next == null) return;
                this.ChangeStateWithMix(next.StateName, next.IsLoop, this.m_CurrentState.TransitionTime);
            }
        }
        
        public void SetCurTrackTimeScale(float value)
        {
            this.m_CurrentTrack.TimeScale = value;
        }

        /// <summary>
        /// 设置子弹时间(顿帧 帧数) 单位 帧数
        /// </summary>
        /// <param name="frame"></param>
        public void SetBullet(int frame)
        {
            this.m_DelayFrame += Mathf.Abs(frame);
        }
        
        /// <summary>
        /// 设置子弹时间, 持续时长 单位 秒
        /// </summary>
        /// <param name="time"></param>
        public void SetBullet(float time)
        {
            int frameCount = Mathf.RoundToInt(time / this.FrameTime);
            this.m_DelayFrame += Mathf.Abs(frameCount);
        }
    }
}