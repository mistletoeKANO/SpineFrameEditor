using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ActionFrame.Editor
{
    public static class AcFrameStyle
    {
        private static Dictionary<string, object> m_StyleCache = new Dictionary<string, object>();

        public static readonly StyleColor FrameBoxNormal = new StyleColor(new Color(0, 0, 0, 0.5f));
        public static readonly StyleColor FrameBoxSelect = new StyleColor(new Color(255, 255, 255, 0.16f));
        public static readonly StyleColor FrameBoxHover = new StyleColor(new Color(255, 255, 255, 0.0f));
        public static readonly StyleColor FrameBoxRun = new StyleColor(new Color(0f, 0.6f, 1f, 0.28f));
        
        public static readonly StyleColor FrameBehaviourEventNormal = new StyleColor(new Color(0, 0, 0, 0.2f));
        public static readonly StyleColor FrameBehaviourEventSelect = new StyleColor(new Color(1f, 0.2f, 0.01f, 0.1f));
        public static readonly StyleColor FrameBehaviourLabelSelect = new StyleColor(new Color(1f, 0.2f, 0.01f, 0.4f));
        
        public static readonly StyleColor FrameBehaviourBodyNormal = new StyleColor(new Color(0f, 0.91f, 1f, 0.57f));
        public static readonly StyleColor FrameBehaviourBodyHover = new StyleColor(new Color(0f, 0.91f, 1f));

        public static readonly StyleColor ClipItemNormal = new StyleColor(new Color(0, 0, 0, 0.4f));
        
        public static readonly StyleColor PlayOff = new StyleColor(new Color(255, 255, 255, 0.1f));
        public static readonly StyleColor PlayOn = new StyleColor(new Color(0.13f, 0.59f, 1f, 0.52f));

        public static readonly Color HitBoxGizMo = new Color(0f, 0f, 1f, 0.2f);
        public static readonly Color AttackBoxGizMo = new Color(1f,0f,0f,0.2f);

        public static readonly string Icon_Play = "d_PlayButton";
        public static readonly string Icon_Pause = "d_PauseButton";
        public static readonly string Icon_NextFrame = "d_StepButton";
        
        public static T LoadIconWithName<T>(string iconName, bool on = false)
        {
            string key = on ? $"{iconName} on" : iconName;
            if (m_StyleCache.ContainsKey(key))
            {
                T result = (T) m_StyleCache[key];
                if (result != null)
                {
                    return result;
                }
                else
                {
                    result = LoadTInternal(iconName);
                    m_StyleCache[key] = result;
                    return result;
                }
            }
            T icon = LoadTInternal(iconName);
            m_StyleCache.Add(key, icon);

            T LoadTInternal(string name)
            {
                var obj = typeof(EditorGUIUtility).InvokeMember("LoadIcon",
                    BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic, null, null,
                    new object[] {name});
                return (T) obj;
            }
            return icon;
        }
    }
}