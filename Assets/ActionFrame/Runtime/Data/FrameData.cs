using System.Collections.Generic;
using UnityEngine;

namespace ActionFrame.Runtime
{
    public class FrameData
    {
        public int FrameNumber;
        public float FrameTime;
        public bool ApplyBeHitBoxToAllFrame;
        public List<BoxItem> BeHitRangeList;
        public List<BoxItem> AttackedRangeList;

        public FrameData()
        {
            this.BeHitRangeList = new List<BoxItem>();
            this.AttackedRangeList = new List<BoxItem>();
        }
    }

    public class BoxItem
    {
        public Vector2 Offset;
        public Vector2 Size;
        public float Rotation;

        public BoxItem()
        {
            this.Offset = new Vector2(0,0);
            this.Size = new Vector2(1,1);
        }
    }
}