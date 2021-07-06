using System.Collections.Generic;
using UnityEngine;

namespace ActionFrame.Runtime
{
    public class KeyCodeCheck
    {
        [LabelName("事件类型")]
        public InputEventType EventType;

        [LabelName("按键类型")]
        public InputType InputType;

        public bool CheckInput(ESkeletonAnimation hero)
        {
            switch (EventType)
                {
                    case InputEventType.Idle:
                        if (!UnityEngine.Input.GetKey(KeyCode.LeftArrow) && !UnityEngine.Input.GetKey(KeyCode.RightArrow))
                        {
                            return true;
                        }
                        return false;
                    case InputEventType.Walk:
                        switch (InputType)
                        {
                            case InputType.Up:
                                if (UnityEngine.Input.GetKeyUp(KeyCode.LeftArrow) || UnityEngine.Input.GetKeyUp(KeyCode.RightArrow))
                                {
                                    return true;
                                }
                                return false;
                            case InputType.KeepDown:
                                if (UnityEngine.Input.GetKey(KeyCode.LeftShift))
                                {
                                    return false;
                                }
                                if (UnityEngine.Input.GetKey(KeyCode.LeftArrow))
                                {
                                    hero.skeleton.ScaleX = -1;
                                    return true;
                                }else if (UnityEngine.Input.GetKey(KeyCode.RightArrow))
                                {
                                    hero.skeleton.ScaleX = 1;
                                    return true;
                                }
                                return false;
                        }
                        return false;
                    case InputEventType.Run:
                        switch (InputType)
                        {
                            case InputType.KeepDown:
                                if (!UnityEngine.Input.GetKey(KeyCode.LeftShift))
                                {
                                    return false;
                                }
                                if (UnityEngine.Input.GetKey(KeyCode.LeftArrow))
                                {
                                    hero.skeleton.ScaleX = -1;
                                    return true;
                                }else if (UnityEngine.Input.GetKey(KeyCode.RightArrow))
                                {
                                    hero.skeleton.ScaleX = 1;
                                    return true;
                                }
                                return false;
                            case InputType.Up:
                                if (UnityEngine.Input.GetKeyUp(KeyCode.LeftShift) || UnityEngine.Input.GetKeyUp(KeyCode.LeftArrow) || UnityEngine.Input.GetKeyUp(KeyCode.RightArrow))
                                {
                                    return true;
                                }
                                return false;
                        }
                        
                        return false;
                    case InputEventType.Jump:
                        if ((UnityEngine.Input.GetKey(KeyCode.Space)))
                        {
                            return true;
                        }
                        return false;
                    case InputEventType.Attack:
                        if ((UnityEngine.Input.GetKey(KeyCode.A)))
                        {
                            return true;
                        }
                        return false;
                    default:
                        break;
                }
                return false;
        }
    }

    public enum InputEventType
    {
        Idle,
        Walk,
        Run,
        Jump,
        Attack,
    }

    public enum InputType
    {
        Down,
        Up,
        KeepDown,
    }
}