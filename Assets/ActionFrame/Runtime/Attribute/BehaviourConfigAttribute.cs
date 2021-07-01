using System;

namespace ActionFrame.Runtime
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BehaviourConfigAttribute : Attribute
    {
        public Type HandleType;
        public bool AllowedSelected;

        public BehaviourConfigAttribute(Type handleType, bool allowedSelected = true)
        {
            this.HandleType = handleType;
            this.AllowedSelected = allowedSelected;
        }
    }
}