using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ActionFrame.Editor
{
    /// <summary>
    /// 动画 片段 列表视图
    /// </summary>
    public class AnimationClipView : BaseView
    {
        private ScrollView m_ScrollView;
        private VisualElement m_ClipInfo;

        private Label m_ClipName;
        private Label m_ClipDuration;
        private Label m_ClipFrameCount;

        private List<ClipInfo> m_ClipItemList;
        private ClipInfo m_SelectedClip;
        
        public override void Init(EditorWindow host, VisualElement parent)
        {
            this.m_Host = (ActionFrameWindow) host;
            this.m_ParentView = parent;
            this.m_ClipItemList = new List<ClipInfo>();
            this.m_CurView = new IMGUIContainer();
            this.m_CurView.name = "AnimClipView";
            this.m_CurView.AddToClassList("LeftPanelClass");
            this.DoAnimClipView();
            this.m_ParentView.Add(this.m_CurView);
        }
        
        private void DoAnimClipView()
        {
            var styleSheet = Resources.Load<StyleSheet>("Uss/AnimClipView");
            this.m_CurView.styleSheets.Add(styleSheet);
            
            Label label = new Label("角色动画列表");
            label.AddToClassList("ViewTitle");
            this.m_CurView.Add(label);
            
            this.m_ClipInfo = new VisualElement();
            this.m_ClipInfo.name = "AnimClipInfoView";
            
            Label infoTitle = new Label("动画片段基本信息");
            infoTitle.name = "AnimClipViewTitle";
            this.m_ClipInfo.Add(infoTitle);
            
            this.m_ClipName = new Label($"动画名称:");
            this.m_ClipName.AddToClassList("InfoLabel");
            this.m_ClipInfo.Add(this.m_ClipName);
            
            this.m_ClipDuration = new Label($"动画长度:");
            this.m_ClipDuration.AddToClassList("InfoLabel");
            this.m_ClipInfo.Add(this.m_ClipDuration);
            
            this.m_ClipFrameCount = new Label($"动画帧数:");
            this.m_ClipFrameCount.AddToClassList("InfoLabel");
            this.m_ClipInfo.Add(this.m_ClipFrameCount);

            this.m_CurView.Add(this.m_ClipInfo);
            
            this.m_ScrollView = new ScrollView();
            this.m_ScrollView.name = "AnimClipScrollView";
            this.m_CurView.Add(this.m_ScrollView);
        }

        public void InitClipList(PrefabDataParse dataParse)
        {
            if (this.m_Host.PrefabData == null)
            {
                return;
            }
            this.m_ScrollView.contentContainer.RemoveAllChild();
            this.m_ClipItemList.Clear();
            if (dataParse == null || dataParse.ESkeletonAnim.skeleton == null)
            {
                this.m_Host.TipDialog.ShowTip("请使用Hierarchy视图下预制体, 否则无法读取 预制体动画信息.", 15f);
                return;
            }
            foreach (var anim in dataParse.ESkeletonAnim.skeleton.Data.Animations)
            {
                ClipInfo clipInfo = new ClipInfo();
                clipInfo.View = new IMGUIContainer();
                clipInfo.Anim = anim;
                clipInfo.View.RegisterCallback<MouseDownEvent>((e) =>
                {
                    this.OnClickClipItem(e, clipInfo);
                });
                clipInfo.View.RegisterCallback<ContextClickEvent>(this.OnContextClick);
                Label name = new Label($"{anim.Name}");
                name.AddToClassList("ClipItemLabel");
                clipInfo.View.Add(name);
                clipInfo.View.AddToClassList("ClipItem");
                this.m_ScrollView.contentContainer.Add(clipInfo.View);
                this.m_ClipItemList.Add(clipInfo);
                if (this.m_SelectedClip == null)
                {
                    this.m_SelectedClip = clipInfo;
                    this.m_SelectedClip.View.style.backgroundColor = AcFrameStyle.FrameBoxSelect;
                    this.RefreshInfo();
                }
            }
        }

        private void OnClickClipItem(IMouseEvent e, ClipInfo select)
        {
            if (e.button == 0 || e.button == 1)
            {
                this.m_SelectedClip = select;
                foreach (var box in this.m_ScrollView.contentContainer.Children())
                {
                    box.style.backgroundColor = AcFrameStyle.ClipItemNormal;
                }
                select.View.style.backgroundColor = AcFrameStyle.FrameBoxSelect;
                this.RefreshInfo();
            }
        }

        private void OnContextClick(ContextClickEvent e)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("添加到状态列表"), false, this.AddClipToStateList);
            menu.AddItem(new GUIContent("全部添加到状态列表"), false, this.AddAllClipToStateList);
            menu.ShowAsContext();
        }

        private void AddClipToStateList()
        {
            if (this.m_SelectedClip == null)
            {
                return;
            }
            this.AddClipListToStateInternal(new List<Spine.Animation>(){this.m_SelectedClip.Anim});
        }

        private void AddAllClipToStateList()
        {
            if (this.m_ClipItemList.Count == 0)
            {
                return;
            }
            List<Spine.Animation> se = this.m_ClipItemList.Select(item => item.Anim).ToList();
            this.AddClipListToStateInternal(se);
        }

        private void AddClipListToStateInternal(List<Spine.Animation> animList)
        {
            if (this.m_Host.JsonData == null)
            {
                EditorUtility.DisplayDialog("警告", "未添加 Json 动画状态控制文件", "确定");
                return;
            }
            this.m_Host.AddState(animList);
        }

        private void RefreshInfo()
        {
            if (this.m_SelectedClip == null || this.m_SelectedClip.Anim == null)
            {
                return;
            }
            this.m_ClipName.text = $"动画名称: {this.m_SelectedClip.Anim.Name}";
            this.m_ClipDuration.text = $"动画长度: {this.m_SelectedClip.Anim.Duration} S";
            this.m_ClipFrameCount.text = $"动画帧数: {(int)(this.m_SelectedClip.Anim.Duration * ActionFrameWindow.FrameRate)}";
        }
        
        private class ClipInfo
        {
            public IMGUIContainer View;
            public Spine.Animation Anim;
        }
    }
}