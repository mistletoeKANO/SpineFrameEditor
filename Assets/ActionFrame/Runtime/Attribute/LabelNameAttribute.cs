using System;
using System.Diagnostics;

namespace ActionFrame.Runtime
{
    [Conditional("UNITY_EDITOR")]
    public class LabelNameAttribute : Attribute
    {
        public string LabelName;

        public LabelNameAttribute(string labelName)
        {
            this.LabelName = labelName;
        }
    }
}