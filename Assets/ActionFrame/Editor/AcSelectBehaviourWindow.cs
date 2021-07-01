using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActionFrame.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ActionFrame.Editor
{
    public class AcSelectBehaviourWindow : EditorWindow
    {
        public static readonly GUIStyle AcSelectWindowBg = (GUIStyle) "flow background";
        private VisualElement m_BG;
        private ScrollView m_ScrollView;
        private List<string> selectNames = new List<string>();
        private Action<int> callback;
        
        public static void ShowTypeWithAttr(Action<Type> callback)
        {
            List<Type> configTypeList = TypeCache.GetTypesWithAttribute<BehaviourConfigAttribute>().ToList();
            List<string> configNames = configTypeList.Where(item =>
            {
                BehaviourConfigAttribute a = (BehaviourConfigAttribute)item.GetCustomAttribute(typeof(BehaviourConfigAttribute), false);
                return a.AllowedSelected;
            }).Select(item => item.Name).ToList();
            OpenWindow(configNames, i => callback(configTypeList[i]));
        }

        private static void OpenWindow(List<string> names, Action<int> callback)
        {
            var win = GetWindow<AcSelectBehaviourWindow>();
            win.selectNames.AddRange(names);
            win.callback = callback;
            win.titleContent = new GUIContent("选择行为配置");
            win.ShowAuxWindow();
            win.OnInit();
        }

        private void OnInit()
        {
            var styleSheet = Resources.Load<StyleSheet>("Uss/AcSelectWindow");
            this.rootVisualElement.styleSheets.Add(styleSheet);
            
            this.m_BG = new IMGUIContainer(() =>
            {
                if (Event.current.type != EventType.Repaint)
                {
                    return;
                }
                AcSelectWindowBg.Draw(this.m_BG.layout, false, false, false, false);
            });
            this.m_BG.name = "WindowBG";
            this.rootVisualElement.Add(this.m_BG);
            
            this.m_ScrollView = new ScrollView();
            this.m_ScrollView.name = "AcSelectWindowScroll";
            this.rootVisualElement.Add(this.m_ScrollView);
            this.DrawScrollView();
        }

        private void DrawScrollView()
        {
            for (int i = 0; i < this.selectNames.Count; i++)
            {
                int index = i;
                Button button = new Button(() =>
                {
                    this.callback.Invoke(index);
                    this.Close();
                });
                button.name = "SelectItem";
                button.text = this.selectNames[i];
                this.m_ScrollView.Add(button);
            }
        }
    }
}