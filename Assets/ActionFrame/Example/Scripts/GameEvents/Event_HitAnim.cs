using System.Collections.Generic;
using UnityEngine;

namespace ActionFrame.Runtime
{
    public class Event_HitAnim : IEventHandle
    {
        public Vector2 HitSpeed;
        public int FrameIndex;

        public Event_HitAnim(Vector2 hitSpeed, int frameTime)
        {
            this.HitSpeed = hitSpeed;
            this.FrameIndex = frameTime;
        }
    }
}