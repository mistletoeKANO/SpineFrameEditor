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
    /// 状态视图
    /// </summary>
    public class AnimationStateView : BaseView
    {
        private IMGUIContainer m_EntryStateView;
        private PopupField<string> m_EntryStatePopUp;
        
        private ScrollView m_ScrollView;

        private List<InternalStateInfo> m_StateList;
        private InternalStateInfo m_SelectedStateInfo;

        public StateData CurSelectedState
        {
            get => this.m_SelectedStateInfo?.Data;
        }
        public override void Init(EditorWindow host, VisualElement parent)
        {
            this.m_Host = (ActionFrameWindow) host;
            this.m_ParentView = parent;
            this.m_StateList = new List<InternalStateInfo>();
            
            this.m_CurView = new IMGUIContainer();
            this.m_CurView.name = "AnimStateView";
            this.m_CurView.AddToClassList("LeftPanelClass");
            this.DoAnimStateView();
            this.m_ParentView.Add(this.m_CurView);
        }
        
        private void DoAnimStateView()
        {
            var styleSheet = Resources.Load<StyleSheet>("Uss/AnimStateView");
            this.m_CurView.styleSheets.Add(styleSheet);
            Label label = new Label("状态列表");
            label.AddToClassList("ViewTitle");
            this.m_CurView.Add(label);
            
            this.m_EntryStateView = new IMGUIContainer();
            this.m_EntryStateView.name = "EntryStateView";
            
            Label entryTitle = new Label("初始状态");
            entryTitle.style.alignSelf = new StyleEnum<Align>(Align.Center);
            entryTitle.style.justifyContent = new StyleEnum<Justify>(Justify.Center);
            this.m_EntryStateView.Add(entryTitle);

            this.m_CurView.Add(this.m_EntryStateView);

            this.m_ScrollView = new ScrollView();
            this.m_ScrollView.name = "AnimStateViewList";
            this.m_ScrollView.showHorizontal = false;

            this.m_CurView.Add(this.m_ScrollView);
        }

        private void ResetEntryState()
        {
            if (this.m_EntryStatePopUp != null)
            {
                this.m_EntryStateView.Remove(this.m_EntryStatePopUp);
                this.m_EntryStatePopUp = null;
            }
            List<string> strList = this.m_Host.JsonData.SpineController.m_StateDataList.Select(item => item.StateName).ToList();
            string selected = "";
            if (this.m_Host.JsonData.SpineController.m_EntryState != null)
            {
                selected = this.m_Host.JsonData.SpineController.m_EntryState.StateName;
            }
            else if (strList.Count > 0)
                selected = strList[0];
            else
                return;
            this.m_EntryStatePopUp = new PopupField<string>(strList, selected);
            this.m_EntryStatePopUp.formatSelectedValueCallback += this.OnValueChangeOfEntryState;
            this.m_EntryStatePopUp.name = "SelectedEntryState";
            this.m_EntryStateView.Add(this.m_EntryStatePopUp);
        }

        /// <summary>
        /// 重置状态列表
        /// </summary>
        public void ResetStateList()
        {
            if (this.m_Host.JsonData == null)
            {
                return;
            }

            if (this.m_Host.JsonData.SpineController.m_StateDataList.Count == 0)
            {
                return;
            }
            this.m_ScrollView.contentContainer.RemoveAllChild();
            this.m_StateList.Clear();
            this.ResetEntryState();
            foreach (var state in this.m_Host.JsonData.SpineController.m_StateDataList)
            {
                InternalStateInfo stateInfo = new InternalStateInfo();
                stateInfo.View = new IMGUIContainer();
                
                Label name = new Label($"{state.StateName}");
                name.AddToClassList("StateItemLabel");
                stateInfo.View.Add(name);
                
                stateInfo.View.RegisterCallback<MouseDownEvent>(e =>
                {
                    this.OnClickStateItem(e, stateInfo);
                });
                stateInfo.View.RegisterCallback<ContextClickEvent>(this.OnContextClick);
                stateInfo.View.name = "AnimStateItem";
                this.m_ScrollView.contentContainer.Add(stateInfo.View);
                stateInfo.Data = state;
                this.m_StateList.Add(stateInfo);
                if (this.m_SelectedStateInfo == null)
                {
                    this.m_SelectedStateInfo = stateInfo;
                    this.m_SelectedStateInfo.View.style.backgroundColor = AcFrameStyle.FrameBoxSelect;
                    this.RefreshFrameView();
                }
            }
        }

        private void OnClickStateItem(IMouseEvent e, InternalStateInfo select)
        {
            if (e.button != 0 && e.button != 1)
            {
                return;
            }
            this.m_SelectedStateInfo = select;
            foreach (var box in this.m_ScrollView.contentContainer.Children())
            {
                box.style.backgroundColor = AcFrameStyle.ClipItemNormal;
            }
            select.View.style.backgroundColor = AcFrameStyle.FrameBoxSelect;
            this.RefreshFrameView();
        }
        
        private void OnContextClick(ContextClickEvent e)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("复制状态名称"), false, this.CopySelectStateName);
            menu.AddItem(new GUIContent("移除当前状态"), false, this.RemoveCurrentState);
            menu.ShowAsContext();
        }

        private string OnValueChangeOfEntryState(string stateName)
        {
            this.m_Host.ResetEntryState(stateName);
            return stateName;
        }

        private void RefreshFrameView()
        {
            this.m_Host.RefreshFrameViewData(this.m_SelectedStateInfo?.Data);
        }

        private void CopySelectStateName()
        {
            UnityEngine.GUIUtility.systemCopyBuffer = this.m_SelectedStateInfo.Data.StateName;
        }

        private void RemoveCurrentState()
        {
            this.m_Host.RemoveState(this.m_SelectedStateInfo.Data);
            this.m_StateList.Remove(this.m_SelectedStateInfo);
            this.m_ScrollView.contentContainer.Remove(this.m_SelectedStateInfo.View);
            this.m_SelectedStateInfo = this.m_StateList.Count > 0? this.m_StateList.ElementAt(0) : null;
            if (this.m_SelectedStateInfo != null)
            {
                this.m_SelectedStateInfo.View.style.backgroundColor = AcFrameStyle.FrameBoxSelect;
            }
            this.RefreshFrameView();
            this.ResetEntryState();
        }
        
        private class InternalStateInfo
        {
            public IMGUIContainer View;
            public StateData Data;
        }
    }
}