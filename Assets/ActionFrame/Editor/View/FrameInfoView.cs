using System;
using System.Collections.Generic;
using System.Linq;
using ActionFrame.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ActionFrame.Editor
{
    /// <summary>
    /// 动画帧 信息 视图
    /// </summary>
    public class FrameInfoView : BaseView
    {
        private AnimationStateView m_StateView;
        private FrameSequenceView m_FrameSeqView;

        #region 当前状态基本信息
        
        private VisualElement m_StateInfo;
        
        private Toggle m_StateInfoIsLoop;
        private Toggle m_UseCurBeHitBox;
        private FloatField m_StateMixDuration;
        private PopupField<string> m_NextStatePopUp;

        #endregion

        #region 当前选择行为基本信息
        
        private VisualElement m_CurSelectedBehaviourView;

        private TextField m_BehaviourName;

        #endregion

        #region 受击范围

        private VisualElement m_BeHitRangeView;
        private VisualElement m_BeHitListContainer;

        #endregion

        #region 攻击范围

        private VisualElement m_AttackRangeView;
        private VisualElement m_AttackListContainer;

        #endregion
        
        private BoxItem m_CopyBuffer;
        
        public override void Init(EditorWindow host, VisualElement parent)
        {
            this.m_Host = (ActionFrameWindow) host;
            this.m_ParentView = parent;
            this.m_CurView = new ScrollView();
            this.m_CurView.contentContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            this.m_CurView.contentContainer.style.flexGrow = 1;
            this.m_CurView.name = "FrameInfoView";
            this.DoFrameInfoView();
            this.m_ParentView.Add(this.m_CurView);
        }

        public void InitViewData(AnimationStateView stateView, FrameSequenceView fSeqView)
        {
            this.m_StateView = stateView;
            this.m_FrameSeqView = fSeqView;
        }

        private void DoFrameInfoView()
        {
            var styleSheet = Resources.Load<StyleSheet>("Uss/FrameInfoView");
            this.m_CurView.styleSheets.Add(styleSheet);
            
            this.m_StateInfo = new VisualElement();
            this.m_StateInfo.name = "CommonBehaviourList";
            this.m_StateInfo.AddToClassList("CommonListStyle");
            this.m_CurView.Add(this.m_StateInfo);

            this.m_CurSelectedBehaviourView = new ScrollView();
            this.m_CurSelectedBehaviourView.name = "CurSelectedBehaviour";
            this.m_CurSelectedBehaviourView.AddToClassList("CommonListStyle");
            this.m_CurView.Add(this.m_CurSelectedBehaviourView);
            
            this.m_BeHitRangeView = new ScrollView();
            this.m_BeHitRangeView.name = "BeHitRangeView";
            this.m_BeHitRangeView.AddToClassList("CommonListStyle");
            this.m_CurView.Add(this.m_BeHitRangeView);
            
            this.m_AttackRangeView = new ScrollView();
            this.m_AttackRangeView.name = "AttackRangeView";
            this.m_AttackRangeView.AddToClassList("CommonListStyle");
            this.m_CurView.Add(this.m_AttackRangeView);
        }

        public void ResetSelectedStateInfo(StateData data)
        {
            this.m_StateInfo.RemoveAllChild();
            Label stateInfoTitle = new Label("当前状态基本信息");
            stateInfoTitle.AddToClassList("ViewTitle");
            this.m_StateInfo.Add(stateInfoTitle);
            if (data == null)
            {
                return;
            }
            
            VisualElement baseInfoContainer = new VisualElement();
            baseInfoContainer.name = "StateInfoBaseContainer";
            
            Label frameCount = new Label($"当前状态总帧数: {data.FrameCount}");
            baseInfoContainer.Add(frameCount);
            Label frameDuration = new Label($"当前状态时长: {data.Duration:F}S");
            baseInfoContainer.Add(frameDuration);

            this.m_StateInfo.Add(baseInfoContainer);
            
            this.m_StateInfoIsLoop = new Toggle("是否循环播放");
            this.m_StateInfoIsLoop.value = data.IsLoop;
            this.m_StateInfoIsLoop.RegisterCallback<UnityEngine.UIElements.ChangeEvent<bool>>(e =>
            {
                data.IsLoop = e.newValue;
                if (this.m_NextStatePopUp != null && data.IsLoop)
                {
                    this.m_StateInfo.Remove(this.m_NextStatePopUp);
                    this.m_StateInfo.Remove(this.m_StateMixDuration);
                }
                this.ResetNextStateInfo(data);
            });
            this.m_StateInfo.Add(this.m_StateInfoIsLoop);
            
            this.m_UseCurBeHitBox = new Toggle("应用当前状态受击框到所有状态");
            this.m_UseCurBeHitBox.value = data.UseCurSBeHitBox;
            this.m_UseCurBeHitBox.RegisterValueChangedCallback(e =>
            {
                data.UseCurSBeHitBox = e.newValue;
            });
            this.m_StateInfo.Add(this.m_UseCurBeHitBox);
            
            this.ResetNextStateInfo(data);
            Button addNewState = new Button(this.OnAddBehaviour);
            addNewState.text = "新增行为";
            addNewState.name = "StateInfoBtn";
            this.m_StateInfo.Add(addNewState);
        }

        private void ResetNextStateInfo(StateData stateData)
        {
            if (stateData.IsLoop)
                return;
            List<string> strList = this.m_Host.JsonData.SpineController.m_StateDataList.Select(item => item.StateName).ToList();
            string select = String.Empty;
            if (string.IsNullOrEmpty(stateData.NextStateName))
            {
                select = strList[0];
                stateData.NextStateName = select;
            }
            else
            {
                select = stateData.NextStateName;
            }
            this.m_NextStatePopUp = new PopupField<string>("下一个状态", strList, select);
            this.m_NextStatePopUp.RegisterCallback<ChangeEvent<string>>(e =>
            {
                stateData.NextStateName = e.newValue;
            });
            this.m_StateInfo.Add(this.m_NextStatePopUp);
            
            this.m_StateMixDuration = new FloatField("动画过渡时长");
            this.m_StateMixDuration.value = stateData.TransitionTime;
            this.m_StateMixDuration.RegisterValueChangedCallback(e =>
            {
                stateData.TransitionTime = e.newValue;
            });
            this.m_StateInfo.Add(this.m_StateMixDuration);
        }

        public void ResetSelectedBehaviour(object data)
        {
            this.m_CurSelectedBehaviourView.RemoveAllChild();
            
            Label selectedBehaviourTitle = new Label("行为信息");
            selectedBehaviourTitle.AddToClassList("ViewTitle");
            this.m_CurSelectedBehaviourView.Add(selectedBehaviourTitle);
            
            if (data == null)
            {
                return;
            }
            this.m_CurSelectedBehaviourView.DrawObject("", "", data, data.GetType(), o =>
            {
                this.m_FrameSeqView.RefreshCurSelectedBehaviour();
            });
        }

        public void ResetSelectFrameBox(FrameData frameData, bool isSelected)
        {
            this.m_BeHitRangeView.RemoveAllChild();
            if (this.m_BeHitRangeView.hierarchy.childCount > 3)
            {
                this.m_BeHitRangeView.hierarchy.RemoveAt(3);
            }
            this.m_AttackRangeView.RemoveAllChild();
            if (this.m_AttackRangeView.hierarchy.childCount > 3)
            {
                this.m_AttackRangeView.hierarchy.RemoveAt(3);
            }

            if (!isSelected)
                return;
            this.ResetSelectFrameBeHitBox(frameData);
            this.ResetSelectFrameAttackBox(frameData);
        }

        private void ResetSelectFrameBeHitBox(FrameData frameData)
        {
            Label selectedBehaviourTitle = new Label("受击框");
            selectedBehaviourTitle.AddToClassList("ViewTitle");
            this.m_BeHitRangeView.Add(selectedBehaviourTitle);
            
            this.m_BeHitListContainer = new VisualElement();
            this.m_BeHitListContainer.style.flexGrow = 1;
            this.m_BeHitRangeView.Add(this.m_BeHitListContainer);
            
            Button addBoxBtn = new Button(this.OnAddBeHitBox);
            addBoxBtn.text = "新增受击框";
            addBoxBtn.name = "AddHitOrAttackBoxBtn";
            this.m_BeHitRangeView.hierarchy.Add(addBoxBtn);
            if (frameData == null || frameData.BeHitRangeList.Count <= 0)
            {
                return;
            }
            Toggle applyToAll = new Toggle("应用到所有帧");
            applyToAll.value = frameData.ApplyBeHitBoxToAllFrame;
            applyToAll.RegisterValueChangedCallback(e =>
            {
                frameData.ApplyBeHitBoxToAllFrame = e.newValue;
            });
            this.m_BeHitRangeView.Add(applyToAll);
            foreach (var item in frameData.BeHitRangeList)
            {
                this.DrawAttackAndHitBox(this.m_BeHitListContainer, item, true);
            }
        }

        private void ResetSelectFrameAttackBox(FrameData frameData)
        {
            Label selectedBehaviourTitle = new Label("攻击框");
            selectedBehaviourTitle.AddToClassList("ViewTitle");
            this.m_AttackRangeView.Add(selectedBehaviourTitle);
            
            this.m_AttackListContainer = new VisualElement();
            this.m_AttackListContainer.style.flexGrow = 1;
            this.m_AttackRangeView.Add(this.m_AttackListContainer);
            
            Button addBoxBtn = new Button(this.OnAddAttackBox);
            addBoxBtn.text = "新增攻击框";
            addBoxBtn.name = "AddHitOrAttackBoxBtn";
            this.m_AttackRangeView.hierarchy.Add(addBoxBtn);
            
            if (frameData == null || frameData.AttackedRangeList.Count <= 0)
            {
                return;
            }
            foreach (var item in frameData.AttackedRangeList)
            {
                this.DrawAttackAndHitBox(this.m_AttackListContainer, item);
            }
        }

        private void DrawAttackAndHitBox(VisualElement root, BoxItem item, bool isHitBox = false)
        {
            VisualElement itemContainer = new VisualElement();
            itemContainer.name = "BoxItemContainer";
            
            Vector2Field offsetField = new Vector2Field("偏移量");
            offsetField.value = item.Offset;
            offsetField.RegisterValueChangedCallback(e =>
            {
                item.Offset = e.newValue;
            });
            itemContainer.Add(offsetField);
            
            Vector2Field sizeField = new Vector2Field("大小");
            sizeField.value = item.Size;
            sizeField.RegisterValueChangedCallback(e =>
            {
                item.Size = e.newValue;
            });
            itemContainer.Add(sizeField);
            
            FloatField rotateField = new FloatField("旋转角");
            rotateField.value = item.Rotation;
            rotateField.RegisterValueChangedCallback(e =>
            {
                item.Rotation = e.newValue;
            });
            itemContainer.Add(rotateField);
            itemContainer.RegisterCallback<ContextClickEvent>(e =>
            {
                this.HitOrAttackBoxContextClick(itemContainer, offsetField, sizeField, rotateField, item, isHitBox);
            });

            root.Add(itemContainer);
        }

        private void OnAddBeHitBox()
        {
            this.OnAddBoxInternal(true);
        }

        private void OnAddAttackBox()
        {
            this.OnAddBoxInternal(false);
        }

        private void OnAddBoxInternal(bool isHitBox)
        {
            BoxItem item = new BoxItem();
            if (this.m_FrameSeqView.CurSelectFrame.FrameData == null)
            {
                this.m_FrameSeqView.CurSelectFrame.FrameData = new FrameData();
            }
            if (!this.m_StateView.CurSelectedState.FrameDic.ContainsKey(this.m_FrameSeqView.CurSelectFrame.FrameIndex))
            {
                this.m_StateView.CurSelectedState.FrameDic.Add(this.m_FrameSeqView.CurSelectFrame.FrameIndex, this.m_FrameSeqView.CurSelectFrame.FrameData);
            }
            if (isHitBox)
            {
                this.DrawAttackAndHitBox(this.m_BeHitListContainer, item, true);
                this.m_FrameSeqView.CurSelectFrame.FrameData.BeHitRangeList.Add(item);
            }
            else
            {
                this.DrawAttackAndHitBox(this.m_AttackListContainer, item, false);
                this.m_FrameSeqView.CurSelectFrame.FrameData.AttackedRangeList.Add(item);
            }
            this.ResetSelectFrameBox(this.m_FrameSeqView.CurSelectFrame.FrameData, true);
            this.m_FrameSeqView.ResetSelectFrameBoxHitBox();
        }

        private void HitOrAttackBoxContextClick(VisualElement e, Vector2Field offset, Vector2Field size, FloatField rotation, BoxItem item, bool isHitBox)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("拷贝"),false, () =>
            {
                this.m_CopyBuffer = item;
            } );
            menu.AddItem(new GUIContent("粘贴"),false, () =>
            {
                item.Offset = this.m_CopyBuffer.Offset;
                item.Size = this.m_CopyBuffer.Size;
                item.Rotation = this.m_CopyBuffer.Rotation;
                offset.value = item.Offset;
                size.value = item.Size;
                rotation.value = item.Rotation;
            } );
            if (isHitBox)
            {
                menu.AddItem(new GUIContent("移除当前受击框"), false, () =>
                {
                    this.m_BeHitListContainer.Remove(e);
                    this.m_FrameSeqView.CurSelectFrame.FrameData.BeHitRangeList.Remove(item);
                    this.m_FrameSeqView.ResetSelectFrameBoxHitBox();
                    CheckItemNum();
                });
            }
            else
            {
                menu.AddItem(new GUIContent("移除当前攻击框"), false, () =>
                {
                    this.m_AttackListContainer.Remove(e);
                    this.m_FrameSeqView.CurSelectFrame.FrameData.AttackedRangeList.Remove(item);
                    this.m_FrameSeqView.ResetSelectFrameBoxHitBox();
                    CheckItemNum();
                });
            }
            menu.ShowAsContext();

            void CheckItemNum()
            {
                if (this.m_FrameSeqView.CurSelectFrame.FrameData.BeHitRangeList.Count > 0 ||
                    this.m_FrameSeqView.CurSelectFrame.FrameData.AttackedRangeList.Count > 0)
                {
                    return;
                }
                this.m_StateView.CurSelectedState.FrameDic.Remove(this.m_FrameSeqView.CurSelectFrame.FrameIndex);
                this.m_FrameSeqView.CurSelectFrame.FrameData = null;
            }
        }

        private void OnAddBehaviour()
        {
            AcSelectBehaviourWindow.ShowTypeWithAttr(t =>
            {
                object obj = System.Activator.CreateInstance(t);
                this.m_StateView.CurSelectedState.EventList.Add(obj);
                this.m_FrameSeqView.AddBehaviourAndRefreshView(obj);
            });
        }
    }
}