using System;
using System.Diagnostics;

namespace ActionFrame.Runtime
{
    [Conditional("UNITY_EDITOR")]
    public class LabelNameAttribute : Attribute
    {
        public string LabelName;
        public string ToolTip;

        public LabelNameAttribute(string labelName, string toolTip = "")
        {
            this.LabelName = labelName;
            this.ToolTip = toolTip;
        }
    }
}