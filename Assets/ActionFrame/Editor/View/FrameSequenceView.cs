using System.Collections.Generic;
using ActionFrame.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ActionFrame.Editor
{
    /// <summary>
    /// 帧序列视图
    /// </summary>
    public class FrameSequenceView : BaseView
    {
        private static readonly int FrameBoxWidth = 48;
        private static readonly int BehaviourItemHeight = 32;
        private static readonly int BehaviourListWidth = 160;
        private int m_ScrollContainerWidth = BehaviourListWidth;
        
        private static readonly int kDragSelectionControlID = "DragSelection".GetHashCode();
        private static readonly int kDragLeftHndleControlID = "DragLeftHandle".GetHashCode();
        private static readonly int kDragRightHandleControlID = "DragRightHandle".GetHashCode();
        
        private AnimationStateView m_StateView;
        private FrameInfoView m_FrameInfoView;

        #region ToolBar
        
        private Toolbar m_FrameSeqToolBar;
        private ToolbarButton m_ToolBarPlayBtn;
        private Image m_ToolBarPlayBtnIcon;
        private ToolbarButton m_ToolBarPauseBtn;
        private Image m_ToolBarPauseBtnIcon;
        private ToolbarButton m_ToolBarNextFrameBtn;
        private Image m_ToolBarNextFrameBtnIcon;
        private Slider m_AnimSpeedSlider;
        private Label m_SpeedValue;
        
        #endregion
        
        private VisualElement m_BehaviourListContainer;
        private ScrollView m_ScrollView;

        private List<BehaviourEventInfo> m_BehaviourList;
        private BehaviourEventInfo m_SelectedBehaviour;
        
        private Dictionary<int, FrameInfoInternal> m_FrameDic;
        private FrameInfoInternal m_CurSelectFrame;
        private FrameInfoInternal m_DrawGizMorFrame;

        internal FrameInfoInternal CurSelectFrame
        {
            get => this.m_CurSelectFrame;
            set => this.m_CurSelectFrame = value;
        }

        private bool m_IsPLay = false;
        private bool m_IsPause = false;
        
        private float m_LastFrameTime = 0f;
        
        /// <summary>
        /// 当前状态执行总帧数
        /// </summary>
        private int m_RunFrameCount = -1;
        
        private float m_DragBehaviourBodyStartPos = 0f;
        private bool m_IsInDragHandle = false;

        private object m_CopyBuffer;

        public override void Init(EditorWindow host, VisualElement parent)
        {
            this.m_Host = (ActionFrameWindow) host;
            this.m_ParentView = parent;
            this.m_FrameDic = new Dictionary<int, FrameInfoInternal>();
            this.m_BehaviourList = new List<BehaviourEventInfo>();
            this.m_CurView = new IMGUIContainer();
            this.m_CurView.name = "FrameSeqOuter";
            this.DoFrameSeqView();
            this.m_ParentView.Add(this.m_CurView);
        }

        public void InitStateView(AnimationStateView view, FrameInfoView infoView)
        {
            this.m_StateView = view;
            this.m_FrameInfoView = infoView;
        }
        
        public void ViewUpdate()
        {
            this.UpdateHeroAnimation();
        }

        public void OnSceneViewGUI()
        {
            if (this.m_CurSelectFrame == null)
            {
                return;
            }
            
            if (this.m_DrawGizMorFrame != null)
            {
                Matrix4x4 localToWorld = this.m_Host.PrefabData.PrefabTransform.localToWorldMatrix;
                SceneViewDrawer.DrawRect(localToWorld, this.m_DrawGizMorFrame.FrameData.BeHitRangeList, AcFrameStyle.HitBoxGizMo);
            }

            if (this.m_CurSelectFrame.FrameData == null)
            {
                return;
            }

            this.m_DrawGizMorFrame = this.m_CurSelectFrame.FrameData.ApplyBeHitBoxToAllFrame
                ? this.m_CurSelectFrame
                : null;

            if (this.m_CurSelectFrame.FrameData.BeHitRangeList.Count > 0)
            {
                if (this.m_DrawGizMorFrame == null)
                {
                    Matrix4x4 localToWorld = this.m_Host.PrefabData.PrefabTransform.localToWorldMatrix;
                    SceneViewDrawer.DrawRect(localToWorld, this.m_CurSelectFrame.FrameData.BeHitRangeList, AcFrameStyle.HitBoxGizMo);
                }
            }
            
            if (this.m_CurSelectFrame.FrameData.AttackedRangeList.Count > 0)
            {
                Matrix4x4 localToWorld = this.m_Host.PrefabData.PrefabTransform.localToWorldMatrix;
                SceneViewDrawer.DrawRect(localToWorld, this.m_CurSelectFrame.FrameData.AttackedRangeList, AcFrameStyle.AttackBoxGizMo);
            }
        }
        
        /// <summary>
        /// 更新角色 骨骼动画
        /// </summary>
        private void UpdateHeroAnimation()
        {
            if (Time.realtimeSinceStartup - m_LastFrameTime < (1f / ActionFrameWindow.FrameRate) * 1f / this.m_AnimSpeedSlider.value)
            {
                return;
            }
            this.m_LastFrameTime = Time.realtimeSinceStartup;
            if (this.m_IsPLay && !this.m_IsPause)
            {
                this.UpdateRunFrameStyle();
                this.m_Host.PrefabData.ESkeletonAnim.UpdateFrame(1f / ActionFrameWindow.FrameRate);
                this.m_RunFrameCount = Mathf.Clamp(this.m_Host.PrefabData.ESkeletonAnim.RunFrameCount, 0,
                    this.m_StateView.CurSelectedState.FrameCount);
                this.m_CurSelectFrame = this.m_FrameDic[this.m_RunFrameCount];
                this.UpdateRunFrameStyle();
            }
        }

        /// <summary>
        /// 更新运行时 帧序列 表现
        /// </summary>
        private void UpdateRunFrameStyle()
        {
            foreach (var frame in this.m_FrameDic.Values)
            {
                frame.View.style.backgroundColor = AcFrameStyle.FrameBoxNormal;
            }

            if (this.m_RunFrameCount < 0)
            {
                return;
            }
            this.m_FrameDic[(int) this.m_RunFrameCount].View.style.backgroundColor = AcFrameStyle.FrameBoxRun;
        }

        private void DoFrameSeqView()
        {
            var styleSheet = Resources.Load<StyleSheet>("Uss/FrameSequenceView");
            this.m_CurView.styleSheets.Add(styleSheet);

            this.DoToolBar();

            this.m_ScrollView = new ScrollView();
            this.m_ScrollView.showHorizontal = true;
            this.m_ScrollView.name = "FrameScrollView";
            this.m_ScrollView.contentContainer.name = "FrameScrollViewContainer";
            
            this.m_BehaviourListContainer = new VisualElement();
            this.m_BehaviourListContainer.name = "BehaviourList";
            
            Label behaviourListTitle = new Label("帧行为列表");
            behaviourListTitle.AddToClassList("ViewTitle");
            this.m_BehaviourListContainer.Add(behaviourListTitle);

            this.m_ScrollView.contentContainer.Add(this.m_BehaviourListContainer);

            this.m_CurView.Add(this.m_ScrollView);
        }

        private void DoToolBar()
        {
            this.m_FrameSeqToolBar = new Toolbar();
            this.m_FrameSeqToolBar.name = "FrameSeqToolBar";
            
            this.m_ToolBarPlayBtn = new ToolbarButton(this.OnPlayBtn);
            this.m_ToolBarPlayBtn.AddToClassList("ToolBarCubeBtn");
            this.m_ToolBarPlayBtnIcon = new Image(){image = AcFrameStyle.LoadIconWithName<Texture2D>(AcFrameStyle.Icon_Play)};
            this.m_ToolBarPlayBtn.Add(this.m_ToolBarPlayBtnIcon);
            this.m_FrameSeqToolBar.Add(this.m_ToolBarPlayBtn);
            
            this.m_ToolBarPauseBtn = new ToolbarButton(this.OnPauseBtn);
            this.m_ToolBarPauseBtn.name = "ToolBarPauseBtn";
            this.m_ToolBarPauseBtn.AddToClassList("ToolBarCubeBtn");
            this.m_ToolBarPauseBtnIcon = new Image(){image = AcFrameStyle.LoadIconWithName<Texture2D>(AcFrameStyle.Icon_Pause)};
            this.m_ToolBarPauseBtn.Add(this.m_ToolBarPauseBtnIcon);
            this.m_FrameSeqToolBar.Add(this.m_ToolBarPauseBtn);
            
            this.m_ToolBarNextFrameBtn = new ToolbarButton(this.OnNextFrameBtn);
            this.m_ToolBarNextFrameBtn.AddToClassList("ToolBarCubeBtn");
            this.m_ToolBarNextFrameBtnIcon = new Image(){image = AcFrameStyle.LoadIconWithName<Texture2D>(AcFrameStyle.Icon_NextFrame)};
            this.m_ToolBarNextFrameBtn.Add(this.m_ToolBarNextFrameBtnIcon);
            this.m_FrameSeqToolBar.Add(this.m_ToolBarNextFrameBtn);
            
            ToolbarSpacer spacer = new ToolbarSpacer();
            spacer.name = "ToolbarSpacer";
            this.m_FrameSeqToolBar.Add(spacer);
            
            this.m_AnimSpeedSlider = new Slider();
            this.m_AnimSpeedSlider.RegisterValueChangedCallback(this.OnSliderValueChange);
            this.m_AnimSpeedSlider.lowValue = 0f;
            this.m_AnimSpeedSlider.highValue = 4f;
            this.m_AnimSpeedSlider.value = 1f;
            this.m_AnimSpeedSlider.name = "AnimSpeedSlider";
            this.m_FrameSeqToolBar.Add(this.m_AnimSpeedSlider);
            
            this.m_SpeedValue = new Label($"{this.m_AnimSpeedSlider.value:F}F");
            this.m_SpeedValue.name = "SpeedValueLabel";
            this.m_FrameSeqToolBar.Add(this.m_SpeedValue);
            
            this.m_CurView.Add(this.m_FrameSeqToolBar);
        }

        /// <summary>
        /// 刷新 当前选择状态视图
        /// </summary>
        /// <param name="stateData"></param>
        public void RefreshSelectedFrameView(StateData stateData)
        {
            this.ResetSelectItemInternal();
            
            if (this.m_FrameDic.Count > 0)
            {
                foreach (var view in this.m_FrameDic.Values)
                {
                    this.m_ScrollView.contentContainer.Remove(view.View);
                }
            }
            this.m_ScrollContainerWidth = BehaviourListWidth;
            this.m_FrameDic.Clear();
            if (stateData == null)
            {
                return;
            }
            for (int i = 0; i <= stateData.FrameCount; i++)
            {
                this.m_ScrollContainerWidth += FrameBoxWidth;
                FrameInfoInternal frameInfo = new FrameInfoInternal();
                if (stateData.FrameDic.ContainsKey(i))
                {
                    frameInfo.FrameData = stateData.FrameDic[i];
                }
                frameInfo.FrameIndex = i;
                frameInfo.View = new IMGUIContainer(this.OnScrollDragMoveHro);
                string labelStr = $"{i}";
                
                frameInfo.LabelContainer = new VisualElement();
                frameInfo.LabelContainer.name = "FrameBoxLabelContainer";

                frameInfo.ViewLabel = new Label(labelStr);
                frameInfo.ViewLabel.name = "FrameBoxLabel";
                frameInfo.LabelContainer.Add(frameInfo.ViewLabel);

                this.RefreshFrameBoxLabelInternal(frameInfo);
                
                frameInfo.View.Add(frameInfo.LabelContainer);
                
                frameInfo.View.name = "FrameBox";
                frameInfo.View.style.backgroundColor = AcFrameStyle.FrameBoxNormal;
                frameInfo.View.style.width = FrameBoxWidth;
                frameInfo.View.RegisterCallback<MouseDownEvent>(e => this.OnClickOfFrameBox(e, frameInfo));
                frameInfo.View.RegisterCallback<MouseEnterEvent>(e => this.OnFrameBoxAnim(e, frameInfo));
                frameInfo.View.RegisterCallback<MouseLeaveEvent>(e => this.OnFrameBoxAnim(e, frameInfo));
                this.m_ScrollView.contentContainer.Insert(0, frameInfo.View);
                this.m_FrameDic.Add(i, frameInfo);
            }
            this.RefreshBehaviourListView();
        }

        private void ResetSelectItemInternal()
        {
            this.m_CurSelectFrame = null;
            this.m_SelectedBehaviour = null;
            this.m_FrameInfoView?.ResetSelectFrameBox(null, false);
            this.m_FrameInfoView?.ResetSelectedBehaviour(null);
        }

        public void ResetSelectFrameBoxHitBox()
        {
            this.RefreshFrameBoxLabelInternal(this.m_CurSelectFrame);
        }

        private void RefreshFrameBoxLabelInternal(FrameInfoInternal frameInfo)
        {
            
            if (frameInfo.FrameData == null)
            {
                return;
            }

            if (frameInfo.FrameData.BeHitRangeList.Count > 0)
            {
                if (frameInfo.BeHitBox == null)
                {
                    frameInfo.BeHitBox = new Box();
                    frameInfo.BeHitBox.tooltip = "当前帧受击框存在标识";
                    frameInfo.BeHitBox.name = "FrameBoxLabelBeHitBox";
                    frameInfo.LabelContainer.Add(frameInfo.BeHitBox);
                }
            }
            else
            {
                if (frameInfo.BeHitBox != null)
                {
                    frameInfo.LabelContainer.Remove(frameInfo.BeHitBox);
                    frameInfo.BeHitBox = null;
                }
            }

            if (frameInfo.FrameData.AttackedRangeList.Count > 0)
            {
                if (frameInfo.AttackBox == null)
                {
                    frameInfo.AttackBox = new Box();
                    frameInfo.AttackBox.tooltip = "当前帧攻击框存在标识";
                    frameInfo.AttackBox.name = "FrameBoxLabelAttackBox";
                    frameInfo.LabelContainer.Add(frameInfo.AttackBox);
                }
            }
            else
            {
                if (frameInfo.AttackBox != null)
                {
                    frameInfo.LabelContainer.Remove(frameInfo.AttackBox);
                    frameInfo.AttackBox = null;
                }
            }
        }

        private void OnFrameBoxWheel(WheelEvent e)
        {
            float num = this.m_ScrollView.horizontalScroller.value;
            Rect layout = this.m_ScrollView.contentContainer.layout;
            double height1 = layout.width;
            layout = this.m_ScrollView.layout;
            double height2 = layout.width;
            if (height1 - height2 > 0.0)
            {
                if (e.delta.y < 0.0)
                    this.m_ScrollView.horizontalScroller.ScrollPageUp(Mathf.Abs(e.delta.y));
                else if (e.delta.y > 0.0)
                    this.m_ScrollView.horizontalScroller.ScrollPageDown(Mathf.Abs(e.delta.y));
            }
            if (Mathf.Abs(this.m_ScrollView.horizontalScroller.value - num) <= 0.0001f)
                return;
            e.StopPropagation();
        }

        /// <summary>
        /// 水平拖拽 移动 帧视图
        /// </summary>
        private void OnScrollDragMoveHro()
        {
            if (Event.current.button != 2 || Event.current.type != EventType.MouseDrag)
            {
                return;
            }
            var e = Event.current.delta;
            e = e * 0.04f;
            float num = this.m_ScrollView.horizontalScroller.value;
            Rect layout = this.m_ScrollView.contentContainer.layout;
            double height1 = layout.width;
            layout = this.m_ScrollView.layout;
            double height2 = layout.width;
            if (height1 - height2 > 0.0)
            {
                if (e.x > 0.0)
                    this.m_ScrollView.horizontalScroller.ScrollPageUp(Mathf.Abs(e.x));
                else if (e.x < 0.0)
                    this.m_ScrollView.horizontalScroller.ScrollPageDown(Mathf.Abs(e.x));
            }
        }
        
        /// <summary>
        /// 当前鼠标悬停的 帧 Box
        /// </summary>
        private FrameInfoInternal m_EnterImgui;
        private void OnFrameBoxAnim(IMouseEvent e, FrameInfoInternal self)
        {
            if (m_IsPLay)
            {
                return;
            }
            if (e.GetType() == typeof(MouseEnterEvent))
            {
                this.m_EnterImgui = self;
            }
            if (e.GetType() == typeof(MouseLeaveEvent))
            {
                this.m_EnterImgui = null;
            }
            foreach (var frame in this.m_FrameDic.Values)
            {
                frame.View.style.backgroundColor = AcFrameStyle.FrameBoxNormal;
            }
            if (m_EnterImgui != null)
            {
                m_EnterImgui.View.style.backgroundColor = AcFrameStyle.FrameBoxHover;
            }
            if (this.m_CurSelectFrame != null)
            {
                this.m_CurSelectFrame.View.style.backgroundColor = AcFrameStyle.FrameBoxSelect;
            }
        }

        private void OnClickOfFrameBox(MouseDownEvent e, FrameInfoInternal select)
        {
            if (this.m_IsPLay && !this.m_IsPause) return;

            if (e.button == 0)
            {
                if (this.m_IsPLay) this.OnPlayBtn();
                if (this.m_CurSelectFrame == select)
                {
                    select.View.style.backgroundColor = AcFrameStyle.FrameBoxNormal;
                    this.m_CurSelectFrame = null;
                    this.m_FrameInfoView.ResetSelectFrameBox(null, false);
                    return;
                }
                this.m_CurSelectFrame = select;
                foreach (var box in this.m_FrameDic.Values)
                {
                    box.View.style.backgroundColor = AcFrameStyle.FrameBoxNormal;
                }
                select.View.style.backgroundColor = AcFrameStyle.FrameBoxSelect;
                this.m_FrameInfoView.ResetSelectFrameBox(select.FrameData, true);
                this.m_Host.PrefabData.ESkeletonAnim.ResetSelectFramePosture(this.m_StateView.CurSelectedState.StateName,
                    select.FrameIndex * 1f / ActionFrameWindow.FrameRate);
            }
        }

        private void OnPlayBtn()
        {
            if (this.m_Host.PrefabData == null)
            {
                this.m_Host.TipDialog.ShowTip("未添加 Json 动画状态控制文件", 1f);
                return;
            }

            if (this.m_StateView.CurSelectedState == null)
            {
                this.m_Host.TipDialog.ShowTip("未选择状态", 1f);
                return;
            }
            this.m_IsPLay = !this.m_IsPLay;
            this.m_RunFrameCount = -1;
            if (this.m_IsPLay)
            {
                this.m_RunFrameCount = 0;
                this.m_Host.PrefabData.ESkeletonAnim.InitAnimState(this.m_StateView.CurSelectedState,
                    this.m_Host.JsonData.SpineController.m_EntryState, this.m_AnimSpeedSlider.value);
            }
            else
            {
                this.m_IsPause = false;
                this.m_CurSelectFrame = null;
                this.RefreshPauseBtn();
                this.UpdateRunFrameStyle();
            }
            this.m_ToolBarPlayBtn.style.backgroundColor = this.m_IsPLay? AcFrameStyle.PlayOn : AcFrameStyle.PlayOff;
            this.m_ToolBarPlayBtnIcon.image = this.m_IsPLay
                ? AcFrameStyle.LoadIconWithName<Texture2D>(AcFrameStyle.Icon_Play, true)
                : AcFrameStyle.LoadIconWithName<Texture2D>(AcFrameStyle.Icon_Play, false);
        }

        private void OnPauseBtn()
        {
            if (!m_IsPLay)
            {
                return;
            }
            this.m_IsPause = !this.m_IsPause;
            this.RefreshPauseBtn();
        }

        private void RefreshPauseBtn()
        {
            this.m_ToolBarPauseBtn.style.backgroundColor = this.m_IsPause ? AcFrameStyle.PlayOn : AcFrameStyle.PlayOff;
            this.m_ToolBarPauseBtnIcon.image = this.m_IsPause
                ? AcFrameStyle.LoadIconWithName<Texture2D>(AcFrameStyle.Icon_Pause, true)
                : AcFrameStyle.LoadIconWithName<Texture2D>(AcFrameStyle.Icon_Pause, false);
        }

        private void OnNextFrameBtn()
        {
            if (this.m_StateView.CurSelectedState == null)
            {
                this.m_Host.TipDialog.ShowTip("未选择状态", 1f);
                return;
            }
            if (this.m_IsPLay && this.m_IsPause)
            {
                this.m_Host.PrefabData.ESkeletonAnim.UpdateFrame(1f / ActionFrameWindow.FrameRate);
                this.m_RunFrameCount = this.m_Host.PrefabData.ESkeletonAnim.RunFrameCount;
                this.UpdateRunFrameStyle();
            }
        }
        private void OnSliderValueChange(ChangeEvent<float> e)
        {
            this.m_SpeedValue.text = $"{e.newValue:F}F";
            this.m_Host.PrefabData?.ESkeletonAnim.SetCurTrackTimeScale(e.newValue);
        }

        private void RefreshBehaviourListView()
        {
            foreach (var behaviour in this.m_BehaviourList)
            {
                this.m_BehaviourListContainer.Remove(behaviour.HeadView);
            }
            this.m_SelectedBehaviour = null;
            this.m_BehaviourList.Clear();
            foreach (var frameEvent in this.m_StateView.CurSelectedState.EventList)
            {
                this.AddBehaviourEvent(frameEvent);
            }
        }

        public void AddBehaviourAndRefreshView(object data)
        {
            this.AddBehaviourEvent(data);
        }

        private void RemoveBehaviourAndRefreshView(ContextClickEvent e, BehaviourEventInfo info)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("拷贝"),false, () =>
            {
                this.m_CopyBuffer = info.CopyBehaviourData();
            });
            menu.AddItem(new GUIContent("粘贴"),false, () =>
            {
                BehaviourData anim = (BehaviourData) this.m_CopyBuffer;
                BehaviourData cur = (BehaviourData) info.FrameData;
                cur.BehaviourName = anim.BehaviourName;
                cur.BehaviourFrameStartTime = anim.BehaviourFrameStartTime;
                cur.BehaviourFrameEndTime = anim.BehaviourFrameEndTime;
                this.RefreshBehaviourListView();
            });
            menu.AddItem(new GUIContent("移除当前行为"), false, () =>
            {
                if (info.FrameData.GetType() == typeof(ActionFrame.Runtime.SpineEventConfig))
                {
                    this.m_Host.TipDialog.ShowTip("Spine 内置行为 请不要移除.", 2f);
                    return;
                }
                this.m_StateView.CurSelectedState.EventList.Remove(info.FrameData);
                this.m_BehaviourListContainer.Remove(info.HeadView);
                this.m_BehaviourList.Remove(info);
            });
            menu.ShowAsContext();
        }
        
        /// <summary>
        /// 新增状态行为
        /// </summary>
        /// <param name="eventData"></param>
        private void AddBehaviourEvent(object eventData)
        {
            BehaviourData data = (BehaviourData) eventData;
            BehaviourEventInfo info = new BehaviourEventInfo();
            info.FrameData = eventData;

            info.HeadView = new VisualElement();
            info.HeadView.RegisterCallback<TooltipEvent>(e=> this.OnOverBehaviourToolTip(e, info));
            info.HeadView.name = "BehaviourItemHeadView";
            info.HeadView.style.height = BehaviourItemHeight;
            info.HeadView.style.width = this.m_ScrollContainerWidth;
            info.HeadView.style.backgroundColor = AcFrameStyle.FrameBehaviourEventNormal;
            info.HeadViewTitle = new Label(data.BehaviourName);
            info.HeadViewTitle.name = "BehaviourItemHeadTitle";
            info.HeadViewTitle.RegisterCallback<MouseDownEvent>(e => { this.OnClickBehaviourItem(e, info);});
            info.HeadViewTitle.RegisterCallback<ContextClickEvent>(e => { this.RemoveBehaviourAndRefreshView(e, info);});
            info.HeadViewTitle.style.backgroundColor = AcFrameStyle.FrameBehaviourEventNormal;
            info.HeadViewTitle.style.height = BehaviourItemHeight;
            info.HeadView.Add(info.HeadViewTitle);
            
            int startFrame = Mathf.RoundToInt((float) data.BehaviourFrameStartTime * ActionFrameWindow.FrameRate);
            int endFrame = Mathf.RoundToInt((float) data.BehaviourFrameEndTime * ActionFrameWindow.FrameRate);
            int offsetLeft = BehaviourListWidth + startFrame * FrameBoxWidth;
            int bodyWidth = (endFrame - startFrame + 1) * FrameBoxWidth;
            
            info.BodyViewContainer = new VisualElement();
            info.BodyViewContainer.name = "BehaviourItem_BodyView_Container";
            info.BodyViewContainer.style.width = bodyWidth;
            info.BodyViewContainer.style.height = BehaviourItemHeight;
            info.BodyViewContainer.style.left = offsetLeft;
            
            info.BodyLeftHandle = new IMGUIContainer(() => this.OnDragBehaviourBodyLeftHandle(info));
            info.BodyLeftHandle.name = "BehaviourItemBody_Handle";
            info.BodyLeftHandle.style.height = BehaviourItemHeight + 4;
            info.BodyViewContainer.Add(info.BodyLeftHandle);

            info.BodyViewCenter = new IMGUIContainer(() => { this.OnDragBehaviourBodyView(info); });
            info.BodyViewCenter.RegisterCallback<MouseEnterEvent>(e => this.OnMouseEnterBehaviourBodyView(e, info));
            info.BodyViewCenter.RegisterCallback<MouseLeaveEvent>(e => this.OnMouseLeaveBehaviourBodyView(e, info));
            info.BodyViewCenter.RegisterCallback<ContextClickEvent>(e => this.OnContextClickBehaviourBodyView(e, info));
            info.BodyViewCenter.name = "BehaviourItemBodyView_Center";
            info.BodyViewCenter.style.height = BehaviourItemHeight;
            info.BodyViewCenter.style.backgroundColor = AcFrameStyle.FrameBehaviourBodyNormal;
            info.BodyViewContainer.Add(info.BodyViewCenter);

            info.BodyRightHandle = new IMGUIContainer(() => this.OnDragBehaviourBodyRightHandle(info));
            info.BodyRightHandle.name = "BehaviourItemBody_Handle";
            info.BodyRightHandle.style.height = BehaviourItemHeight + 4;
            info.BodyViewContainer.Add(info.BodyRightHandle);
            
            info.HeadView.Add(info.BodyViewContainer);

            this.m_BehaviourListContainer.Add(info.HeadView);
            this.m_BehaviourList.Add(info);
        }
        
        private void OnOverBehaviourToolTip(TooltipEvent e, BehaviourEventInfo behaviour)
        {
            e.tooltip = ((BehaviourData)behaviour.FrameData).BehaviourName;
            Rect toolTipRect = new Rect();
            toolTipRect.position = new Vector2(int.MaxValue, behaviour.HeadViewTitle.worldBound.position.y);
            toolTipRect.height = behaviour.HeadViewTitle.layout.height;
            e.rect = toolTipRect;
        }

        private void OnContextClickBehaviourBodyView(ContextClickEvent e, BehaviourEventInfo behaviour)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("应用所有帧"),false, () =>
            {
                float maxTime = this.m_StateView.CurSelectedState.Duration;
                float minTime = 1f / (float)ActionFrameWindow.FrameRate;

                ((BehaviourData) (behaviour.FrameData)).BehaviourFrameStartTime = minTime;
                ((BehaviourData) (behaviour.FrameData)).BehaviourFrameEndTime = maxTime;
                int curEndFrame = Mathf.RoundToInt(maxTime * ActionFrameWindow.FrameRate);
                behaviour.BodyViewContainer.style.width = (curEndFrame + 1) * FrameBoxWidth;
                behaviour.BodyViewContainer.style.left = BehaviourListWidth;
            });
            menu.ShowAsContext();
        }
        
        private void OnMouseEnterBehaviourBodyView(MouseEnterEvent e, BehaviourEventInfo behaviour)
        {
            if (this.m_IsInDragHandle)
            {
                return;
            }
            behaviour.BodyViewCenter.style.backgroundColor = AcFrameStyle.FrameBehaviourBodyHover;
        }
        
        private void OnMouseLeaveBehaviourBodyView(MouseLeaveEvent e, BehaviourEventInfo behaviour)
        {
            behaviour.BodyViewCenter.style.backgroundColor = AcFrameStyle.FrameBehaviourBodyNormal;
        }

        private void OnDragBehaviourBodyInternal(BehaviourEventInfo behaviour, int controlId, DragType dragType)
        {
            Event current = Event.current;
            if (current.type == EventType.Layout || current.type == EventType.Repaint)
            {
                return;
            }
            switch (current.GetTypeForControl(controlId))
            {
                case EventType.MouseDown:
                    if (current.button != 0 || current.clickCount == 2)
                        break;
                    GUIUtility.hotControl = controlId;
                    this.m_DragBehaviourBodyStartPos = Event.current.mousePosition.x;
                    break;
                case EventType.MouseUp:
                    this.m_IsInDragHandle = false;
                    if (GUIUtility.hotControl != controlId)
                        break;
                    GUIUtility.hotControl = 0;
                    this.m_DragBehaviourBodyStartPos = 0f;
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl != controlId)
                        break;
                    float moveX = Event.current.mousePosition.x - this.m_DragBehaviourBodyStartPos;
                    int moveFrame = Mathf.RoundToInt(moveX / FrameBoxWidth);
                    int curStartFrame = Mathf.RoundToInt(((BehaviourData)behaviour.FrameData).BehaviourFrameStartTime * (float)ActionFrameWindow.FrameRate);
                    int curEndFrame =  Mathf.RoundToInt(((BehaviourData)behaviour.FrameData).BehaviourFrameEndTime * (float)ActionFrameWindow.FrameRate);
                    if (moveFrame == 0)
                        break;
                    switch (dragType)
                    {
                        case DragType.DragCenter:
                            if (curStartFrame + moveFrame < 0)
                                break;
                            if (curEndFrame + moveFrame > this.m_StateView.CurSelectedState.FrameCount)
                                break;
                            ((BehaviourData)behaviour.FrameData).BehaviourFrameStartTime += moveFrame * (1f / (float)ActionFrameWindow.FrameRate);
                            ((BehaviourData)behaviour.FrameData).BehaviourFrameEndTime += moveFrame * (1f / (float)ActionFrameWindow.FrameRate);
                            break;
                        case DragType.LeftHandle:
                            this.m_IsInDragHandle = true;
                            if (curStartFrame + moveFrame < 0)
                                break;
                            if (moveFrame > 0)
                            {
                                if (curStartFrame + moveFrame > curEndFrame)
                                    break;
                            }
                            ((BehaviourData)behaviour.FrameData).BehaviourFrameStartTime += moveFrame * (1f / (float)ActionFrameWindow.FrameRate);
                            int startFrameLeft = Mathf.RoundToInt(((BehaviourData)behaviour.FrameData).BehaviourFrameStartTime * (float)ActionFrameWindow.FrameRate);
                            behaviour.BodyViewContainer.style.width = ((curEndFrame - startFrameLeft + 1) * FrameBoxWidth);
                            break;
                        case DragType.RightHandle:
                            this.m_IsInDragHandle = true;
                            if (curEndFrame + moveFrame > this.m_StateView.CurSelectedState.FrameCount)
                                break;
                            if (moveFrame < 0)
                            {
                                if (curEndFrame + moveFrame < curStartFrame)
                                    break;
                            }
                            ((BehaviourData)behaviour.FrameData).BehaviourFrameEndTime += moveFrame * (1f / (float)ActionFrameWindow.FrameRate);
                            int endFrameRight = Mathf.RoundToInt(((BehaviourData)behaviour.FrameData).BehaviourFrameEndTime * (float)ActionFrameWindow.FrameRate);
                            behaviour.BodyViewContainer.style.width = (int) ((endFrameRight - curStartFrame + 1) * FrameBoxWidth);
                            break;
                    }
                    int startFrame = Mathf.RoundToInt(((BehaviourData)behaviour.FrameData).BehaviourFrameStartTime * (float)ActionFrameWindow.FrameRate);
                    behaviour.BodyViewContainer.style.left = BehaviourListWidth + startFrame * FrameBoxWidth;
                    current.Use();
                    break;
            }
        }

        private void OnDragBehaviourBodyLeftHandle(BehaviourEventInfo behaviour)
        {
            int controlId = GUIUtility.GetControlID(kDragLeftHndleControlID, FocusType.Passive);
            EditorGUIUtility.AddCursorRect(behaviour.BodyLeftHandle.layout, MouseCursor.ResizeHorizontal, controlId);
            this.OnDragBehaviourBodyInternal(behaviour, controlId, DragType.LeftHandle);
        }

        private void OnDragBehaviourBodyRightHandle(BehaviourEventInfo behaviour)
        {
            int controlId = GUIUtility.GetControlID(kDragRightHandleControlID, FocusType.Passive);
            var rightHandleRect = behaviour.BodyRightHandle.layout;
            Rect rect = new Rect(0, rightHandleRect.y, rightHandleRect.width, rightHandleRect.height);
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal, controlId);
            this.OnDragBehaviourBodyInternal(behaviour, controlId, DragType.RightHandle);
        }
        
        private void OnDragBehaviourBodyView(BehaviourEventInfo behaviour)
        {
            int controlId = GUIUtility.GetControlID(kDragSelectionControlID, FocusType.Passive);
            this.OnDragBehaviourBodyInternal(behaviour, controlId, DragType.DragCenter);
        }

        private void OnClickBehaviourItem(MouseDownEvent e, BehaviourEventInfo behaviour)
        {
            if (e.button == 0 || e.button == 1)
            {
                if (this.m_SelectedBehaviour != null)
                {
                    this.m_SelectedBehaviour.HeadView.style.backgroundColor = AcFrameStyle.FrameBehaviourEventNormal;
                    this.m_SelectedBehaviour.HeadViewTitle.style.backgroundColor = AcFrameStyle.FrameBehaviourEventNormal;
                }

                if (this.m_SelectedBehaviour == behaviour)
                {
                    this.m_SelectedBehaviour = null;
                    this.m_FrameInfoView.ResetSelectedBehaviour(null);
                    return;
                }
                this.m_SelectedBehaviour = behaviour;
                this.m_SelectedBehaviour.HeadView.style.backgroundColor = AcFrameStyle.FrameBehaviourEventSelect;
                this.m_SelectedBehaviour.HeadViewTitle.style.backgroundColor = AcFrameStyle.FrameBehaviourLabelSelect;
                this.m_FrameInfoView.ResetSelectedBehaviour(behaviour.FrameData);
            }
        }

        private enum DragType
        {
            LeftHandle,
            RightHandle,
            DragCenter,
        }
        
        internal class FrameInfoInternal
        {
            public VisualElement View;
            public VisualElement LabelContainer;
            public Label ViewLabel;
            public Box BeHitBox;
            public Box AttackBox;
            public int FrameIndex;
            public FrameData FrameData;
        }

        private class BehaviourEventInfo
        {
            public Label HeadViewTitle;
            public VisualElement HeadView;
            
            public VisualElement BodyViewContainer;
            public IMGUIContainer BodyViewCenter;
            public VisualElement BodyLeftHandle;
            public VisualElement BodyRightHandle;
            
            public object FrameData;

            public BehaviourData CopyBehaviourData()
            {
                System.Type type = this.FrameData.GetType();
                BehaviourData cur = (BehaviourData) this.FrameData;
                BehaviourData obj = (BehaviourData) System.Activator.CreateInstance(type);
                obj.BehaviourName = cur.BehaviourName;
                obj.BehaviourFrameStartTime = cur.BehaviourFrameStartTime;
                obj.BehaviourFrameEndTime = cur.BehaviourFrameEndTime;
                return obj;
            }
        }
    }
}