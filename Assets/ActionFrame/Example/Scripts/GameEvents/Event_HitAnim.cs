using System.Collections.Generic;
using UnityEngine;

namespace ActionFrame.Runtime
{
    public class Event_HitAnim : IEventHandle
    {
        public Vector2 HitForce;
        public int FrameIndex;

        public Event_HitAnim(Vector2 hitForce, int frameTime)
        {
            this.HitForce = hitForce;
            this.FrameIndex = frameTime;
        }
    }
}