using UnityEditor;
using UnityEngine.UIElements;

namespace ActionFrame.Editor
{
    public abstract class BaseView
    {
        protected VisualElement m_CurView;
        protected VisualElement m_ParentView;
        protected ActionFrameWindow m_Host;
        public abstract void Init(EditorWindow host, VisualElement parent);
    }
}