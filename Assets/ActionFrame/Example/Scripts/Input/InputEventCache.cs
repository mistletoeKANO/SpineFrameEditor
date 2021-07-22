using UnityEngine;

namespace ActionFrame.Runtime
{
    public static class InputEventCache
    {
        public static InputEventType EventType;
        public static Vector2 InputAxis;

        public static bool IsHasInput(InputEventType cur)
        {
            return (EventType & cur) == cur;
        }

        public static bool IsOnlyInput(InputEventType cur)
        {
            return (EventType | cur) == cur && EventType != 0;
        }

        public static void Clear()
        {
            EventType = InputEventType.None;
            InputAxis = Vector2.zero;
        }
    }
    
    public enum InputEventType
    {
        None = 0,
        Idle = 1,
        Walk = 2,
        Run = 4,
        Jump = 8,
        Jumping = 16,
        Attack = 32,
    }
}