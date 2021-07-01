using System;
using System.Diagnostics;

namespace ActionFrame.Runtime
{
    [Conditional("UNITY_EDITOR")]
    public class HideInViewAttribute : Attribute
    {
        
    }
}