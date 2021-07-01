using System;
using System.Collections.Generic;
using ActionFrame.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace ActionFrame.Editor
{
    public class ActionFrameWindow : EditorWindow
    {
        //帧率, spine 动画帧率 默认为 30 帧
        public static readonly int FrameRate = 30;
        
        private AcFrameTip m_TipDialog;
        private Toolbar m_WindowToolBar;
        private VisualElement m_MainView;

        private AnimationClipView m_AnimClipView;
        private AnimationStateView m_AnimStateView;
        
        private VisualElement m_FrameView;
        
        private FrameSequenceView m_FrameSeqView;
        private FrameInfoView m_FrameInfoView;
        
        private ObjectField m_Prefab;

        private PrefabDataParse m_PrefabData;
        private AnimationJsonData m_JsonData;
        
        private float m_LastFrameTime = 0f;

        public AcFrameTip TipDialog
        {
            get => this.m_TipDialog;
        }

        public PrefabDataParse PrefabData
        {
            get => this.m_PrefabData;
        }

        public AnimationJsonData JsonData
        {
            get => this.m_JsonData;
        }

        [MenuItem("Tool/Spine动作编辑器")]
        private static void ShowWindow()
        {
            var curWindow = GetWindow<ActionFrameWindow>();
            curWindow.minSize = new Vector2(600, 400);
            curWindow.titleContent = new GUIContent(
                "Spine动作编辑器",
                Resources.Load<Texture2D>("Icon/actionFrameIcon"));
            curWindow.Focus();
        }

        private void OnEnable()
        {
            this.OnInit();
            this.OnGUIInit();
            autoRepaintOnSceneChange = true;
            SceneView.duringSceneGui += this.OnSceneViewGUI;
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= this.OnSceneViewGUI;
        }

        private void OnSceneViewGUI(SceneView view)
        {
            this.m_FrameSeqView?.OnSceneViewGUI();
            view.Repaint();
            this.Repaint();
        }

        /// <summary>
        /// 初始化 窗口数据
        /// </summary>
        private void OnInit()
        {
            Application.targetFrameRate = (int)ActionFrameWindow.FrameRate;
            this.m_TipDialog = new AcFrameTip();
            this.m_TipDialog.Init(this.rootVisualElement);
        }

        /// <summary>
        /// 初始化窗口布局
        /// </summary>
        private void OnGUIInit()
        {
            var styleSheet = Resources.Load<StyleSheet>("Uss/ActionFrameWindow");
            var styleSheetCommon = Resources.Load<StyleSheet>("Uss/CommonClass");
            this.rootVisualElement.styleSheets.Add(styleSheet);
            this.rootVisualElement.styleSheets.Add(styleSheetCommon);

            this.m_WindowToolBar = new Toolbar();
            this.m_WindowToolBar.name = "WindowToolBar";
            this.DoToolBarMenu(m_WindowToolBar);
            this.rootVisualElement.Add(m_WindowToolBar);
            
            this.m_MainView = new VisualElement();
            this.m_MainView.name = "MainView";

            this.m_AnimClipView = new AnimationClipView();
            this.m_AnimClipView.Init(this, this.m_MainView);

            this.m_AnimStateView = new AnimationStateView();
            this.m_AnimStateView.Init(this, this.m_MainView);

            this.m_FrameView = new VisualElement();
            this.m_FrameView.name = "FrameView";

            this.m_FrameSeqView = new FrameSequenceView();
            this.m_FrameSeqView.Init(this, this.m_FrameView);

            this.m_FrameInfoView = new FrameInfoView();
            this.m_FrameInfoView.Init(this, this.m_FrameView);
            
            this.m_FrameSeqView.InitStateView(this.m_AnimStateView, this.m_FrameInfoView);
            this.m_FrameInfoView.InitViewData(this.m_AnimStateView, this.m_FrameSeqView);

            this.m_MainView.Add(this.m_FrameView);
            this.rootVisualElement.Add(this.m_MainView);
        }

        /// <summary>
        /// 绘制窗口顶部菜单栏
        /// </summary>
        /// <param name="toolbar"></param>
        private void DoToolBarMenu(Toolbar toolbar)
        {
            var styleSheet = Resources.Load<StyleSheet>("Uss/MainToolBar");
            toolbar.styleSheets.Add(styleSheet);

            if (this.m_Prefab == null)
            {
                this.m_Prefab = new ObjectField();
            }
            this.m_Prefab.RegisterValueChangedCallback(this.OnPrefabValueChange);
            this.m_Prefab.name = "ToolBarPrefab";
            this.m_Prefab.objectType = typeof(GameObject);
            this.m_Prefab.allowSceneObjects = true;
            toolbar.Add(this.m_Prefab);
            
            ToolbarSpacer spacer = new ToolbarSpacer();
            spacer.flex = true;
            toolbar.Add(spacer);

            ToolbarButton saveButton = new ToolbarButton(this.SaveAsset);
            saveButton.text = "保存配置";
            toolbar.Add(saveButton);
        }

        private void OnPrefabValueChange(ChangeEvent<Object> evt)
        {
            GameObject prefab = evt.newValue as GameObject;
            if (prefab == null)
            {
                return;
            }
            if (prefab.GetComponent<ESkeletonAnimation>() == null)
            {
                this.m_Prefab.value = null;
                EditorUtility.DisplayDialog("错误", "预制体类型错误, 请重新添加", "确定");
                return;
            }
            this.m_PrefabData = new PrefabDataParse(evt.newValue as GameObject);
            this.m_AnimClipView?.InitClipList();
            this.m_PrefabData.ESkeletonAnim.JsonFileChangeEvent += this.OnJsonValueChange;
            if (this.m_PrefabData.ESkeletonAnim.ESpineCtrJsonFile == null)
            {
                return;
            }
            this.OnJsonValueChange(this.m_PrefabData.ESkeletonAnim.ESpineCtrJsonFile);
        }

        private void OnJsonValueChange(object json)
        {
            this.m_JsonData = new AnimationJsonData((TextAsset) json);
            this.m_AnimStateView?.ResetStateList();
        }

        public void AddState(List<Spine.Animation> stateList)
        {
            if (stateList.Count == 0)
            {
                return;
            }
            foreach (var state in stateList)
            {
                if (this.m_JsonData.IsStateExist(state.Name))
                {
                    continue;
                }
                StateData data = new StateData(state.Name, state.Duration, ActionFrameWindow.FrameRate);
                foreach (var timeline in state.Timelines)
                {
                    if (timeline.GetType() != typeof(Spine.EventTimeline))
                    {
                        continue;
                    }
                    Spine.EventTimeline curTimeLine = (Spine.EventTimeline) timeline;
                    for (int i = 0; i < curTimeLine.FrameCount; i++)
                    {
                        SpineEventConfig frameData = new SpineEventConfig();
                        frameData.BehaviourName = curTimeLine.Events[i].Data.Name;
                        frameData.BehaviourFrameStartTime = curTimeLine.Events[i].Time;
                        frameData.BehaviourFrameEndTime = curTimeLine.Events[i].Time;
                        data.EventList.Add(frameData);
                    }
                }
                this.m_JsonData.AddState(data);
            }
            this.m_AnimStateView?.ResetStateList();
        }

        public void RemoveState(StateData stateData)
        {
            this.m_JsonData.RemoveState(stateData);
        }

        public void ResetEntryState(string stateName)
        {
            this.m_JsonData.ResetEntryState(stateName);
        }

        public void RefreshFrameViewData(StateData stateData)
        {
            this.m_FrameSeqView?.RefreshSelectedFrameView(stateData);
            this.m_FrameInfoView?.ResetSelectedStateInfo(stateData);
            this.m_FrameInfoView?.ResetSelectedBehaviour(null);
        }
        

        private void SaveAsset()
        {
            if (this.m_JsonData == null)
            {
                return;
            }
            string tip = this.m_JsonData.Save() ? "保存成功..." : "保存失败...";
            this.TipDialog.ShowTip(tip, 2f);
        }

        private void Update()
        {
            float frameTime = Time.realtimeSinceStartup - this.m_LastFrameTime;
            this.m_LastFrameTime = Time.realtimeSinceStartup;

            this.m_FrameSeqView?.ViewUpdate();
            this.m_TipDialog.UpdateTipTime(frameTime);
        }

        private void OnDisable()
        {
            UnityEditor.EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}

