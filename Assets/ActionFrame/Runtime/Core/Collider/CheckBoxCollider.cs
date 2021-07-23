﻿using System.Collections.Generic;
using UnityEngine;

namespace ActionFrame.Runtime
{
    public static class CheckBoxCollider
    {
        private static List<ESkeletonAnimation> m_StayAnimList = new List<ESkeletonAnimation>();
        public static List<ESkeletonAnimation> RectColliderEnter(
            this ESkeletonAnimation self,
            List<ESkeletonAnimation> animList,
            int frameIndex)
        {
            List<ESkeletonAnimation> beHitAnim = new List<ESkeletonAnimation>();
            foreach (var monster in animList)
            {
                if (CheckSkeletonCollider(self, monster, frameIndex))
                {
                    beHitAnim.Add(monster);
                }
            }
            return beHitAnim;
        }

        public static List<ESkeletonAnimation> CheckBoxColliderStay(
            this ESkeletonAnimation self,
            List<ESkeletonAnimation> animList)
        {
            return null;
        }

        public static List<ESkeletonAnimation> CheckBoxColliderExit(
            this ESkeletonAnimation self,
            List<ESkeletonAnimation> animList)
        {
            return null;
        }

        private static bool CheckSkeletonCollider(ESkeletonAnimation self, ESkeletonAnimation anim, int frameIndex)
        {
            if (self.CurrentState.FrameDic.Count <= 0)
            {
                return false;
            }
            Dictionary<int, FrameData> selfFrameData = self.CurrentState.FrameDic;
            Dictionary<int, FrameData> animFrameData = anim.CurrentState.FrameDic;
            List<BoxItem> beHitBoxList = null;
            //是否使用 当前状态受击框
            if (anim.DefaultState.UseCurSBeHitBox)
            {
                if (animFrameData.ContainsKey(0) && animFrameData[0].ApplyBeHitBoxToAllFrame)
                {
                    beHitBoxList = animFrameData[0].BeHitRangeList;
                }
                else if (animFrameData.ContainsKey(anim.CurrentFrameCount))
                {
                    beHitBoxList = animFrameData[anim.CurrentFrameCount].BeHitRangeList;
                }
            }
            else
            {
                //如果不使用 当前 状态默认受击框 或者 当前状态当前帧 受击框，则 使用 角色默认 状态 受击框
                Dictionary<int, FrameData> animDefaultFrameData = anim.DefaultState.FrameDic;
                if (animDefaultFrameData.ContainsKey(0))
                {
                    beHitBoxList = animDefaultFrameData[0].BeHitRangeList;
                }
            }
            if (beHitBoxList == null)
            {
                return false;
            }
            List<BoxItem> attackBoxList = selfFrameData[frameIndex].AttackedRangeList;
            return CheckBoxOverlap(self, anim, attackBoxList, beHitBoxList);
        }

        private static bool CheckBoxOverlap(ESkeletonAnimation self, ESkeletonAnimation anim, List<BoxItem> attackBoxList, List<BoxItem> beHitRangeList)
        {
            Queue<Rect> beHitBoxList = new Queue<Rect>();
            Vector2 maxHeight = new Vector2();
            foreach (var box in beHitRangeList)
            {
                Vector3 pos = anim.transform.position + new Vector3(box.Offset.x * anim.skeleton.ScaleX, box.Offset.y);
                pos = pos - (Vector3) box.Size / 2f;
                Rect newRect = new Rect(pos, box.Size);
                beHitBoxList.Enqueue(newRect);
                float newBottom = box.Offset.y - box.Size.y / 2f;
                maxHeight.x = maxHeight.x > newBottom ? newBottom : maxHeight.x;
                float newTop = box.Offset.y + box.Size.y / 2f;
                maxHeight.y = maxHeight.y < newTop ? newTop : maxHeight.y;
            }
            float height = Mathf.Abs(maxHeight.y - maxHeight.x);
            // z 轴 攻击距离 暂定 为 受击框长度 的 1/6, 后续 可 针对单个技能可配
            if (Mathf.Abs(self.transform.parent.position.y - anim.transform.parent.position.y) > height / 6f)
            {
                return false;
            }
            //检查所有攻击框 与 所有 受击框 两两 是否有交集
            foreach (var boxItem in attackBoxList)
            {
                Vector3 pos = self.transform.position +
                              new Vector3(boxItem.Offset.x * self.skeleton.ScaleX, boxItem.Offset.y);
                pos = pos - (Vector3) boxItem.Size / 2f;
                Rect newRect = new Rect(pos, boxItem.Size);
                while (beHitBoxList.Count > 0)
                {
                    Rect popRect = beHitBoxList.Dequeue();
                    if (popRect.Overlaps(newRect)) return true;
                }
            }
            return false;
        }
    }
}